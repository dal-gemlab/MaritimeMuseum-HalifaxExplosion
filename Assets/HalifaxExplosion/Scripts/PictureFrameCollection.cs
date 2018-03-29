using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
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

        foreach(Transform child in GetComponentInChildren<Transform>())
        {
            var frame = Instantiate(PictureFramePrefab);


            frame.transform.position = child.position;
            frame.transform.rotation = child.rotation;
            frame.transform.localScale = new Vector3(PictureSize, PictureSize, PictureSize);
            //frame.transform.LookAt(transform.position);
            //frame.transform.forward *= -1;
            //frame.transform.parent = transform;

            var framePlane = new Plane(frame.transform.forward * -1, frame.transform.position);



            planes.Add(frame.GetInstanceID(), framePlane);
            frames.Add(frame.GetInstanceID(), frame.GetComponent<PictureFrame>());
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
	

}
