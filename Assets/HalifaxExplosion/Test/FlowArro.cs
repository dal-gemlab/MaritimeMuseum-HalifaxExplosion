using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowArro : MonoBehaviour {

    public Transform Origin;
    public Transform Target;

    private Vector3 ArrowOrigin;
    private Vector3 ArrowTarget;
    private LineRenderer cachedLineRenderer;
    void Start()
    {
        ArrowOrigin = Origin.position;
        ArrowTarget = Target.position;

        cachedLineRenderer = this.GetComponent<LineRenderer>();
        cachedLineRenderer.widthCurve = new AnimationCurve(
            new Keyframe(0, 0.4f)
            , new Keyframe(0.9f, 0.4f) // neck of arrow
            , new Keyframe(0.91f, 1f)  // max width of arrow head
            , new Keyframe(1, 0f));  // tip of arrow
        cachedLineRenderer.SetPositions(new Vector3[] {
             ArrowOrigin
             , Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.9f)
             , Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.91f)
             , ArrowTarget });
    }

    // Update is called once per frame
    void Update () {
		
	}
}
