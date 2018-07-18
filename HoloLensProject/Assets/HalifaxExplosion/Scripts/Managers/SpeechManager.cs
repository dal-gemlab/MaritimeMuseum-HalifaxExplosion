using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SpeechManager : MonoBehaviour {

    public Material buildingMat;
    public Material defaultBuildingColor;

    private bool isAdmin = false;
    private Color adminColor;

    public void Start()
    {
        buildingMat.color = defaultBuildingColor.color;
        adminColor = new Color(232f/255f, 88f/255f, 1);

    }

    public void ResetScene()
    {
        if (!isAdmin)
            return;
        if(WorldAnchorManager.Instance.AnchorStore != null)
            WorldAnchorManager.Instance.AnchorStore.Clear();
        SceneManager.LoadScene(0);
    }

    public void PrintTransforms()
    {

        var anchor = GameObject.Find("Anchor");
        var childCount = anchor.transform.childCount;
        string s = "";
        //foreach(Transform g in anchor.GetComponentsInChildren<Transform>())
        for(int i = 0; i<childCount; i++)
        {
            //var t = g; ;
            var t = anchor.transform.GetChild(i);
            s += System.String.Format("Name: {0} \nx: {1} y: {2} z: {3}\n", t.gameObject.name, t.localPosition.x, t.localPosition.y, t.localPosition.z );
            s += System.String.Format("rx: {0} ry: {1}, rz: {2}\n", t.localRotation.eulerAngles.x, t.localRotation.eulerAngles.y, t.localRotation.eulerAngles.z);
            //Debug.LogFormat("Name: {3} \nx: {0} y: {1} z: {2}", t.localPosition.x, t.localPosition.y, t.localPosition.z, t.gameObject.name);


        }
        Debug.Log(s);
    }

    public void AddDrag()
    {
        if (!isAdmin)
            return;
        StateManager.Instance.AddManipulationCapability();
        StateManager.Instance.manipulationMethod = StateManager.ManipulationMethod.Translate;
    }

    public void AddRotate()
    {
        if (!isAdmin)
            return;
        StateManager.Instance.AddManipulationCapability();
        StateManager.Instance.manipulationMethod = StateManager.ManipulationMethod.Rotate;
    }

    public void SwitchAdmin()
    {
        isAdmin = !isAdmin;
        if (isAdmin)
            buildingMat.color = adminColor;
        else
            buildingMat.color = defaultBuildingColor.color;

        
    }

}
