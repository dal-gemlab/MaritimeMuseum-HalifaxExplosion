using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;
using System;

public class HandTracker : MonoBehaviour, ISourceStateHandler
{

    Camera cam;
    GameObject c;
    public GameObject menuPrefab;

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        Debug.Log("Hand Detected");

        //only show the menu if there is no object on focus
        //31 is the layer for the spatial mapping
        if ((GazeManager.Instance.HitObject == null || GazeManager.Instance.HitObject.layer == 31) && c == null
            && StateManager.Instance.currentState == StateManager.State.AdjustingBuldings) 
        {
            c = GameObject.Instantiate(menuPrefab);
            //c.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            c.transform.position = cam.transform.position;
            c.transform.position += new Vector3(0, 0, 1.5f);
            c.transform.RotateAround(cam.transform.position, Vector3.right, cam.transform.rotation.eulerAngles.x);
            c.transform.RotateAround(cam.transform.position, Vector3.up, cam.transform.rotation.eulerAngles.y);
            c.transform.Translate(Vector3.right / 10f);
            //c.transform.parent = cam.transform;
        }
        else if(c != null)
        {
            Destroy(c);
        }


    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        Debug.Log("Hand Lost");
    }

    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
        cam = Camera.main;
    }

}
