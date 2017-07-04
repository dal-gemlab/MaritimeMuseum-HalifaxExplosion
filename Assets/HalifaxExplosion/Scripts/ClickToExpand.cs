using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

/// <summary>
/// Script that handles enlarging of buldings in Show state.
/// It also takes a picture of the current view of the user
/// </summary>
public class ClickToExpand : MonoBehaviour, IInputClickHandler
{
    private bool isEnlarged;
    private Vector3 modelScale;
    private Vector3 modelPosition;
    private Quaternion modelRotation;

    private float animationTime = 1f;
    private Vector3 initialScale;
    public Vector3 finalScale;

    private HoloCapture holoCap;

    private void Start()
    {
        isEnlarged = false;
        //Get the default orientation/scale
        modelScale = transform.localScale;
        modelPosition = transform.position;
        modelRotation = transform.rotation;

        initialScale = transform.localScale;
    }

    private void Update()
    {
        //Rotate when enlarged. We are not doing this anymore per Brian request.
        //if(isEnlarged)
        //{
        //    transform.Rotate(Vector3.up * 10 * Time.deltaTime, Space.World);
        //}
    }
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(!isEnlarged)
        {
            //TODO: Enable this and send to dispacher
            //holoCap = new HoloCapture();
            //holoCap.TakePicture();
            StartCoroutine(ScaleUp(5, animationTime, 0.2f));
            isEnlarged = !isEnlarged;
        }
        else
        {
            StartCoroutine(ScaleDown(animationTime, 0.2f));
            this.transform.rotation = modelRotation;
            isEnlarged = !isEnlarged;
            //holoCap = null;
        }

    }

    //Scale up co-routine: scales without freezing application
    IEnumerator ScaleUp(float scaleFactor, float animationTime, float upTranslation)
    {
        float elapsedTime = 0.0f;
        Vector3 startScale = initialScale;
        Vector3 endScale = initialScale * scaleFactor;
        Vector3 startPosition = transform.position;
        Vector3 endposition = transform.position;
        endposition.y += upTranslation;



        while (elapsedTime < animationTime)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / animationTime);
            transform.position = Vector3.Lerp(startPosition, endposition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return 0;

    }
    //Scale down co-routine: scales without freezing application
    IEnumerator ScaleDown(float animationTime, float downTranslation)
    {
        float elapsedTime = 0.0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = initialScale;
        Vector3 startPosition = transform.position;
        Vector3 endposition = transform.position;
        endposition.y -= downTranslation;



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



