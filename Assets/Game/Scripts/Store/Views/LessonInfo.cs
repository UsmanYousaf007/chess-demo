using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

[System.CLSCompliantAttribute(false)]
public class LessonInfo : MonoBehaviour
{
    public Text title;
    public Text totalLessons;
    public Text totalTime;

    public void Init(string _title, string _lessons, string _time)
    {
        this.title.text = _title;
        this.totalLessons.text = _lessons + " Lessons";
        totalTime.text = _time + "min";
    }
}
