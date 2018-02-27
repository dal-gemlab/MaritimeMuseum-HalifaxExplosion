using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;

[RequireComponent(typeof(BuildingDescription))]
public class ShowBuildingName : MonoBehaviour, IFocusable {

    
    public GameObject NameBarPrefab;
    [Range(0, 2)]
    public float textHeightInMeters = 0.5f;

    private string BuildingName;
    private LineRenderer lR;
    private bool isAnimating = false;
    private bool isAnimationOnQueue = false;
    private GameObject infoBar;

    public void OnFocusEnter()
    {
        if (isAnimating)
            isAnimationOnQueue = true;
        StartCoroutine(AnimateLine(false));
    }

    public void OnFocusExit()
    {
        if (isAnimating)
            isAnimationOnQueue = true;
        StartCoroutine(AnimateLine(true));
    }

    // Use this for initialization
    void Start () {

        infoBar = Instantiate(NameBarPrefab);
        infoBar.transform.parent = gameObject.transform;
        infoBar.transform.position = new Vector3(transform.position.x,
                                                 transform.position.y + textHeightInMeters,
                                                 transform.position.z);
        
        //infoBar.transform.rotation = Quaternion.identity;
        //infoBar.transform.parent = this.gameObject.transform;
        //infoBar.transform.position = new Vector3(transform.position.x, transform.position.y + textHeightInMeters,
         //                                       transform.position.z);

        lR = infoBar.GetComponent<LineRenderer>();
        BuildingName = GetComponent<BuildingDescription>().buildingName;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator AnimateLine(bool shouldGoDown)
    {
        if (lR.positionCount != 2)
            lR.positionCount = 2;

        if (isAnimating)
            yield break;
        isAnimating = true;

        var startTime = Time.time;
        var length = Vector3.Distance(this.transform.position, infoBar.transform.position);
        var distCovered = 0.0f;
        Vector3 targetPosition = new Vector3();
        List<Vector3> positions = new List<Vector3>();

            positions.Add(this.transform.position);
            positions.Add(new Vector3());
        
        if(shouldGoDown)
            infoBar.GetComponent<TextMesh>().text = "";

        while (distCovered < length)
        {
            distCovered = (Time.time - startTime) * 0.4f;
            float fracJourney = distCovered / length;
            if(!shouldGoDown)
                targetPosition = Vector3.Lerp(this.transform.position, infoBar.transform.position, fracJourney);
            else
                targetPosition = Vector3.Lerp(infoBar.transform.position, this.transform.position, fracJourney);
            positions[1] = targetPosition;
            lR.SetPositions(positions.ToArray());
            yield return new WaitForEndOfFrame();
        }
        if (!shouldGoDown)
        {
            infoBar.GetComponent<TextMesh>().text = " " + BuildingName;
            yield return new WaitForSeconds(5);
        }
        isAnimating = false;
        if(isAnimationOnQueue)
        {
            isAnimationOnQueue = false;
            StartCoroutine(AnimateLine(!shouldGoDown));
        }

    }
}
