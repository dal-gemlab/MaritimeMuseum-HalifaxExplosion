using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class addButtonsRuntime : MonoBehaviour {

    public GameObject verticalLayout;
    public GameObject buttonPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    #if UNITY_ENDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("AddButtons"))
        {
            var b = (GameObject)Instantiate(buttonPrefab);
            b.transform.parent = verticalLayout.transform;
            b.transform.GetChild(0).GetComponent<Text>().text = "Hue";
        }


    }
#endif
}
