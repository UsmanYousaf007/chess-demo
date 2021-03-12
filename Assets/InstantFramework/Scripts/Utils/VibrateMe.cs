/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using DG.Tweening;

public class VibrateMe : MonoBehaviour {

    public float startDelay;
    public float upSpeedTime;
    public float downSpeedTime;
    public Vector3 displacement;

    private Vector3 UpLimits;
    private Vector3 LoLimits;
    private bool started = false;
    private bool sequenced = false;

    void Awake()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(startDelay);
        sequence.AppendCallback(() => FetchPosition());
        sequence.PlayForward();
    }

    void FetchPosition()
    {
        UpLimits = transform.localPosition + displacement;
        LoLimits = transform.localPosition - displacement;
        started = true;
    }

    void Update () 
    {
        if (started == true & sequenced == false)
        {
            sequenced = true;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMove(UpLimits, upSpeedTime));
            sequence.Append(transform.DOLocalMove(LoLimits, downSpeedTime));
            sequence.AppendCallback(() => sequenced = false);
            sequence.PlayForward();
        }
    }
}
