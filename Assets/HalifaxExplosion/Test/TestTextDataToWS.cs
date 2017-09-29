#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using WebSocketSharp;

public class TestTextDataToWS : MonoBehaviour {

    public string jsname;
    public string bname;
    [TextArea(3, 10)]
    public string text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Send(string data)
    {
        string serverAddr = "127.0.0.1";
        serverAddr = serverAddr.Trim();
        serverAddr = serverAddr.Substring(serverAddr.LastIndexOf(':') + 1);
        using (var ws = new WebSocket("ws://" + serverAddr + ":8888/ws"))
        {
            ws.OnMessage += (sender, e) =>
                Debug.Log("Laputa says: " + e.Data);

            ws.Connect();
            ws.Send(data);
            ws.Close();
            //ws.Send("BALUS");

        }
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Send Text"))
        {
            var b = new BuildingJS();
            b.modelJSName = jsname;
            b.buildingName = bname;
            b.description = text;
            var js = JsonUtility.ToJson(b);
            this.Send(js);
        }
    }

    public class BuldingInfo
    {
        public string name;
        public string description;
    }
}
#endif
