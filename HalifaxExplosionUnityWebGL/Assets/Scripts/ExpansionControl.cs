using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpansionControl : MonoBehaviour {

    WebSocketManager wsManager;

	// Use this for initialization
	void Start () {

        wsManager = WebSocketManager.Instance;
        wsManager.onMsgReceived += MsgReceived;

    }

    private void MsgReceived(string msg)
    {
        var json = JsonUtility.FromJson<StreamingData>(msg);
        if(json.click)
        {
            var b = GameObject.Find(json.clickedName);
            b.GetComponent<ClickToExpand>().OnInputClicked();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
