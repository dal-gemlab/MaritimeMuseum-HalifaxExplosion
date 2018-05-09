using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

using HoloToolkit.Unity.InputModule;

public class oscControler : MonoBehaviour, IInputClickHandler {

    public string TargetAddr;
    public int OutGoingPort;
    public int InComingPort;
    public float updateIntervalInSeconds;
    public SpeechManager SpeechManager;
    public GameObject hololensCamera;
    public CameraMaterialCaster materialCaster;

    private Dictionary<string, ServerLog> servers;
    private Dictionary<string, ClientLog> clients;
    private readonly string oscID = "HalifaxExplosion";
    private readonly string oscSendPath = "/logger";
    private readonly string oscControlPath = "/control";
    private Coroutine loggerCoroutine;

    private int clickCountSinceLastSend;
    private int lastLogIndex;
    

    // Use this for initialization
    void Start ()
    {
        OSCHandler.Instance.Init(oscID, TargetAddr, OutGoingPort, InComingPort);
        InputManager.Instance.AddGlobalListener(this.gameObject);
        servers = new Dictionary<string, ServerLog>();
        clients = new Dictionary<string, ClientLog>();
        clickCountSinceLastSend = 0;
        lastLogIndex = 0;
        loggerCoroutine = StartCoroutine(SendLogData(updateIntervalInSeconds));
    }



    // Update is called once per frame
    void Update()
    {
        OSCHandler.Instance.UpdateLogs();
        servers = OSCHandler.Instance.Servers;
        //Controll sequence
        foreach (KeyValuePair<string, ServerLog> item in servers)
        {
            if (item.Value.log.Count > lastLogIndex)
            {
                lastLogIndex++;
                int lastPacketIndex = item.Value.packets.Count - 1;
                if(string.Compare(item.Value.packets[lastPacketIndex].Address,oscControlPath) == 0)
                {
                    var recvCount = item.Value.packets[lastPacketIndex].Data.Count;
                    if(recvCount >0)
                    {
                        string commnad = item.Value.packets[lastPacketIndex].Data[0].ToString();
                        SpeechManager.SwitchAdmin();
                        switch (commnad)
                        { 
                            case "Reset":
                                SpeechManager.ResetScene();
                                break;
                            case "Load":
                                StateManager.Instance.MatchPositionsFromFile();
                                break;
                            case "Save":
                                StateManager.Instance.SaveBuldingsTransformToFile();
                                break;
                            case "Translate":
                                SpeechManager.AddDrag();
                                break;
                            case "Rotate":
                                SpeechManager.AddRotate();
                                break;
                            case "StartLog":
                                loggerCoroutine = StartCoroutine(SendLogData(updateIntervalInSeconds));
                                break;
                            case "StopLog":
                                StopCoroutine(loggerCoroutine);
                                break;
                            default:
                                break;

                        }
                        SpeechManager.SwitchAdmin();
                    }
                }
                
            }
        }
    }

    IEnumerator SendLogData(float updateInterval)
    {
        
        while (true)
        {
            OSCHandler.Instance.UpdateLogs();
            servers = OSCHandler.Instance.Servers;
            clients = OSCHandler.Instance.Clients;
            //Data loging sequence
            var p = hololensCamera.transform.position;
            var q = hololensCamera.transform.rotation;

            float[] arrayP = new float[3] { p.x, p.y, p.z };
            float[] arrayQ = new float[4] { q.x, q.y, q.z, q.w };

            List<object> data = new List<object>() { p.x, p.y, p.z, q.x,q.y,q.z,q.w };

            if (materialCaster.gazeTarget != null)
            {

                data.Add(materialCaster.gazeTarget.name);
                if (materialCaster.gazeTarget.CompareTag("Hologram"))
                {
                    data.Add(materialCaster.gazeTarget.GetComponent<ClickToExpand>().IsEnlarged.ToString());
                }
                else
                {
                    if (materialCaster.gazeTarget.CompareTag("PictureFrame"))
                    {
                        data.Add(materialCaster.gazeTarget.GetComponent<PictureFrame>().GetImageName());
                    }
                    else
                    {
                        data.Add("false");
                    }
                    
                }
            }
            else
            {
                data.Add("No Target");
                data.Add("false");
            }
            data.Add(clickCountSinceLastSend);

            OSCHandler.Instance.SendMessageToClient(oscID, oscSendPath, data);

            clickCountSinceLastSend = 0;
            yield return new WaitForSeconds(updateInterval);
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        clickCountSinceLastSend++;
    }
}
