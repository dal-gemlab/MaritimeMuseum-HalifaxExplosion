using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WSManager : MonoBehaviour {

    WebSocket socket;
    public string addr = "ws://localhost:8888/ws";


    public delegate void msgReceived(string msg);
    public event msgReceived onMsgReceived;

    IEnumerator Start()
    {
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
                if(onMsgReceived != null)
                    onMsgReceived.Invoke(json);
            }
            yield return 0;
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

        public StreamingData(float[] pos, float[] quat, bool click)
        {
            this.pos = pos;
            this.quat = quat;
            this.click = click;
            isAnchorUpdate = false;
        }

        public StreamingData(float[] pos, float[] quat, bool click, string clickedName, bool isBuildingEnlarged) : this(pos, quat, click)
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
}
