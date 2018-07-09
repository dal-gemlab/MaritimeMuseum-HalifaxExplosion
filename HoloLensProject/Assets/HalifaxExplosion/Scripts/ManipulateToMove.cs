using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class ManipulateToMove : MonoBehaviour, IManipulationHandler {

    //[Range(10, 100)]
    //public int manipulationDampner;
    private Vector3 startPosition;

    private readonly float moveDampner = 70;
    private readonly float rotationDampner = 0.5f;

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.RemoveGlobalListener(gameObject);
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        
        InputManager.Instance.RemoveGlobalListener(gameObject);
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        startPosition = gameObject.transform.position;
        //register as a universal receiver for now
        InputManager.Instance.AddGlobalListener(gameObject);
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        switch(StateManager.Instance.manipulationMethod)
        {
            case StateManager.ManipulationMethod.Translate:
                gameObject.transform.position += eventData.CumulativeDelta / moveDampner;
                break;
            case StateManager.ManipulationMethod.Rotate:
                Vector3 rot = new Vector3(0, eventData.CumulativeDelta.x/rotationDampner, 0);
                //Debug.Log(rot);
                gameObject.transform.Rotate(rot);
                

                break;
            default:
                Debug.LogError("Method not yet implemented!");
                break;
        }
        
    }

    private void Update()
    {
        
    }


}
