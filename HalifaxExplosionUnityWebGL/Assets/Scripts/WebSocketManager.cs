using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebSocketManager : MIMIR.Util.Singleton<WebSocketManager> {

    WebSocket socket;

    public string addr = "ws://localhost:8888/ws";
    public delegate void msgReceived(string msg);
    public event msgReceived onMsgReceived;

    // Use this for initialization
    IEnumerator Start() {
        socket = new WebSocket(new Uri(addr));
        yield return StartCoroutine(socket.Connect());
        StartCoroutine(ReceiveMessages());
        
    }

    IEnumerator ReceiveMessages()
    {
        
        while (true)
        {
            

            string json = socket.RecvString();
            if (json != null)
            {
        
                onMsgReceived?.Invoke(json);
            }
            yield return 0;
        }
    }

    
}

public class StreamingData
{
    public float[] pos;
    public float[] quat;
    public bool click;
    public string clickedName;
    public bool isBuildingEnlarged;
    public bool isAnchorUpdate;
    public string gazedBuilding;

    public StreamingData(float[] pos, float[] quat, bool click, string gazedBuilding)
    {
        this.pos = pos;
        this.quat = quat;
        this.click = click;
        isAnchorUpdate = false;
        this.gazedBuilding = gazedBuilding;
    }
    

    public StreamingData(float[] pos, float[] quat, bool click, string clickedName, bool isBuildingEnlarged, string gazedBuilding) : this(pos, quat, click,gazedBuilding )
    {
        this.clickedName = clickedName;
        this.isBuildingEnlarged = isBuildingEnlarged;
        isAnchorUpdate = false;
    }

    public void SetAnchorUpdate()
    {
        isAnchorUpdate = true;
    }

}



