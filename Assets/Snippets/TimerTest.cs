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

        TimeSpan t1 = new TimeSpan(2, 0, 0, 0);        // 2d
        TimeSpan t2 = new TimeSpan(3, 23, 59, 59);    // 2d
        TimeSpan t3 = new TimeSpan(1, 23, 0, 0);    // 1d 23h
        TimeSpan t4 = new TimeSpan(1, 22, 59, 59);    // 1d 23h
        TimeSpan t5 = new TimeSpan(1, 0, 59, 59);    // 1d 1h
        TimeSpan t6 = new TimeSpan(1, 0, 0, 59);    // 1d 1h
        TimeSpan t7 = new TimeSpan(1, 0, 0, 0);        // 1d
        TimeSpan t8 = new TimeSpan(23, 59, 59);        // 1d
        TimeSpan t9 = new TimeSpan(1, 1, 0);        // 1h 1m
        TimeSpan t10 = new TimeSpan(1, 0, 1);        // 1h 1m
        TimeSpan t11 = new TimeSpan(1, 0, 0);        // 1h
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

        ticker = new TimeSpan(0, 2, 0, 10);

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

        if (timer.TotalHours > 1)
        {
            int days = timer.Days;
            int hours = timer.Hours;
            int minutes = timer.Minutes;

            // Day calculation
            if (timer.Hours == 23 && (timer.Minutes > 0 || timer.Seconds > 0))
            {
                days++;
                hours = 0;
                minutes = 0;
            }
            // Hours calculation
            else if (timer.Minutes == 59 && timer.Seconds > 0)
            {
                hours++;
                minutes = 0;
            }
            else if (days > 0 && (timer.Minutes > 0 || timer.Seconds > 0))
            {
                hours++;
                minutes = 0;
            }
            // Minutes calculation
            else if (timer.Seconds > 0)
            {
                minutes++;
            }

            StringBuilder sb = new StringBuilder();
            if (days > 0)
            {
                sb.Append(days);
                sb.Append("d");
            }

            if (hours > 0)
            {
                if (days > 0)
                {
                    sb.Append(" ");

                }
                sb.Append(hours);
                sb.Append("h");
            }

            if (minutes > 0 && (days == 0))
            {
                if (hours > 0)
                {
                    sb.Append(" ");

                }
                sb.Append(minutes);
                sb.Append("m");
            }

            return sb.ToString();
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
