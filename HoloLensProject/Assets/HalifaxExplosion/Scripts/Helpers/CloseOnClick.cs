using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;
using System;

public class CloseOnClick : MonoBehaviour, IInputClickHandler
{

    // Use this for initialization
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Destroy(this.gameObject);
    }
}
