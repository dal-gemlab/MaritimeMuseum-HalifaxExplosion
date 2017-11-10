using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using WebSocketSharp;
#else
using System.Runtime.Serialization.Json;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

public class StreamCameraWS : MIMIR.Util.Singleton<StreamCameraWS> {

#if UNITY_EDITOR
    WebSocket ws;
#else
    StreamWebSocket ws;
    DataWriter messageWrite;
#endif
    public GameObject hololensCamera;
    public GameObject anchor;
    //Reusing the same data structure...
    BuildingJS notABuilding;
    public bool shouldSend;
    public string addr;
    private bool isAsyncBusy;

    private void Start()
    {
        connectToWS();
        notABuilding = new BuildingJS();
        shouldSend = false;
        StartCoroutine(updateRemoteAnchor());
        isAsyncBusy = false;
        
    }

    private void FixedUpdate()
    {
        if (isAsyncBusy)
            return;
        var p = hololensCamera.transform.position;
        var q = hololensCamera.transform.rotation;

        float[] arrayP = new float[3] { p.x, p.y, p.z };
        float[] arrayQ = new float[4] { q.x, q.y, q.z,q.w };

        notABuilding.SetPosRot(arrayP, arrayQ);
        
        if (shouldSend)
            sendJS(notABuilding);
    }

    IEnumerator updateRemoteAnchor()
    {
        while (true)
        {
            var p = anchor.transform.position;
            var q = anchor.transform.rotation;
            float[] arrayP = new float[3] { p.x, p.y, p.z };
            float[] arrayQ = new float[4] { q.x, q.y, q.z, q.w };

            var notABuildingButAnAnchor = new BuildingJS("anchor", "", "");
            notABuildingButAnAnchor.SetPosRot(arrayP, arrayQ);
            
            if (shouldSend)
                sendJS(notABuildingButAnAnchor);
            yield return new WaitForSeconds(20);
        }
    }

#if UNITY_EDITOR
    private void sendJS(BuildingJS data)
    {
        var json = JsonUtility.ToJson(data);
        ws.Send(json);
    }

    private void connectToWS()
    {
        ws = new WebSocket("ws://localhost:8888/ws");
        ws.Connect();
    }
#else
    private async void connectToWS()
    {
        string serverAddr = addr;
        serverAddr = serverAddr.Trim();
        serverAddr = serverAddr.Substring(serverAddr.LastIndexOf(':') + 1);
        ws = new StreamWebSocket();
        Uri serverUri = new Uri("ws://"+serverAddr+":8888/ws");
        try
        {
            await ws.ConnectAsync(serverUri);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Booo.... something went wrong with the websocket\n{0}", ex.ToString());
        }
        messageWrite = new DataWriter(ws.OutputStream);
    }

    private async void sendJS(BuildingJS data)
    {
        var ser = new DataContractJsonSerializer(typeof(BuildingJS));
        var js = new System.IO.MemoryStream();
        ser.WriteObject(js, data);
        
        messageWrite.WriteBytes(js.ToArray());
        try
        {
            isAsyncBusy = true;          
            await messageWrite.StoreAsync();
            isAsyncBusy = false;
        }
        catch(Exception ex)
        {
            Debug.Log($"Sending problem: {ex.ToString()}");
        }
    }

#endif
}
