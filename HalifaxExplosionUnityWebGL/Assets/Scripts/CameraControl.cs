using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    WebSocketManager wsManager;
    Camera cam;
    private GameObject previousGazedBuilding;

    public GameObject anchor;

    private void Start()
    {
        wsManager = WebSocketManager.Instance;
        cam = Camera.main;
        wsManager.onMsgReceived += msgReceived;
    }

    private void msgReceived(string msg)
    {
        //Contains the relative position from the camera to the anchor
        var json = JsonUtility.FromJson<StreamingData>(msg);
        if(json.isAnchorUpdate)
            UpdateAnchorPosition(json);
        else
        {
            cam.transform.position = cam.transform.position * 0.9f +
                new Vector3(json.pos[0], json.pos[1], json.pos[2]) * 0.1f;
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation,
                new Quaternion(json.quat[0], json.quat[1], json.quat[2], json.quat[3]),
                0.1f);

            cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x,
                                        cam.transform.eulerAngles.y, 0f);

            if (json.click)
            {
                var b = GameObject.Find(json.clickedName);
                if (b != null)
                {
                    previousGazedBuilding.GetComponent<ShowBuildingName>().ClearBar();
                    previousGazedBuilding = null;
                    var expandComponent = b.GetComponent<ClickToExpand>();
                    if (expandComponent != null)
                    {
                        if(expandComponent.isEnlarged == json.isBuildingEnlarged)
                            expandComponent.OnInputClicked();
                    }
                }
            }
            else
            {
                var b = GameObject.Find(json.gazedBuilding);
                if (b == null)
                {
                    if (previousGazedBuilding != null)
                    {
                        previousGazedBuilding.GetComponent<ShowBuildingName>().OnFocusExit();
                        previousGazedBuilding = null;
                    }

                    return;
                }
                if(!b.CompareTag("Hologram"))
                    return;
                if(b.GetComponent<ClickToExpand>().ScalingDownInProgress)
                    return;

                if(!b.GetComponent<ShowBuildingName>().WasEnterCalled)
                    b.GetComponent<ShowBuildingName>().OnFocusEnter();

                if (previousGazedBuilding == null)
                    previousGazedBuilding = b;
                else if(!previousGazedBuilding.name.Equals(b.name))
                {
                    previousGazedBuilding.GetComponent<ShowBuildingName>().OnFocusExit();
                    previousGazedBuilding = b;
                }
            }

        }
        
        
    }

    private void UpdateAnchorPosition(StreamingData data)
    {
        Debug.Log("Anchor update");
        anchor.transform.position = new Vector3(data.pos[0], data.pos[1], data.pos[2]);
        anchor.transform.rotation = new Quaternion(data.quat[0], data.quat[1], data.quat[2], data.quat[3]);
    }
}
