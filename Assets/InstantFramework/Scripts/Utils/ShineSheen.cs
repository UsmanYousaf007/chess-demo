using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShineSheen : MonoBehaviour
{
    public Transform shine;

    private float offset = 400;

    // Start is called before the first frame update
    void Start()
    {
        Animate();   
    }

    private void Animate()
    {
        shine.DOLocalMoveX(offset, 2.0f).SetEase(Ease.Linear).SetDelay(Random.Range(5.0f, 7.0f)).OnComplete(() =>
            {
                shine.DOLocalMoveX(-offset, 0);
                Animate();
            });
    }
}
