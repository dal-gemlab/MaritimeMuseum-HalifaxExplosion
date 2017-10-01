using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity;
using System;
using UnityEngine.VR.WSA;

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
    //The instructional text
    public TextMesh instructionalText;

    

    //Local references;
    private WorldAnchorManager anchorManager;
    private GameObject anchor;
    private GameObject arrow;

    private const string findText = "Please find the QR tag in the wood model";
    private const string foundText = "QR Found, use the clicker";

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
                //Sign up for Vuforia tracking state changes
                var qrTag = GameObject.Find("ImageTarget");
                qrTag.GetComponent<AnchorTrackableEventHandler>().OnAnchorTrackingChanged += QRTagTrackingChanged;
                //Set-up the inst text
                instructionalText.text = findText;

                break;

            
    
            //Final stage where people can interact with the exhibit
            case State.Show:
                currentState = nextState;
                RemoveManipulationCapability();
                AddEnlargeCapability();
                AlignHorizon();
                break;
        }
    }

    private void QRTagTrackingChanged(bool isTracked)
    {
        if (isTracked)
            instructionalText.text = foundText;
        else
            instructionalText.text = findText;
    }

    private void InitializeStates()
    {
        currentState = State.SpatialMaping;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        var anchor = imgTarget.transform.GetChild(0);
        anchor.transform.parent = null;
        //imgTarget.SetActive(false);
        //arCam.SetActive(false);
        Destroy(imgTarget);
        Destroy(arCam);
        Vuforia.VuforiaManager.Instance.Deinit();
        InputManager.Instance.RemoveGlobalListener(this.gameObject);
        //Move the buldings slighty up
        anchor.Translate(new Vector3(0, 0.2f, 0));

        var worldAnchor = anchor.gameObject.AddComponent<WorldAnchor>();
        if (anchorManager.AnchorStore != null)
        {
            anchorManager.AnchorStore.Clear();
            anchorManager.AnchorStore.Save(anchor.name.ToString(), worldAnchor);
        }
        //Destroy the text
        Destroy(instructionalText.gameObject);
        ChangeState(State.Show);
    }

    private void ScanningFinished()
    {
        ChangeState(State.DefineOrigin);
    
    }
    

    public void AddManipulationCapability()
    {
        GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
        foreach(GameObject hologram in holograms)
        {
            ManipulateToMove cap = hologram.GetComponent<ManipulateToMove>();
            if(cap == null)
                hologram.AddComponent<ManipulateToMove>();
        }
    }

    
    private void RemoveManipulationCapability()
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

    
#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Finish Placement"))
        {
            this.ChangeState(State.Show);
        }
        if (GUILayout.Button("Change to Adjusting"))
        {
            AddManipulationCapability();
            this.ChangeState(State.AdjustingBuldings);
        }
    }
#endif

    private void AlignHorizon()
    {
        GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
        foreach (GameObject hologram in holograms)
        {
            var rot = hologram.transform.rotation;
            hologram.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
        }

        return;
    }

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
        foreach (GameObject go in holograms)
        {
            transforms.Add(go.transform);
        }
        PositionFileHelper.SaveRelativePositions(transforms, filename);
    }


}
