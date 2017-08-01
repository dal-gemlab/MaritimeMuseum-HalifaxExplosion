using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity;
using System;

/// <summary>
/// Halifax Explosion State Manager
/// Class that control the flow from Spatial Mapping
///   to showing the buldings to public
/// </summary>
public class StateManager : MonoBehaviour, IInputClickHandler {

    //Prefab containing all the buldings
    public  GameObject anchorPrefab;
    //Arrow model used in the set-up process
    public GameObject placementArrow;
    //Name of the file used for saving relative positions of the models
    public string positionsFile;
    //ARCamera from Vuforia
    public GameObject arCam;
    //Image target for vuforia
    public GameObject imgTarget;

    

    //Local references;
    private WorldAnchorManager anchorManager;
    private GameObject anchor;
    private GameObject arrow;

    //Singleton implementation
    private static StateManager instance;
    public static StateManager Instance
    {
        get
        {
            return instance;
        }
    }
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Trying to instantiante a second instance of the SateManager singleton");
        }
        else
        {
            instance = this;
        }
    }


    //States used (provided?) by the manager
    public enum State {SpatialMaping, DefineOrigin, PlacingAnchor, AdjustingBuldings, Show, SaveBuildings}
    public State currentState { get; private set; }

    //Allowed types of manipulation for the buldings when in edit mode.
    public enum ManipulationMethod {Rotate, Scale, Translate}
    public ManipulationMethod manipulationMethod { get; set; }

    private void Start()
    {
        InitializeStates();
        //Register for the event that signals the end of the spatial mapping
        SpatialMappingManager.Instance.gameObject
            .GetComponent<SpatialMappingLimiter>().finishedMappingEvent += ScanningFinished;
        //Default method in edit is translate
        manipulationMethod = ManipulationMethod.Translate;
        Debug.Log("State Manager Initialized");
        anchorManager = WorldAnchorManager.Instance;
    }
         

    /// <summary>
    /// State control method.
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangeState(State nextState)
    {
        switch (nextState)
        {
            //In this stage the user has to set the place where the origin
            //  for all buldings is and the -*right vector.
            //Reason for -*right: origin is set at the top most right corner
            // of the model
            case State.DefineOrigin:
                //Start the vuforiaCam
                arCam.SetActive(true);
                //Enable the target
                imgTarget.SetActive(true);
                //Let the user click once the tag is in view
                InputManager.Instance.AddGlobalListener(this.gameObject);

                /*arrow = Instantiate(placementArrow);
                //Register for the event raised once the user sets the points
                arrow.GetComponent<TapToSetOrigin>().pointsSetEvent += PointsSet;*/
                break;

            //TODO: Remove this state. The anchor is placed when the origin is defined :)
            case State.PlacingAnchor:
                currentState = nextState;
                anchor = Instantiate(anchorPrefab);
                //Means we have a store and we are inside the hololens
                if(anchorManager.AnchorStore != null)
                    if (anchorManager.AnchorStore.Load("Reference", anchor) != null)
                    {
                        Debug.Log("Anchor exists!");
                        anchor.GetComponent<TapToSetAnchor>().AnchorExistsSkipStep();
                        //Load positions from file (relative from the ancho origin)
                        //MatchPositionsFromFile(positionsFile);
                        this.ChangeState(State.Show);
                    }
                break;

            //Stage that allows the user to adjust all the buldings
            case State.AdjustingBuldings:
                currentState = nextState;
                HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance.DrawVisualMeshes = false;
                AddDragableCapability();
                break;
            
            //Once the adjustment is complete this stage should save the transforms
            case State.SaveBuildings:
                //SaveBuldingsTransformToFile(positionsFile);
                ChangeState(State.Show);
                break;

            //Final stage where people can interact with the exhibit
            case State.Show:
                currentState = nextState;
                Destroy(SpatialMappingManager.Instance.gameObject);
                RemoveDragableCapability();
                AddEnlargeCapability();
                break;
        }
    }

    private void InitializeStates()
    {
        currentState = State.SpatialMaping;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        imgTarget.transform.GetChild(0).transform.parent = null;
        //imgTarget.SetActive(false);
        //arCam.SetActive(false);
        Destroy(imgTarget);
        Destroy(arCam);
        Vuforia.VuforiaManager.Instance.Deinit();
        ChangeState(State.Show);
    }

    private void ScanningFinished()
    {
        ChangeState(State.DefineOrigin);
        SpatialMappingManager.Instance.DrawVisualMeshes = false;
    }

    /// <summary>
    /// Event handler function that is called once the
    /// reference points (origin and right) are set.
    /// </summary>
    /// <param name="points"></param>
    private void PointsSet(List<Vector3> points)
    {
        Destroy(arrow);
        var origin = points[0];
        var xDirection = points[0];
        anchor = Instantiate(anchorPrefab);
        anchor.transform.position = origin;
        //For now, but we need to define how we are adjusting/saving
        //Look how it was done through the anchor store in the placing anchor state
        //We can maybe set them in the editor using the spatial mapped data.
        anchor.GetComponent<TapToSetAnchor>().AnchorExistsSkipStep();
        ChangeState(State.Show);

        //We cant simply use the two points because they are not parallel to the ground
        //First, we need to project the direction onto the Y plane
        Vector3 lookDirection = Vector3.ProjectOnPlane(xDirection, transform.up).normalized;
        anchor.transform.rotation = Quaternion.LookRotation(lookDirection, transform.up);
        //Look rotation expects the forward vector (look) so we compensate by rotating
        //90 after we set the first rotation
        anchor.transform.rotation *= Quaternion.Euler(0, -90, 0);

    }

    private void AddDragableCapability()
    {
        GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
        foreach(GameObject hologram in holograms)
        {
            hologram.AddComponent<ManipulateToMove>();
        }
    }

    
    private void RemoveDragableCapability()
    {
        GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
        foreach (GameObject hologram in holograms)
        {
            ManipulateToMove cap = hologram.GetComponent<ManipulateToMove>();
            if (cap != null)
                Destroy(cap);
        }
    }

    private void AddEnlargeCapability()
    {
        GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
        foreach (GameObject hologram in holograms)
        {
            hologram.AddComponent<ClickToExpand>();
        }
    }
#if UNITY_ENDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Finish Placement"))
        {
            this.ChangeState(State.Show);
        }
        if (GUILayout.Button("Clear Anchors"))
        {
            anchorManager.AnchorStore.Clear();
        }
        if (GUILayout.Button("Change to Adjusting"))
        {
            AddDragableCapability();
            this.ChangeState(State.AdjustingBuldings);
        }
    }
#endif

    private void MatchPositionsFromFile(string filename)
    {
        List<Transform> positions = PositionFileHelper.GetRelativePositions(filename);
        GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");

        if (positions == null)
            return;

        if(positions.Count != holograms.Length)
        {
            Debug.LogError("Postion file has a different number of buldings. Are you sure it is up to date?");
            return;
        }


        for(int i = 0; i<positions.Count; i++)
        {
            Debug.Log("Current: " + holograms[i].name + "From file: " + positions[i].name );
            holograms[i].transform.position = positions[i].position;
            holograms[i].transform.rotation = positions[i].rotation;

        }

    }

    private void SaveBuldingsTransformToFile(string filename)
    {
        GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
        //OK THIS IS UGGLY, TODO: change the helper class
        List<Transform> transforms = new List<Transform>(holograms.Length);
        foreach(GameObject go in holograms)
        {
            transforms.Add(go.transform);
        }
        PositionFileHelper.SaveRelativePositions(transforms , filename);
    }


}
