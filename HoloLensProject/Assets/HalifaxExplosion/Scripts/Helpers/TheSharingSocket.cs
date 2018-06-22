using UnityEngine;



#if UNITY_EDITOR
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
#else
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System;
#endif




//Helper class for using TCP Streams inside Unity
//Anchor data is ~10Mb and UnityNetowrking classes are not
//good at handling that amount of data =/

//Unity DOES NOT LIKE THREADS this is a ugly hack
//e.g. Unity does not let external threads to access
//the main thread monobehaviour clases

public class TheSharingSocket
{
#if UNITY_EDITOR
    private TcpListener listener = null;
    private Socket socket = null;
    public static NetworkStream stream;

    private bool gotData;
    byte[] anchor;

    public delegate void OnDataReady(byte[] data);
    public event OnDataReady dataReadyEvent;

    public TheSharingSocket()
    {
        IPAddress addr = IPAddress.Loopback;
        listener = new TcpListener(11000);
        listener.Start();
        gotData = false;
        Thread t = new Thread(() => StartListen(ref anchor, ref gotData, ref dataReadyEvent));
        t.Start();
    }

    public TheSharingSocket(byte[] data)
    {
        listener = new TcpListener(11000);
        listener.Start();
        Thread t = new Thread(() => StartSend(data));
        t.Start();
    }

    private void StartListen(ref byte[] data, ref bool recvDone, ref OnDataReady dataReady)
    {
        
        while (true)
        {
            socket = listener.AcceptSocket();
            Console.WriteLine("new folk");
            stream = new NetworkStream(socket);
            byte[] t = new byte[4];
            stream.Read(t, 0, 4);
            Array.Reverse(t);
            int size = BitConverter.ToInt32(t, 0);
            Debug.Log("Receiving: " + size);
            anchor = new byte[size];
            int recvBytes = 0;
            while(recvBytes < size)
            {
                int acqSize = 1024;
                if (size - recvBytes < 1024)
                    acqSize = size - recvBytes;
                recvBytes += stream.Read(anchor, recvBytes, acqSize);
            }
            
            Debug.LogFormat("Received {0} bytes expected {1}", recvBytes, size);
            break;
        }
        recvDone = true;
        socket.Close();
        dataReady(anchor);
    }

    //Read only (race conditions should no apply)
    private void StartSend(byte[] data)
    {
        while (true)
        {
            socket = listener.AcceptSocket();
            Debug.Log("New folk to send stuff");
            stream = new NetworkStream(socket);
            byte[] size = new byte[4];
            byte[] anchorToSend = data;
            size = BitConverter.GetBytes(data.Length);
            Array.Reverse(size);
            stream.Write(size, 0, 4);
            stream.Write(anchorToSend, 0, anchorToSend.Length);
            Debug.Log("Sending...");
            break;
        }
        socket.Close();        
    }

    //The HoloLens does not uses .NET sockets (neither it can use)
    //Because it is a Universal app it have to use the Windows classes
    //Yeah, it sucks.
    //TODO: What is the right directive?
#elif !UNITY_EDITOR
    /*private int connectionPort = 11000;
    private string serverIP; 
    private byte[] dataBuffer;
    private int size;
    private StreamSocket networkConnection;

    public delegate void OnDataReady(byte[] data);
    public event OnDataReady dataReadyEvent;



    //If we want to send
    public TheSharingSocket(byte[] anchor, string addr)
    {
        dataBuffer = anchor;
        serverIP = addr;
    }
    //If we want to receive
    public TheSharingSocket(string addr)
    {
        serverIP = addr;
    }

    //Set-up the network listener (TCP)
    //and wait for connection
    //this looks a little odd but hey :)
    //TODO: Smart this up
    public void SendAnchor()
    {
        //Welcome to the marverolous world of async work in UWP
        Debug.Log("Connecting to " + serverIP);
        HostName host = new HostName(serverIP);
        networkConnection = new StreamSocket();
        IAsyncAction connectAction = networkConnection.ConnectAsync(host, "11000");
        AsyncActionCompletedHandler connectHandler = new AsyncActionCompletedHandler(sendHandler);
        connectAction.Completed = connectHandler;
        //HCStateManager.Instance.ChangeState(HCStateManager.AppStates.ClickToAnchor);
        HCStateManager.Instance.ChangeState(HCStateManager.AppStates.SearchingForTag);
    }

    private async void sendHandler(IAsyncAction asyncInfo, AsyncStatus status)
    {
        if (status == AsyncStatus.Completed)
        {
            Debug.Log("connected");
            // If we have data, send it. 
            if (dataBuffer != null)
            {
                IOutputStream stream = networkConnection.OutputStream;
                using (DataWriter writer = new DataWriter(stream))
                {
                    //Sends size first
                    //little endian
                    writer.WriteInt32(dataBuffer.Length);
                    writer.WriteBytes(dataBuffer);
                    await writer.StoreAsync();//.AsTask().Wait();
                    writer.FlushAsync().AsTask().Wait();
                }

            }
            else
            {
                Debug.LogError("No data to send but we've been connected to.  This is unexpected.");
            }
        }
    }

    public void ReceiveAnchor()
    {
        Debug.Log("Connecting to " + serverIP);
        HostName host = new HostName(serverIP);
        networkConnection = new StreamSocket();
        IAsyncAction connectAction = networkConnection.ConnectAsync(host, "11000");
        AsyncActionCompletedHandler connectHandler = new AsyncActionCompletedHandler(recvHandler);
        connectAction.Completed = connectHandler;
    }
    private async void recvHandler(IAsyncAction asyncInfo, AsyncStatus status)
    {
        if (status == AsyncStatus.Completed)
        {
            Debug.Log("connected");
            if(dataBuffer == null)
            {
                using (DataReader reader = new DataReader(networkConnection.InputStream))
                {
                    DataReaderLoadOperation drlo = reader.LoadAsync(4);
                    while (drlo.Status == AsyncStatus.Started)
                    {
                        // just waiting.
                        // Yes... not very async friendly
                    }
                    size = reader.ReadInt32();
                    dataBuffer = new byte[size];
                    await reader.LoadAsync((uint)size);
                    reader.ReadBytes(dataBuffer);
                    //Here we should tell the store that we are ready.
                    dataReadyEvent?.Invoke(dataBuffer);
                }
            }
        }
    }
    */
#endif

}
