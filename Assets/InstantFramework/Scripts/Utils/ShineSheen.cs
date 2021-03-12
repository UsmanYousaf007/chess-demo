using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.CLSCompliant(false)]
public class ShineSheen : MonoBehaviour
{
    public Transform shine;

    public float randomDelayMin;
    public float randomDelayMax;
    public float offset;
    public float displacementDuration;

    // Start is called before the first frame update
    void OnEnable()
    {
        Animate();   
    }

    void OnDisable()
    {;
        shine.localPosition = new Vector3(-offset, shine.localPosition.y, shine.localPosition.z);
        shine.DOKill();
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
