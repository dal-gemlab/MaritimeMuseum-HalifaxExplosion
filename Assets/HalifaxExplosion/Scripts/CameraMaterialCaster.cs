using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMaterialCaster : MonoBehaviour {

    private RaycastHit hit;

    public Material pictureFrameMaterial;
    public PictureFrameCollection PictureCollection;

	// Use this for initialization
	void Start () {
        this.transform.localPosition = new Vector3(0, 0, Camera.main.nearClipPlane);
	}
	
	// Update is called once per frame
	void LateUpdate () {
        var ray = new Ray(this.transform.position, Camera.main.transform.forward);

        if (!Physics.Raycast(ray, out hit))
            return;

        if (hit.collider.CompareTag("PictureFrame"))
            PictureFrameCollection.Instance.PictureWasStarred(hit.collider.gameObject.GetInstanceID());

        Debug.DrawRay(this.transform.position, Camera.main.transform.forward, Color.red);

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null || meshCollider.convex)
            return;

        var m = meshCollider.sharedMesh;
        var submeshIndex = 0;

        int[] hittedTriangle = new int[]
        {
            m.triangles[hit.triangleIndex * 3],
            m.triangles[hit.triangleIndex * 3 + 1],
            m.triangles[hit.triangleIndex * 3 + 2]
        };

        for (int i = 0; i < m.subMeshCount; i++)
        {
            int[] subMeshTris = m.GetTriangles(i);
            for (int j = 0; j < subMeshTris.Length; j += 3)
            {
                if (subMeshTris[j] == hittedTriangle[0] &&
                    subMeshTris[j + 1] == hittedTriangle[1] &&
                    subMeshTris[j + 2] == hittedTriangle[2])
                {
                 //   Debug.Log(string.Format("triangle index:{0} submesh index:{1} submesh triangle index:{2}", hit.triangleIndex, i, j / 3));
                    submeshIndex = i;
                }
            }
        }

        if(submeshIndex >0)
        {
            Debug.Log(string.Format("Material name: {0}", meshCollider.GetComponentInChildren<MeshRenderer>().materials[submeshIndex].name));
            var image = meshCollider.GetComponentInChildren<MeshRenderer>().materials[submeshIndex].mainTexture;
            PictureCollection.SetTextureToFrame(ray, image);
       }

    }
}
