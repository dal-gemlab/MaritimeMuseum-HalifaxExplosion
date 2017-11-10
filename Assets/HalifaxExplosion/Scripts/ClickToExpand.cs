using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;


#if UNITY_EDITOR
using WebSocketSharp;
#else
using System.Runtime.Serialization.Json;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

/// <summary>
/// Script that handles enlarging of buldings in Show state.
/// It also takes a picture of the current view of the user
/// </summary>
[RequireComponent(typeof(BuildingDescription))]
public class ClickToExpand : MonoBehaviour, IInputClickHandler
{
    private bool isEnlarged;
    private Vector3 modelScale;
    private Vector3 modelPosition;
    private Quaternion modelRotation;
    private Vector3 expansionTarget;
    private Vector3 startPos;
    private BuildingDescription buildingDescription;

    private float animationTime = 1f;
    private Vector3 initialScale;
    public Vector3 finalScale;

    private void Start()
    {
        var gc = GameObject.Find("CameraStreamer");
        isEnlarged = false;
        //Get the default orientation/scale
        modelScale = transform.localScale;
        modelPosition = transform.position;
        modelRotation = transform.rotation;

        initialScale = transform.localScale;

        expansionTarget = GameObject.Find("ExpansionPoint").transform.position;
        buildingDescription = this.GetComponent<BuildingDescription>();
    }

    private void Update()
    {
        //Rotate when enlarged. We are not doing this anymore per Brian request.
        //if(isEnlarged)
        //{
        //    transform.Rotate(Vector3.up * 10 * Time.deltaTime, Space.World);
        //}
    }
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(!isEnlarged)
        {
            //Ensure that there is no other expanded building
            GameObject[] holograms = GameObject.FindGameObjectsWithTag("Hologram");
            foreach(var hologram in holograms)
            {
                var expandScript = hologram.GetComponent<ClickToExpand>();
                if (expandScript.isEnlarged)
                    expandScript.OnInputClicked(null);
            }

            //Save current position and rotation (from secondary load)
            modelPosition = transform.position;
            modelRotation = transform.rotation;

            StartCoroutine(ScaleUp(5, animationTime, 0.2f,expansionTarget));
            isEnlarged = !isEnlarged;


            //var b = new BuildingJS(buildingDescription.modelJSName,
            //    buildingDescription.buildingName,
            //    buildingDescription.buildingDescription);
            //sendBuldingInfo(b);
        }
        else
        {
            StartCoroutine(ScaleDown(animationTime, 0.2f));
            this.transform.rotation = modelRotation;
            isEnlarged = !isEnlarged;
            //holoCap = null;
        }

    }

    //Scale up co-routine: scales without freezing application
    IEnumerator ScaleUp(float scaleFactor, float animationTime, float upTranslation, Vector3 endPosition)
    {
        float elapsedTime = 0.0f;
        Vector3 startScale = initialScale;
        Vector3 endScale = initialScale * scaleFactor;
        Vector3 startPosition = transform.position;
        startPos = startPosition;

        Debug.LogFormat("Name: {3} x: {0} y: {1} z: {2}", transform.localPosition.x, transform.localPosition.y, transform.localPosition.z, transform.gameObject.name);

        while (elapsedTime < animationTime)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / animationTime);
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return 0;

    }
    //Scale down co-routine: scales without freezing application
    IEnumerator ScaleDown(float animationTime, float downTranslation)
    {
        float elapsedTime = 0.0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = initialScale;
        Vector3 startPosition = transform.position;
        Vector3 endposition = startPos;

        while (elapsedTime < animationTime)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / animationTime);
            transform.position = Vector3.Lerp(startPosition, endposition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return 0;
    }

#if UNITY_EDITOR
    private void sendBuldingInfo(BuildingJS b)
    {
        var js = JsonUtility.ToJson(b);

        string serverAddr = "192.168.1.5";
        serverAddr = serverAddr.Trim();
        serverAddr = serverAddr.Substring(serverAddr.LastIndexOf(':') + 1);
        using (var ws = new WebSocket("ws://" + serverAddr + ":8888/ws"))
        {
            ws.OnMessage += (sender, e) =>
                Debug.Log("Laputa says: " + e.Data);

            ws.Connect();
            ws.Send(js);
            ws.Close();
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Expand"))
        {
            if (!isEnlarged)
            {
                StartCoroutine(ScaleUp(5, animationTime, 0.2f, expansionTarget));
                isEnlarged = !isEnlarged;
                var b = new BuildingJS(buildingDescription.modelJSName,
                    buildingDescription.buildingName,
                    buildingDescription.buildingDescription);
                //sendBuldingInfo(b);
            }
            else
            {
                StartCoroutine(ScaleDown(animationTime, 0.2f));
                this.transform.rotation = modelRotation;
                isEnlarged = !isEnlarged;
                //holoCap = null;
            };
        }
    }
#else
    private async void sendBuldingInfo(BuildingJS b)
    {
        var ser = new DataContractJsonSerializer(typeof(BuildingJS));
        var js = new System.IO.MemoryStream();
        ser.WriteObject(js, b);
        string serverAddr = "192.168.1.5";
        serverAddr = serverAddr.Trim();
        serverAddr = serverAddr.Substring(serverAddr.LastIndexOf(':') + 1);

        StreamWebSocket webSock;
        webSock = new StreamWebSocket();
        Uri serverUri = new Uri("ws://"+serverAddr+":8888/ws");
        try
        {
            await webSock.ConnectAsync(serverUri);

            DataWriter messageWrite = new DataWriter(webSock.OutputStream);
            messageWrite.WriteBytes(js.ToArray());
            await messageWrite.StoreAsync();
            webSock.Close(1000, "Closed due to user request.");
        }
        catch (Exception ex)
        {
            Debug.LogFormat("Booo.... something went wrong with the websocket\n{0}", ex.ToString());
        }
    }
#endif

}



