using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPlacer : MonoBehaviour
{

    public List<string> VeithList;
    public Color VeithColor;
    public List<string> PowerPlantList;
    public Color PowerPlantColor;
    public List<string> BellList;
    public Color BellColor;

    public GameObject CameraLaser;

    private void Start()
    {
        foreach (var line in VeithList)
            InstantiateLaser(line,VeithColor);
        foreach (var line in PowerPlantList)
            InstantiateLaser(line, PowerPlantColor);
        foreach (var line in BellList)
            InstantiateLaser(line, BellColor);

    }

    private void InstantiateLaser(string line, Color color)
    {

        var laser = GameObject.Instantiate(CameraLaser);
        laser.transform.GetChild(0).GetComponent<Renderer>().material.color = color;

        var values = line.Split(',');
        if (values.Length < 4)
            return; ;
        var Position = new Vector3(
            float.Parse(values[1]),
            float.Parse(values[2]),
            float.Parse(values[3])
        );

        var Rotation = new Quaternion(
            float.Parse(values[4]),
            float.Parse(values[5]),
            float.Parse(values[6]),
            float.Parse(values[7])
        );


        laser.transform.position = Position;
        laser.transform.rotation = Rotation;

        laser.transform.name = values[8];
    }


}
