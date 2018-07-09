using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that limits the amount of the that the Spatial Observer runs
/// </summary>
public class SpatialMappingLimiter : MonoBehaviour {

    [Tooltip("When checked, the SurfaceObserver will stop running after a specified amount of time.")]
    public bool limitScanningByTime = true;

    [Tooltip("How much time (in seconds) that the SurfaceObserver will run after being started; used when 'Limit Scanning By Time' is checked.")]
    public float scanTime = 30.0f;

    //Event that inform subscribers that the mapping is done
    public delegate void OnFinishedMapping();
    public event OnFinishedMapping finishedMappingEvent;
	
	// Update is called once per frame
	void Update () {
        if(limitScanningByTime)
        {
            if (limitScanningByTime && ((Time.time - HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance.StartTime) < scanTime))
            {
                //Do Nothing
            }
            else
            {
                HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance.StopObserver();
                //Raise the event
                //VS will insist that this can be simplified but unity does not implement
                //  C# 6.0 yet bummer
                if (finishedMappingEvent != null)
                    finishedMappingEvent();
                //TODO: Remove this line and destroy the observer
                limitScanningByTime = false;
                //Destroy(this.gameobject);
            }
        }
		
	}
}
