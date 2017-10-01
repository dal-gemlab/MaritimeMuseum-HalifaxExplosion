using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SpeechManager : MonoBehaviour {


    public void ResetScene()
    {
        WorldAnchorManager.Instance.AnchorStore.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    public void addDrag()
    {
        StateManager.Instance.AddManipulationCapability();
        StateManager.Instance.manipulationMethod = StateManager.ManipulationMethod.Translate;
    }

    public void addRotate()
    {
        StateManager.Instance.AddManipulationCapability();
        StateManager.Instance.manipulationMethod = StateManager.ManipulationMethod.Rotate;
    }

}
