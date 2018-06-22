using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncodeTextureTEst : MonoBehaviour {

    public Texture image;

	// Use this for initialization
	void Start () {
        Texture2D tex = (Texture2D)image;
        byte[] texSer = tex.EncodeToPNG();
        Debug.Log(texSer.Length);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
