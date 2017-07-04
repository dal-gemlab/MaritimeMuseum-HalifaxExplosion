using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExapandAll : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    #if UNITY_ENDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Expand"))
        {

            foreach (Transform c in GetComponentInChildren<Transform>())
            {
                c.gameObject.GetComponent<FlowLine>().AnimateExpansion();
            }
        }
        if (GUILayout.Button("Retract"))
        {
            foreach (Transform c in GetComponentInChildren<Transform>())
            {
                c.gameObject.GetComponent<FlowLine>().AnimateRetraction();
            }
        }

    }
#endif
}
