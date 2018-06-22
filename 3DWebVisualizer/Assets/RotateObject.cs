using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Range(0.1f, 1)] public float RotationFactor = 1;

	// Use this for initialization
	void Start ()
	{
	    StartCoroutine(RotateObjectOverTime());
	}


    IEnumerator RotateObjectOverTime()
    {

        while (true)
        {
            this.transform.Rotate(Vector3.up, 1 *RotationFactor);
            yield return new WaitForEndOfFrame();
        }
    }
}
