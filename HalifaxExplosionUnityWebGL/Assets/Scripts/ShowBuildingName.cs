using System.Collections;
using System.Collections.Generic;

using UnityEngine;




public class ShowBuildingName : MonoBehaviour {

    
    public GameObject NameBarPrefab;
    [Range(0, 2)]
    public float textHeightInMeters = 0.3f;
    [Range(1, 10)]
    public int hoverTipTimer;
    public string hoverFocusTtext;

    private string BuildingName;
    private LineRenderer lR;
    private bool isAnimating = false;
    private bool isAnimationOnQueue = false;
    private bool animationSingleQueue;
    private GameObject infoBar;
    private TextMesh clickMeText;
    private UnityTimer clickMeTimer;

    public bool WasEnterCalled = false;



    public void OnFocusEnter()
    {
        if (GetComponent<ClickToExpand>().isEnlarged)
            return;

        WasEnterCalled = true;

        clickMeTimer.TimerStart();
        if (isAnimating)
        {
            isAnimationOnQueue = true;
            animationSingleQueue = true;
        }
        StartCoroutine(AnimateLine(false));
    }

    public void OnFocusExit()
    {
        clickMeTimer.TimerStop();
        if (GetComponent<ClickToExpand>().isEnlarged)
            return;

        WasEnterCalled = false;

        if (isAnimating)
        {
            isAnimationOnQueue = true;
            animationSingleQueue = false;
        }
        StartCoroutine(AnimateLine(true));
    }

    public void ClearBar()
    {
        infoBar.GetComponent<TextMesh>().text = "";
        clickMeText.text = "";
        List<Vector3> positions = new List<Vector3>
        {
        };

        lR.positionCount = 0;
        lR.SetPositions(positions.ToArray());
        isAnimationOnQueue = false;
        animationSingleQueue = false;
    }

    // Use this for initialization
    void Start () {

        infoBar = Instantiate(NameBarPrefab);
        infoBar.transform.parent = gameObject.transform;
        infoBar.transform.position = new Vector3(transform.position.x,
                                                 transform.position.y + textHeightInMeters,
                                                 transform.position.z);
        clickMeText = infoBar.transform.GetChild(0).GetComponent<TextMesh>();
        clickMeText.transform.position = new Vector3(infoBar.transform.position.x,
                                                     infoBar.transform.position.y - textHeightInMeters *1/3,
                                                     infoBar.transform.position.z);

        clickMeTimer = gameObject.AddComponent<UnityTimer>();
        clickMeTimer.SetInterval(hoverTipTimer);
        clickMeTimer.AutoReset = false;
        clickMeTimer.Elapsed += () => clickMeText.text = " "+ hoverFocusTtext;


        lR = infoBar.GetComponent<LineRenderer>();
        BuildingName = GetComponent<BuildingDescription>().buildingName;

        StateMangerStateChanged();

    }

    private void StateMangerStateChanged()
    {
        
            GetComponent<ClickToExpand>().OnBuildingClicked += (name) => 
            {
                isAnimationOnQueue = false;
                lR.positionCount = 0;
                infoBar.GetComponent<TextMesh>().text = "";
                clickMeText.text = "";
            };
        
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
        List<Vector3> positions = new List<Vector3>
        {
            this.transform.position,
            new Vector3()
        };

        if (shouldGoDown)
        {
            infoBar.GetComponent<TextMesh>().text = "";
            clickMeText.text = "";
        }

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
            if(!animationSingleQueue)
                StartCoroutine(AnimateLine(!shouldGoDown));
        }

    }

}
