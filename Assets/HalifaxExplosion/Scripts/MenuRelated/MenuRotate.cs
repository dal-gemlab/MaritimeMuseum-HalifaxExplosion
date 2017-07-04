using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;
using System;

public class MenuRotate : MonoBehaviour, IInputClickHandler
{
    public void OnInputClicked(InputClickedEventData eventData)
    {
        StateManager.Instance.manipulationMethod = StateManager.ManipulationMethod.Rotate;
        Destroy(transform.parent.gameObject);
    }
}
