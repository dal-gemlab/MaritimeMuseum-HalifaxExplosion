using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTest : MonoBehaviour
{

    public GameObject cube;
    public GameObject sphere;
    public GameObject root;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
#if UNITY_ENDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("DoSomething"))
        {
            Debug.Log(sphere.transform.InverseTransformPoint(root.transform.position));
            Debug.Log(root.transform.InverseTransformPoint(sphere.transform.position));
            Debug.Log(sphere.transform.localPosition);
            Debug.Log(sphere.transform.position);

        }
    }
#endif
}