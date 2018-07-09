using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using UnityEngine.VR.WSA.WebCam;
using System.Linq;

public class HoloCapture  {

    private PhotoCapture photoCaptureObject;

    public HoloCapture()
    {
        photoCaptureObject = null;
    }

    public void TakePicture()
    {
        PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);
    }
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 1.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;
        
        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            //targetTexture.Resize(targetTexture.width / 2, targetTexture.height / 2);
            
            // Do as we wish with the texture such as apply it to a material, etc.
            //ServerManager.Instance.sendText(targetTexture);
        }
        // Clean up
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

#if !UNITY_EDITOR
   /* private async void sendImageOverNetwork(byte[] imagePNG)
    {

        string serverAddr = UnityEngine.Networking.NetworkManager.singleton.networkAddress;
        serverAddr = serverAddr.Trim();
        serverAddr = serverAddr.Substring(serverAddr.LastIndexOf(':') + 1);
        StreamWebSocket webSock;
        webSock = new StreamWebSocket();
        Uri serverUri = new Uri("ws://"+serverAddr+":8888/ws");
        await webSock.ConnectAsync(serverUri);

        DataWriter messageWrite = new DataWriter(webSock.OutputStream);
        messageWrite.WriteBytes(imagePNG);
        await messageWrite.StoreAsync();

        webSock.Close(1000, "Closed due to user request.");
    }*/
#endif


}
