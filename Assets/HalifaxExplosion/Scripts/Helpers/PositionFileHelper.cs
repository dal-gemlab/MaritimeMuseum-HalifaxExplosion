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
        List<Transform> storedTransforms = new List<Transform>();
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();

        TextAsset posFile = (TextAsset)Resources.Load(filename, typeof(TextAsset));
        if(posFile == null)
        {
            return null;
        }
        string json = posFile.text;
        List<storeObject> objs = new List<storeObject>();
        foreach(var line in json.Split('\n'))
        {
            //Here could be >1...
            if(line.Length > 3)
                objs.Add(JsonUtility.FromJson<storeObject>(line.ToString()));
        }


        
        foreach (storeObject obj in objs)
        {
            GameObject g = new GameObject();
            g.transform.name = obj.name;
            g.transform.localPosition = new Vector3(obj.position[0], obj.position[1], obj.position[2]);
            g.transform.localRotation = new Quaternion(obj.rotation[0], obj.rotation[1], obj.rotation[2], obj.rotation[3]);
            storedTransforms.Add(g.transform);
            UnityEngine.MonoBehaviour.Destroy(g);

        }
#else

        var objs = loadFileHolo(filename).Result;
        if(objs == null)
        {
            return null;
        }
        foreach (storeObject obj in objs)
        {
            GameObject g = new GameObject();
            g.transform.name = obj.name;
            g.transform.localPosition = new Vector3(obj.position[0], obj.position[1], obj.position[2]);
            g.transform.localRotation = new Quaternion(obj.rotation[0], obj.rotation[1], obj.rotation[2], obj.rotation[3]);
            storedTransforms.Add(g.transform);
            UnityEngine.MonoBehaviour.Destroy(g);

        }
#endif

        return storedTransforms;
        
    }

#if WINDOWS_UWP
    public static async Task<List<storeObject>> loadFileHolo(string filename)
    {
        Stream stream = null;
        List<storeObject> objs = new List<storeObject>();
        StorageFolder sF = ApplicationData.Current.LocalFolder;
        StorageFile posFile;
        try
        {
            posFile = await sF.GetFileAsync(filename);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return null;
        }
        Debug.Log(sF.Path + "  " + sF.Name);
        stream = await posFile.OpenStreamForWriteAsync();
        if (stream == null)
            return null;
        StreamReader s = new StreamReader(stream);
        string json = await s.ReadToEndAsync();

        foreach (var line in json.Split('\n'))
        {
            //Here could be >1...
            if (line.Length > 3)
                objs.Add(JsonUtility.FromJson<storeObject>(line.ToString()));
        }


        stream.Dispose();
        return objs;
    }
#endif

    public static bool SaveRelativePositions(List<Transform> transforms, string filename)
    {
        
        List<storeObject> serList = new List<storeObject>();
        foreach (Transform t in transforms)
        {
            float[] p = new float[3] { t.localPosition.x, t.localPosition.y, t.localPosition.z };
            float[] r = new float[4] { t.localRotation.x, t.localRotation.y, t.localRotation.z, t.localRotation.w };

            storeObject obj = new storeObject()
            {
                name = t.gameObject.name,
                position = p,
                rotation = r
            };
            serList.Add(obj);
        }

        
        string jsonTransforms = "";
        foreach (var obj in serList)
            jsonTransforms += JsonUtility.ToJson(obj) + '\n';
#if UNITY_EDITOR
        File.WriteAllText(Application.dataPath + "/Resources/" + filename + ".txt", jsonTransforms);
#else
        Stream stream = null;
        Task fileTask = new Task(
            async () =>
            {
                StorageFolder sF = ApplicationData.Current.LocalFolder;
                StorageFile posFile = await sF.CreateFileAsync(filename,CreationCollisionOption.ReplaceExisting);
                Debug.Log(sF.Path + "  " + sF.Name);
                stream = await posFile.OpenStreamForWriteAsync();
                StreamWriter s = new StreamWriter(stream);
                await s.WriteAsync(jsonTransforms.ToCharArray());
                s.Flush();
                stream.Dispose();
            });
        fileTask.Start();
        fileTask.Wait();

        
#endif


        return true;
    }


    [Serializable]
    public class storeObject
    {
        public string name;
        public float[] position;
        public float[] rotation;

    }
}