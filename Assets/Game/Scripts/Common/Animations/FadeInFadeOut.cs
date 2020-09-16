using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;

[CLSCompliant(false)]
public class FadeInFadeOut : MonoBehaviour
{
    public Image uiElement;

    [Tooltip("Color to fade to")]
    [SerializeField]
    private Color EndColor = Color.clear;

    [Tooltip("Color to fade from")]
    [SerializeField]
    private Color StartColor = Color.white;

    public float time = 6f;


    // Start is called before the first frame update
    void OnEnable()
    {
        if (uiElement == null)
        {
            uiElement = gameObject.GetComponent<Image>();
        }
        StartCoroutine(DoAnimate());
    }

    IEnumerator DoAnimate()
    {
        while (true)
        {
            uiElement.color = StartColor;
            uiElement.DOFade(0f, time);

            yield return new WaitForSeconds(6f);

            uiElement.color = EndColor;
            uiElement.DOFade(1f, time);

            yield return new WaitForSeconds(6f);
        }
    }
}
