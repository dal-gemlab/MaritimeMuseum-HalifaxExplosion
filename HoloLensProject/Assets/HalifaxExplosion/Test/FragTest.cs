using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        


    }
#if UNITY_ENDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Send byte[]"))
        {
            int size = 2 * 4;// 63 * 1024 * 4;
            byte[] testPackage = new byte[size];

            int data = 0;
            for (int i = 0; i < size; i++)
            {
                testPackage[i] = (byte)data;
                data++;
                if (data > 255)
                    data = 0;
            }

            GameObject.Find("ServerManager").GetComponent<ServerManager>().sendByteArr(testPackage);
        }

    }
#endif

}
