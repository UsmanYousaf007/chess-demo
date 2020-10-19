using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

[CLSCompliant(false)]
public class MoveFadeAnim : MonoBehaviour
{
    public TextMeshProUGUI uiElement;
    public Transform startPivot;
    public Transform endPivot;

    [Tooltip("Color to fade to")]
    [SerializeField]
    private Color EndColor = Color.white;

    [Tooltip("Color to fade from")]
    [SerializeField]
    private Color StartColor = Color.clear;

    // Start is called before the first frame update
    void OnEnable()
    {
        uiElement.color = EndColor;
        uiElement.gameObject.transform.position = startPivot.position;
        StartCoroutine(DoAnimate());
    }

    IEnumerator DoAnimate()
    {
        yield return new WaitForSeconds(0.5f);

        uiElement.DOFade(0f, 4.5f);
        uiElement.transform.DOMoveY(endPivot.position.y, 4.5f);

        yield return new WaitForSeconds(6.2f);

        gameObject.SetActive(false);
    }

}
