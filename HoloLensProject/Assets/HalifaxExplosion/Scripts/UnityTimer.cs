using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Juliano Franz
 * 
 * @date 2018/03/06
 */

/// <summary>
/// Timer class that mimics System.Timers.Timer interface
/// Uses only Unity methods and time is controled via Coroutine
/// Safe for Desktop,Mobile,UWP
/// Timers that are not running do not use any CPU/GPU/Frame cycles.
/// </summary>
public class UnityTimer : MonoBehaviour {

    public bool AutoReset { get; set; }
    
    public delegate void TimerElapsed();
    public event TimerElapsed Elapsed;

    private bool running;
    private int interval;
    private float timeLeft;
    private Coroutine timerCoroutine;

    // Use this for initialization

    private void Awake()
    {
        AutoReset = true;
        running = false;
    }

    /// <summary>
    /// Defines the timer interval in secconds
    /// </summary>
    /// <param name="interval">Timer interval in seconds</param>
    public void SetInterval(int interval)
    {
        this.interval = interval;
        timeLeft = interval;
    }

    /// <summary>
    /// Starts the timer. If the timer is already running resets
    /// </summary>
    public void TimerStart()
    {
        timeLeft = interval;
        running = true;
        if (timerCoroutine == null)
            timerCoroutine = StartCoroutine(TimerCycle());
        
    }

    /// <summary>
    /// Stop the timer and clear the Coroutine
    /// </summary>
    public void TimerStop()
    {
        if (timerCoroutine == null)
            return;
        running = false;
    }


    private IEnumerator TimerCycle()
    {
        while(running)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0)
            {
                if (Elapsed != null)
                    Elapsed.Invoke();
                if (AutoReset)
                    timeLeft = interval;
                else
                    running = false;
            }
            yield return new WaitForEndOfFrame();
        }
        timerCoroutine = null;
        yield return 0;
    }

}
