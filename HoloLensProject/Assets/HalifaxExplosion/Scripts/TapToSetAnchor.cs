using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using System;

public class TapToSetAnchor : MonoBehaviour, IInputClickHandler {
    public void OnInputClicked(InputClickedEventData eventData)
    {
        StateManager.Instance.ChangeState(StateManager.State.AdjustingBuldings);
        foreach (Transform child in transform)
        {
            MeshRenderer r = child.GetComponent<MeshRenderer>();
            if (r != null)
            {
                r.enabled = true;
            }
             
        }
        gameObject.GetComponent<LineRenderer>().enabled = false;
        //WorldAnchorManager.Instance.AttachAnchor(gameObject, "Reference");
        
        Destroy(this);

        
    }

	// Update is called once per frame
	void Update () {
        //If we are looking at the env mesh
        if (GazeManager.Instance.HitObject != null)
        {
            //Rotate to look the camera
            Vector3 lookDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.up).normalized;
            transform.rotation = Quaternion.LookRotation(lookDirection, transform.up);

            //We only want hits againts the spatial mesh
            if (GazeManager.Instance.HitObject.layer == 31)
            {
                RaycastHit hit = GazeManager.Instance.HitInfo;
                transform.position = hit.point;
            }
        }
	}

    public void AnchorExistsSkipStep()
    {
        gameObject.GetComponent<LineRenderer>().enabled = false;
        
        Destroy(this);
    }
}
