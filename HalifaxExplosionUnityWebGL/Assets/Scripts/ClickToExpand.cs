using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// Script that handles enlarging of buldings in Show state.
/// It also takes a picture of the current view of the user
/// </summary>
public class ClickToExpand : MonoBehaviour
{
    public bool isEnlarged;
    private Vector3 modelScale;
    private Vector3 modelPosition;
    private Quaternion modelRotation;
    private GameObject expansionTarget;
    private Vector3 startPos;
    private BuildingDescription buildingDescription;
    private TextMeshPro informationTextBoard;

    private float animationTime = 1f;
    private Vector3 initialScale;
    public Vector3 finalScale;

    public delegate void buildingClicked(string gameObjectName);
    public event buildingClicked OnBuildingClicked;

    private void Start()
    {
        isEnlarged = false;
        //Get the default orientation/scale
        modelScale = transform.localScale;
        modelPosition = transform.position;
        modelRotation = transform.rotation;

        initialScale = transform.localScale;

        expansionTarget = GameObject.Find("ExpansionPoint");
        buildingDescription = this.GetComponent<BuildingDescription>();
        informationTextBoard = GameObject.FindGameObjectWithTag("InformationTextBoard").GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        //Rotate when enlarged. We are not doing this anymore per Brian request.
        //if(isEnlarged)
        //{
        //    transform.Rotate(Vector3.up * 10 * Time.deltaTime, Space.World);
        //}
    }
    public void OnInputClicked()
    {
        if(!isEnlarged)
        {
            //Ensure that there is no other expanded building
            GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
            foreach(var hologram in holograms)
            {
                ClickToExpand expandScript = hologram.GetComponent<ClickToExpand>();
                if (expandScript.isEnlarged)
                    expandScript.OnInputClicked();
            }

            //Save current position and rotation (from secondary load)
            modelPosition = transform.position;
            modelRotation = transform.rotation;

            StartCoroutine(ScaleUp(5, animationTime, 0.2f,expansionTarget.transform.position));
            isEnlarged = !isEnlarged;
            informationTextBoard.SetText(buildingDescription.buildingDescription);
        }
        else
        {
            StartCoroutine(ScaleDown(animationTime, 0.2f));
            this.transform.rotation = modelRotation;
            isEnlarged = !isEnlarged;
            informationTextBoard.SetText("");
            //holoCap = null;
        }

    }

    //Scale up co-routine: scales without freezing application
    IEnumerator ScaleUp(float scaleFactor, float animationTime, float upTranslation, Vector3 endPosition)
    {

        var bounds = this.gameObject.GetComponent<MeshCollider>().bounds;
        var volume = bounds.size.magnitude;  //Vector3.Distance(bounds.max, bounds.min);

        float elapsedTime = 0.0f;
        Vector3 startScale = initialScale;
        Vector3 endScale = initialScale * 1/volume;
        Vector3 startPosition = transform.position;
        startPos = startPosition;

        //Debug.LogFormat("Name: {3} x: {0} y: {1} z: {2}", transform.localPosition.x, transform.localPosition.y, transform.localPosition.z, transform.gameObject.name);

        if (transform.gameObject.name == "mulgrave park")
        {
            endScale = initialScale * 2f / volume;
        }

        while (elapsedTime < animationTime)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / animationTime);
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        this.transform.position = endPosition;

        yield return 0;

    }
    //Scale down co-routine: scales without freezing application
    IEnumerator ScaleDown(float animationTime, float downTranslation)
    {

        float elapsedTime = 0.0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = initialScale;
        Vector3 startPosition = transform.position;
        Vector3 endposition = startPos;

        while (elapsedTime < animationTime)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / animationTime);
            transform.position = Vector3.Lerp(startPosition, endposition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return 0;
    }

}



