using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PictureFrame : MonoBehaviour {

    [Range(1, 10)]
    public int TimeBeforeFading = 5;
    [Range(0, 1)]
    public float FadeAlphaCutout = 0.2f;

    Coroutine fadeCorroutine;


    private MeshRenderer render;

	// Use this for initialization
	void Start () {
        render = GetComponent<MeshRenderer>();
	    fadeCorroutine = StartCoroutine(Fade());
    }

    public void SetImage(Texture tex)
    {
        render.material.mainTexture = tex;
        render.material.SetFloat("_Alpha", 1);
        if(fadeCorroutine == null)
            fadeCorroutine = StartCoroutine(Fade());
        Debug.Log("Frame # " + gameObject.name + " was updated");
    }

    public void StarredAt()
    {
        if (render.material.GetFloat("_Alpha") != 0)
        {
            render.material.SetFloat("_Alpha", 1);
        }
    }

    public string GetImageName()
    {
        if (render.material.mainTexture != null && render.material.GetFloat("_Alpha") != 0)
            return render.material.mainTexture.ToString();
        return "NoImage";
    }

    IEnumerator Fade()
    {
        
        while (render.material.GetFloat("_Alpha") > FadeAlphaCutout)
        {
            render.material.SetFloat("_Alpha", render.material.GetFloat("_Alpha") - 0.01f);

            yield return new WaitForSeconds(0.1f);
        }

        render.material.SetFloat("_Alpha", 0);
        fadeCorroutine = null;
    }

    public void StopFade()
    {
        if(fadeCorroutine != null)
            StopCoroutine(fadeCorroutine);
        render.material.SetFloat("_Alpha", 1);

    }
	
}