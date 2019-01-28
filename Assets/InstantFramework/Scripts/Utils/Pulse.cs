using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pulse : MonoBehaviour {

    public float size;
    public float scaleTime;
    public int count;
    public float pauseTime;

    RectTransform rect;

    // Use this for initialization
    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        StartPulse();
    }

    void StartPulse()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rect.DOScale(size, scaleTime).SetLoops(count, LoopType.Yoyo));
        sequence.AppendInterval(pauseTime);
        sequence.AppendCallback(StartPulse);
        sequence.PlayForward();
    }
}
