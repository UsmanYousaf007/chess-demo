using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShineSheen : MonoBehaviour
{
    public Transform shine;

    public float randomDelayMin;
    public float randomDelayMax;
    public float offset;
    public float displacementDuration;

    // Start is called before the first frame update
    void Start()
    {
        Animate();   
    }

    private void Animate()
    {
        shine.DOLocalMoveX(offset, displacementDuration).SetEase(Ease.Linear).SetDelay(Random.Range(randomDelayMin, randomDelayMax)).OnComplete(() =>
            {
                shine.DOLocalMoveX(-offset, 0);
                Animate();
            });
    }
}
