using System;

using System.Collections;
using System.Collections.Generic;
using MIMIR.Util;
using UnityEngine;

public class PictureFrameCollection : Singleton<PictureFrameCollection> {
    
    [Range(0.1f,1f )]
    public float PictureSize;
    [Range(0, 10)]
    public float Radius;

    public GameObject PictureFramePrefab;

    private Dictionary<int,PictureFrame> frames;
    private Dictionary<int, Plane> planes;

    // Use this for initialization
    void Start () {
        frames = new Dictionary<int, PictureFrame>();
        planes = new Dictionary<int, Plane>();
        int frameNumber = 0;

        foreach(Transform child in GetComponentInChildren<Transform>())
        {
            var frame = Instantiate(PictureFramePrefab);
            frame.name += frameNumber;
            frameNumber++;

            frame.transform.position = child.position;
            frame.transform.rotation = child.rotation;
            frame.transform.localScale = new Vector3(PictureSize, PictureSize, PictureSize);
            //frame.transform.LookAt(transform.position);
            //frame.transform.forward *= -1;
            //frame.transform.parent = transform;

            var framePlane = new Plane(frame.transform.forward * -1, frame.transform.position);



            planes.Add(frame.GetInstanceID(), framePlane);
            frames.Add(frame.GetInstanceID(), frame.GetComponent<PictureFrame>());


            Destroy(child.gameObject);
        }

        GameObject[] framesGOs = GameObject.FindGameObjectsWithTag("PictureFrame");
        foreach (var frameGO in framesGOs)
        {
            frameGO.transform.parent = this.transform;
        }




    }

    //void Update()
    //{
    //    foreach (var pictureFrame in frames)
    //    {
    //        DrawPlane(planes[pictureFrame.Key], pictureFrame.Value.transform);
    //    }
    //}

    public void AdjustPlanePositions()
    {
        foreach (var pictureFrame in frames)
        {
            planes[pictureFrame.Key] =
                new Plane(pictureFrame.Value.transform.forward * -1, pictureFrame.Value.transform.position);
        }
    }

    public void PictureWasStarred(int frameID)
    {
        frames[frameID].StarredAt();
    }

    public void SetTextureToFrame(Ray ray, Texture tex)
    {
        int selectedFrameID = 0;
        float selectedPlaneDistance = 99999;

        foreach (var plane in planes)
        {
            float distance;
            if(plane.Value.Raycast(ray,out distance))
            {
                if(distance>0 && distance < selectedPlaneDistance)
                {
                    selectedPlaneDistance = distance;
                    selectedFrameID = plane.Key;
                }
            }

        }
       
        if (selectedFrameID != 0)
            frames[selectedFrameID].SetImage(tex);
    }


    public void DrawPlane(Plane plane, Transform frame)
    {

        Vector3 v3;
        var position = frame.position;
        var normal = plane.normal;

        if (plane.normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;

        


        var corner0 = (position + v3);
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }
    
}
