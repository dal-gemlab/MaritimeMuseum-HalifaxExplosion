using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;
using System;

public class TapToSetOrigin : MonoBehaviour, IInputClickHandler
{
    public delegate void OriginPointsSet( List<Vector3> points);
    public event OriginPointsSet pointsSetEvent;

    private List<Vector3> originPoints;


    public void OnInputClicked(InputClickedEventData eventData)
    {
        //TODO: Copy code from the update to ensure that
        //the point added belongs to the spatial mesh
        originPoints.Add(transform.position);
        
    }

    // Use this for initialization
    void Start () {
        originPoints = new List<Vector3>();
        InputManager.Instance.AddGlobalListener(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (GazeManager.Instance.HitObject != null)
        {
            //We only want hits againts the spatial mesh
            if (GazeManager.Instance.HitObject.layer == 31)
            {
                RaycastHit hit = GazeManager.Instance.HitInfo;
                transform.position = hit.point;
            }
        }

        if(originPoints.Count == 2)
        {
            InputManager.Instance.RemoveGlobalListener(this.gameObject);
            if (pointsSetEvent != null)
                pointsSetEvent(originPoints);
        }
        
    }
}
