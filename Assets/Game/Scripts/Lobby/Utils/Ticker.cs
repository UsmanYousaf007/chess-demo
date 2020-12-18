using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.InstantFramework;
using TMPro;

[System.CLSCompliant(false)]
public class Ticker : MonoBehaviour
{
    [Range(1f, 10f)]
    public float itemDuration = 3.0f;

    float width;
    float pixelsPerSecond;
    TickerItem currentItem;
    //public TickerItem[] entries;
    int entryNo = 0;

    bool startTicker = false;
    private List<TickerItem> entries = new List<TickerItem>();

    // Start is called before the first frame update
    void Start()
    {
        width = GetComponent<RectTransform>().rect.width;
        pixelsPerSecond = width / itemDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTicker)
        {
            if (currentItem.GetXPosition <= -currentItem.GetWidth)
            {
                AddTickerItem(entries[entryNo]);
                entryNo++;
                if (entryNo >= 3)
                {
                    entryNo = 0;
                }
            }
        }
    }

    void AddTickerItem(TickerItem item)
    {
        currentItem = item;
        currentItem.Initialize(width, pixelsPerSecond);
    }

    public void StartTicker(List<TickerItem> items)
    {
        entries = items;
        AddTickerItem(entries[entryNo]);
        startTicker = true;
    }
}
