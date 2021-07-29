using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.CLSCompliant(false)]
public class TickerItem : MonoBehaviour
{
    float tickerWidth;
    float pixelsPerSecond;
    RectTransform rt;
    public string id;
    public TMP_Text name;
    public TMP_Text place;
    public RectTransform defaultPos;
    bool initialized = false;

    public float GetXPosition { get { return rt.anchoredPosition.x; } }
    public float GetWidth { get { return rt.rect.width; } }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            rt.position += Vector3.left * pixelsPerSecond * Time.deltaTime;
            if (GetXPosition <= 0 - tickerWidth - GetWidth)
            {
                gameObject.SetActive(false);
                GetComponent<RectTransform>().localPosition = defaultPos.localPosition;
                GetComponent<RectTransform>().anchoredPosition = defaultPos.anchoredPosition;
                initialized = false;
            }
        }
    }

    public void Initialize(float tickerWidth, float pixelsPerSecond)
    {
        this.tickerWidth = tickerWidth;
        this.pixelsPerSecond = pixelsPerSecond;
        rt = GetComponent<RectTransform>();
        gameObject.SetActive(true);
        initialized = true;
    }

    public void Populate()
    {

    }
}
