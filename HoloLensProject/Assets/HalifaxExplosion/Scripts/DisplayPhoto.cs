using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;
using System;


/// <summary>
/// Script that displays a quad containing a photo
///   when the user clicks on the object
/// </summary>
public class DisplayPhoto : MonoBehaviour, IInputClickHandler
{

    public Texture image;
    public Material imageMat;
    GameObject imageQuad;



    public void OnInputClicked(InputClickedEventData eventData)
    {
        //If we are not displaying the photo create one
        if (imageQuad == null)
        {
            imageQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Renderer quadRenderer = imageQuad.GetComponent<Renderer>() as Renderer;
            quadRenderer.material = imageMat;

            imageQuad.transform.parent = this.transform;
            imageQuad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
            imageQuad.transform.localScale = imageQuad.transform.localScale / 2;

            quadRenderer.material.SetTexture("_MainTex", image);


            //This enables the billbord effect
            imageQuad.gameObject.AddComponent<HoloToolkit.Unity.Billboard>();

        }
        //If we have a photo destroy it
        else
        {
            Destroy(imageQuad);
        }
    }
}
