using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAndRotateCamera : MonoBehaviour
{

    public Vector3 Position;
    public Quaternion Rotation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position = Position;
	    transform.rotation = Rotation;
	}
}
