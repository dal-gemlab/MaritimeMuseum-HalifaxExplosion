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

        string s = "";
        foreach(Transform g in anchor.GetComponentsInChildren<Transform>())
        {
            var t = g; ;
            s += System.String.Format("Name: {3} \nx: {0} y: {1} z: {2}\n", t.localPosition.x, t.localPosition.y, t.localPosition.z, t.gameObject.name);
            //Debug.LogFormat("Name: {3} \nx: {0} y: {1} z: {2}", t.localPosition.x, t.localPosition.y, t.localPosition.z, t.gameObject.name);


        }
        Debug.Log(s);
    }

    public void addDrag()
    {
        StateManager.Instance.AddDragableCapability();
    }
}
