using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyDistance : MonoBehaviour {

    public Material colorMat;
    public Transform player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var distance = Vector3.Distance(player.position, this.transform.position);
        Debug.Log(distance);
        
        if (distance < 0.2f)
        {
            colorMat.color = new Color(1, 1, 1, 0);
        }
        else if ( distance < 0.5f)
        {

            var alpha = ((distance - 0.2f) * 3.333f);
            colorMat.color = new Color(1, 1, 1, alpha);

        }
	}
}
