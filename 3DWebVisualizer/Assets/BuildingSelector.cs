using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    private WSManager wsManager;
    private MeshRenderer previousSelected;
    // Use this for initialization
    void Start ()
	{
	    wsManager = GameObject.Find("WSManager").GetComponent<WSManager>();
        wsManager.onMsgReceived += MessageReceived;
	}

    private void MessageReceived(string msg)
    {
        var data = JsonUtility.FromJson<WSManager.StreamingData>(msg);
        if (!data.isAnchorUpdate && data.click && !data.isBuildingEnlarged)
        {
            if (previousSelected != null)
                previousSelected.enabled = false;
            previousSelected = GameObject.Find(data.clickedName).transform.GetChild(0)
                .GetComponent<MeshRenderer>();
            previousSelected.enabled = true;
        }
    }

   
}
