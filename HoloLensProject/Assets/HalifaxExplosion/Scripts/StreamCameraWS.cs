using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;
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

    public bool shouldSend;
    private string addr;
    private bool isAsyncBusy;
    public bool isConnected;
    bool divider = true;
    public CameraMaterialCaster materialCaster;

    private void Start()
    {
        isConnected = false;
        addr = null;
        connectToWS();
        shouldSend = false;
        StartCoroutine(remoteAnchorCoroutine());
        isAsyncBusy = false;
        
    }

    private void FixedUpdate()
    {
        //Clock divider to safe wifi badwith
        if(divider)
        {
            divider = !divider;
            return;
        }

        if (isAsyncBusy)
            return;
        var p = hololensCamera.transform.position;
        var q = hololensCamera.transform.rotation;

        float[] arrayP = new float[3] { p.x, p.y, p.z };
        float[] arrayQ = new float[4] { q.x, q.y, q.z,q.w };

        string gazedBuilding = "";

        if (materialCaster.gazeTarget != null && materialCaster.gazeTarget.CompareTag("Hologram"))
        {
            gazedBuilding = materialCaster.gazeTarget.name;
        }

        //        notABuilding.SetPosRot(arrayP, arrayQ);
        var data = new StreamingData(arrayP, arrayQ, false, "",false, gazedBuilding);

        if (shouldSend)
            sendJS(data);
        divider = !divider;
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
        //var clickedBuilding = new BuildingJS(gameObjectName);
        var p = hololensCamera.transform.position;
        var q = hololensCamera.transform.rotation;

        float[] arrayP = new float[3] { p.x, p.y, p.z };
        float[] arrayQ = new float[4] { q.x, q.y, q.z, q.w };

        var goingToExpand = GameObject.Find(gameObjectName).GetComponent<ClickToExpand>().IsEnlarged;
        string gazedBuilding = "";

        if (materialCaster.gazeTarget != null && materialCaster.gazeTarget.CompareTag("Hologram"))
        {
            gazedBuilding = materialCaster.gazeTarget.name;
        }

        var data = new StreamingData(arrayP,arrayQ,true,gameObjectName, goingToExpand, gazedBuilding);

        if (shouldSend)
            sendJS(data);
    }

    IEnumerator remoteAnchorCoroutine()
    {
        while (true)
        {
            updateRemoteAnchor();
            yield return new WaitForSeconds(5);
        }
    }

    public void updateRemoteAnchor()
    {
        var p = anchor.transform.position;
        var q = anchor.transform.rotation;
        float[] arrayP = new float[3] { p.x, p.y, p.z };
        float[] arrayQ = new float[4] { q.x, q.y, q.z, q.w };

        


        var data = new StreamingData(arrayP,arrayQ,false,null);
        data.SetAnchorUpdate();

        //var notABuildingButAnAnchor = new BuildingJS("anchor", "", "");
        //notABuildingButAnAnchor.SetPosRot(arrayP, arrayQ);

        if (shouldSend)
            sendJS(data);
    }

#if UNITY_EDITOR
    private void sendJS(StreamingData data)
    {
        var json = JsonUtility.ToJson(data);
        ws.Send(json);
    }

    private void connectToWS()
    {
        ws = new WebSocket("ws://192.168.1.6:8888/ws");
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

    private async void sendJS(StreamingData data)
    {
        var ser = new DataContractJsonSerializer(typeof(StreamingData));
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

    public class StreamingData
    {
        public double[] pos;
        public double[] quat;
        public bool click;
        public string clickedName;
        public bool isBuildingEnlarged;
        public bool isAnchorUpdate;
        public string gazedBuilding;

        public StreamingData()
        {
            this.pos = new double[3];
            this.quat = new double[4];
        }

        public StreamingData(float[] pos, float[] quat, bool click, string gazedBuilding)
        {
            this.pos = new double[3];
            this.quat = new double[4];
            this.click = click;
            isAnchorUpdate = false;

            for (int i = 0; i < this.pos.Length; i++)
            {
                this.pos[i] = (float)Math.Round(pos[i], 3, MidpointRounding.AwayFromZero);
            }

            for (int i = 0; i < this.quat.Length; i++)
            {
                this.quat[i] = (float)Math.Round(quat[i], 3, MidpointRounding.AwayFromZero);
            }

            this.gazedBuilding = gazedBuilding;

        }

        public StreamingData(float[] pos, float[] quat, bool click, string clickedName, bool isBuildingEnlarged, string gazedBuilding) : this(pos, quat, click, gazedBuilding)
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
