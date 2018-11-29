using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPlacer : MonoBehaviour {

    public Vector3 Position;
    public Quaternion Rotation;
    public string LogLine;

    private void Update()
    {

        var values = LogLine.Split(',');
        if(values.Length < 4)
            return;;
        Position = new Vector3(
            float.Parse(values[0]),
            float.Parse(values[1]),
            float.Parse(values[2])
            );

        Rotation = new Quaternion(
            float.Parse(values[3]),
            float.Parse(values[4]),
            float.Parse(values[5]),
            float.Parse(values[6])
            );


        transform.position = Position;
        transform.rotation = Rotation;

        transform.name = values[7];
    }
}
