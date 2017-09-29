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

    public BuildingJS()
    {

    }

    public BuildingJS(string modelName, string buildingName, string description)
    {
        modelJSName = modelName;
        this.buildingName = buildingName;
        this.description = description;
    }
        
}
