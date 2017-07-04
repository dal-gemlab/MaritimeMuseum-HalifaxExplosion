using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using Polenter.Serialization;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Storage;
using System.Threading.Tasks;
#endif

/// <summary>
/// This needs REWORK!
/// Utilty class that handles file I/O
/// Note that UWP and .Net have different methods that do not play nice together!
/// </summary>
public static class PositionFileHelper
{
    public static List<Transform> GetRelativePositions(string filename)
    {
        return null;
        /*Stream stream = null;
#if WINDOWS_UWP

        Task fileTask = new Task(
            async () =>
            {
                StorageFolder sF = ApplicationData.Current.LocalFolder;
                StorageFile posFile = await sF.GetFileAsync(filename);
                var acessStream = await posFile.OpenReadAsync();
                stream = acessStream.AsStreamForRead();
            });
        fileTask.Start();
        fileTask.Wait();
#else
        try
        {
            stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

        }
        catch (FileNotFoundException e)
        {
            Debug.LogError("File not found!");
            return null;

        }
#endif

        SharpSerializer serializer = new SharpSerializer(true);
        
        

        List<storeObject> objs = (List<storeObject>)serializer.Deserialize(stream);
#if UNITY_WP8 || UNITY_WP8_1 || UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0
        stream.Dispose();
#else
        stream.Close();
#endif
        List<Transform> storedTransforms = new List<Transform>();
        foreach (storeObject obj in objs)
        {
            GameObject g = new GameObject();
            g.transform.position = new Vector3(obj.position[0], obj.position[1], obj.position[2]);
            g.transform.rotation = new Quaternion(obj.rotation[0], obj.rotation[1], obj.rotation[2], obj.rotation[3]);
            storedTransforms.Add(g.transform);
            UnityEngine.MonoBehaviour.Destroy(g);

        }

        return storedTransforms;
        */
    }

    public static bool SaveRelativePositions(List<Transform> transforms, string filename)
    {
        return true;
        /*List<storeObject> serList = new List<storeObject>();
        foreach (Transform t in transforms)
        {
            float[] p = new float[3] { t.position.x, t.position.y, t.position.z };
            float[] r = new float[4] { t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w };

            storeObject obj = new storeObject()
            {
                name = t.gameObject.name,
                position = p,
                rotation = r
            };
            serList.Add(obj);
        }


#if WINDOWS_UWP
        Stream stream = null;
        Task fileTask = new Task(
            async () =>
            {
                StorageFolder sF = ApplicationData.Current.LocalFolder;
                StorageFile posFile = await sF.CreateFileAsync(filename);
                stream = await posFile.OpenStreamForWriteAsync();
            });
        fileTask.Start();
        fileTask.Wait();

#else
        Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
#endif
        SharpSerializer serializer = new SharpSerializer(true);
        Debug.Log(serList[0].name);
        serializer.Serialize(serList, stream);
#if UNITY_WP8 || UNITY_WP8_1 || UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || WINDOWS_UWP
        stream.Dispose();
#else
        stream.Close();
#endif
        return true;*/
    }


    private class storeObject
    {
        public string name { get; set; }
        public float[] position { get; set; }
        public float[] rotation { get; set; }

    }
}