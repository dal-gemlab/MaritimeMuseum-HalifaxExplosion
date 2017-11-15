using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using WebSocketSharp;
#else
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
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
    private string addr;
    private bool isAsyncBusy;
    public bool isConnected;

    private void Start()
    {
        isConnected = false;
        addr = null;
        connectToWS();
        notABuilding = new BuildingJS();
        shouldSend = false;
        StartCoroutine(remoteAnchorCoroutine());
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

    public void SignForExpansion()
    {
        GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
        foreach (GameObject b in holograms)
        {
            b.GetComponent<ClickToExpand>().OnBuildingClicked += BuildingClicked;
        }

    }

    private void BuildingClicked(string gameObjectName)
    {
        var clickedBuilding = new BuildingJS(gameObjectName);
        if (shouldSend)
            sendJS(clickedBuilding);
    }

    IEnumerator remoteAnchorCoroutine()
    {
        while (true)
        {
            updateRemoteAnchor();
            yield return new WaitForSeconds(20);
        }
    }

    public void updateRemoteAnchor()
    {
        var p = anchor.transform.position;
        var q = anchor.transform.rotation;
        float[] arrayP = new float[3] { p.x, p.y, p.z };
        float[] arrayQ = new float[4] { q.x, q.y, q.z, q.w };

        var notABuildingButAnAnchor = new BuildingJS("anchor", "", "");
        notABuildingButAnAnchor.SetPosRot(arrayP, arrayQ);

        if (shouldSend)
            sendJS(notABuildingButAnAnchor);
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
        ws.OnOpen += ConnectedEvent;
        ws.Connect();
    }

    private void ConnectedEvent(object sender, EventArgs e)
    {
        isConnected = true;
    }
#else
    private async void connectToWS()
    {
        string serverAddr = await loadWSHostAddr();
        if (serverAddr == null)
        {
            Debug.LogError("No file containing host addr");
            isConnected = false;
            return;
        }
        ws = new StreamWebSocket();
        Uri serverUri = new Uri(serverAddr);
        try
        {
            await ws.ConnectAsync(serverUri);
            isConnected = true;
            messageWrite = new DataWriter(ws.OutputStream);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Booo.... something went wrong with the websocket\n{0}", ex.ToString());
            isConnected = false;
            messageWrite = null;
        }
        
        
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

    private async Task<string> loadWSHostAddr()
    {
        Stream stream = null;
        StorageFolder sF = ApplicationData.Current.LocalFolder;
        StorageFile hostFile;

        try
        {
            hostFile = await sF.GetFileAsync("host");
        }
        catch(Exception e)
        {
            //File does not exist, we should not stream
            Debug.LogError(e.ToString());
            return null;
        }
        stream = await hostFile.OpenStreamForReadAsync();
        StreamReader sR = new StreamReader(stream);
        string addr = await sR.ReadLineAsync();
        stream.Dispose();
        return addr;
    }

#endif
}
