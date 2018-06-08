using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUThink : MonoBehaviour {

    public GameObject[] thinkDots;
    public float dotWaitSeconds;
    private Coroutine anim;

    private const float MIN_WAIT = 0.1f;

    void OnEnable()
    {
        DeactivateAll();
        anim = StartCoroutine(Animate());
    }

    void OnDisable()
    {
        StopCoroutine(anim);
        anim = null;
    }

    IEnumerator Animate()
    {
        float waitTime = Mathf.Max(MIN_WAIT, dotWaitSeconds);

        while (true)
        {
            for (int i = 0; i < thinkDots.Length; i++)
            {
                thinkDots[i].SetActive(true);
                yield return new WaitForSeconds(waitTime);
            }

            DeactivateAll();
            yield return new WaitForSeconds(waitTime);
        }
    }

    void DeactivateAll()
    {
        foreach (GameObject thinkDot in thinkDots)
        {
            thinkDot.SetActive(false);
        }

    }
}
