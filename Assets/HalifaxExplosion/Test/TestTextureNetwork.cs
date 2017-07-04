using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestTextureNetwork : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void testSend()
    {
        HoloCapture holoCap = new HoloCapture();
        holoCap.TakePicture();
    }

#if UNITY_ENDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Finish Placement"))
        {
            this.testSend();
        }
    }
#endif
}