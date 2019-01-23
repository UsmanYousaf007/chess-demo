using UnityEngine;
using System.Collections;
using DG.Tweening;
using TurboLabz.InstantGame;

namespace TurboLabz.TLUtils
{
    public class BlinkFader : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            Color colorAlpha = Colors.ColorAlpha(sr.color, 0.25f);
            gameObject.GetComponent<SpriteRenderer>().DOColor(colorAlpha, 0.2f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
