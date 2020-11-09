using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

[System.CLSCompliantAttribute(false)]
public class SubscriptionOffer : MonoBehaviour
{
    public Text text;
    public Image icon;

    public void Init(Sprite icon, string text)
    {
        this.text.text = text;
        this.icon.sprite = icon;
        this.icon.SetNativeSize();
    }
}
