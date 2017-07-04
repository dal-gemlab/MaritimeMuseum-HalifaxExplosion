using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSerialization : MonoBehaviour {

    public List<Transform> transforms;

	// Use this for initialization
	void Start () {

        PositionFileHelper.SaveRelativePositions(transforms, "trans.dat");
        List<Transform> l = PositionFileHelper.GetRelativePositions("trans.dat");

        foreach( Transform t in l)
        {
            Debug.Log(t.position);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
