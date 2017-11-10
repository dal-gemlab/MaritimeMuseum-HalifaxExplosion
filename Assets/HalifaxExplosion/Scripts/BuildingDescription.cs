using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDescription : MonoBehaviour {

    public string modelJSName;
    public string buildingName;
    [TextArea(3, 10)]
    public string buildingDescription;

}

public class BuildingJS
{
    public string modelJSName;
    public string buildingName;
    public string description;
    public bool isTracking;
    public float[] pos;
    public float[] quat;

    public BuildingJS()
    {
        isTracking = false;
    }

    public BuildingJS(string modelName, string buildingName, string description) : this()
    {
        modelJSName = modelName;
        this.buildingName = buildingName;
        this.description = description;
    }

    public void SetPosRot(float[] pos, float[] quat)
    {
        this.pos = new float[3];
        this.quat = new float[4];
        Array.Copy(pos, this.pos, 3);
        Array.Copy(quat, this.quat, 4);
        isTracking = true;
    }

}
