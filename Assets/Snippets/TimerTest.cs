using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TurboLabz.TLUtils;
using UnityEngine;

public class TimerTest : MonoBehaviour {

    TimeSpan ticker = TimeSpan.Zero;

	// Use this for initialization
	void Start () {

        TimeSpan t1 = new TimeSpan(2, 0, 0, 0);        // 48:00:00
        TimeSpan t2 = new TimeSpan(3, 23, 59, 59);    // 95:59:59
        TimeSpan t3 = new TimeSpan(1, 23, 0, 0);    // 47:00:00
        TimeSpan t4 = new TimeSpan(1, 22, 59, 59);    // 46:59:59
        TimeSpan t5 = new TimeSpan(1, 22, 59, 0);    // 46:59:00
        TimeSpan t6 = new TimeSpan(1, 0, 0, 59);    // 24:00:59
        TimeSpan t7 = new TimeSpan(1, 0, 0, 0);        // 24:00:00
        TimeSpan t8 = new TimeSpan(23, 59, 59);        // 23:59:59
        TimeSpan t9 = new TimeSpan(1, 1, 0);        // 01:01:00
        TimeSpan t10 = new TimeSpan(1, 0, 1);        // 01:00:01
        TimeSpan t11 = new TimeSpan(1, 0, 0);        // 01:00:00
        TimeSpan t12 = new TimeSpan(0, 59, 59);        // 59:59

        Debug.Log("t1=" + FormatTimer(t1));
        Debug.Log("t2=" + FormatTimer(t2));
        Debug.Log("t3=" + FormatTimer(t3));
        Debug.Log("t4=" + FormatTimer(t4));
        Debug.Log("t5=" + FormatTimer(t5));
        Debug.Log("t6=" + FormatTimer(t6));
        Debug.Log("t7=" + FormatTimer(t7));
        Debug.Log("t8=" + FormatTimer(t8));
        Debug.Log("t9=" + FormatTimer(t9));
        Debug.Log("t10=" + FormatTimer(t10));
        Debug.Log("t11=" + FormatTimer(t11));
        Debug.Log("t12=" + FormatTimer(t12));


        ticker = new TimeSpan( 1, 0, 10);
        StartCoroutine(Tick());
    }

    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            ticker = ticker.Subtract(new TimeSpan(0, 0, 1));

            LogUtil.Log("[TICK] " + FormatTimer(ticker) + " ...............[RAW] " + ticker, "cyan");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private string FormatTimer(TimeSpan timer)
    {
        // TODO: localize clock

        long seconds = timer.Seconds;
        long minutes = timer.Minutes;
        long hours = timer.Hours;

        if (timer.Days > 0)
        {
            hours += timer.Days * 24;
        }

        if (timer.TotalHours >= 1)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        // else show 00:00 format
        // This code is similar to rounding the seconds to a ceiling
        if (timer.TotalMilliseconds > 0)
        {
            timer = TimeSpan.FromMilliseconds(timer.TotalMilliseconds + 999);
        }

        return string.Format("{0:00}:{1:00}", Mathf.FloorToInt((float)timer.TotalMinutes), timer.Seconds);
    }
}
