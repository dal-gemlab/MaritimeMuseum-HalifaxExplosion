using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantiation : MonoBehaviour {

    public GameObject shipA, shipB, shipC;
    #if UNITY_ENDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Instantiate"))
        {

            var sA = GameObject.Instantiate(shipA);
            var sB = GameObject.Instantiate(shipB);
            var sC = GameObject.Instantiate(shipC);

            sA.transform.GetChild(0).GetComponent<ParticleFlow>().destination = sB.transform;
            sB.transform.GetChild(1).GetComponent<ParticleFlow>().destination = sC.transform;
            sB.transform.GetChild(2).GetComponent<ParticleFlow>().destination = sC.transform;

        }
    }
#endif

}
