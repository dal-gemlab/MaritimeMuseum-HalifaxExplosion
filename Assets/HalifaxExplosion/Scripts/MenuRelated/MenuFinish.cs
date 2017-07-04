using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;
using System;

public class MenuFinish : MonoBehaviour, IInputClickHandler {
    public void OnInputClicked(InputClickedEventData eventData)
    {
        StateManager.Instance.ChangeState(StateManager.State.SaveBuildings);
        Destroy(transform.parent.gameObject);

        GameObject anchor = GameObject.FindGameObjectWithTag("StageAnchor");
        foreach(Transform child in anchor.transform)
        {
            Debug.Log(child.name + " " + child.transform.localPosition.ToString());
        }
    }

}
