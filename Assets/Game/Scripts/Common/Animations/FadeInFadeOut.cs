using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour
{
    public Image uiObj;

    [Tooltip("Color to fade to")]
    [SerializeField]
    private Color EndColor = Color.white;

    [Tooltip("Color to fade from")]
    [SerializeField]
    private Color StartColor = Color.clear;

    public float time = 6f;


    // Start is called before the first frame update
    void OnEnable()
    {
        FadeIn();
    }

    public void updateColor(float val)
    {
        uiObj.color = ((1f - val) * StartColor) + (val * EndColor);
    }

    public void FadeIn()
    {
        iTween.ValueTo(this.gameObject, iTween.Hash("from", 1f, "to", 0f, "delay", 0f, "time", time, "onupdate", "updateColor", "oncomplete", "FadeOut"));
    }

    public void FadeOut()
    {
        iTween.ValueTo(this.gameObject, iTween.Hash("from", 0f, "to", 1f, "delay", 0f, "time", time, "onupdate", "updateColor", "oncomplete", "FadeIn"));
    }
}
