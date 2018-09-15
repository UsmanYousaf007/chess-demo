using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;

public class NotchHandler : MonoBehaviour {

    [Header("Navs")]
    public RectTransform[] topNavs;
    public RectTransform[] botNavs;

    [Header("Lobby")]
    public RectTransform facebookButton;
    public RectTransform centerContent;
    public RectTransform freeBucksButton;
    public RectTransform devFen;

    [Header("Editor")]
    public GameObject notchOverlay;

    void Awake()
    {
        notchOverlay.SetActive(false);

        #if UNITY_EDITOR
        // Detect iphoneX emulation in editor
        if (!(Screen.width == 1125 && Screen.height == 2436)) return;
        notchOverlay.SetActive(true);
        #else
        // Detect notch presence
        if (!(Screen.safeArea.height < Screen.height)) return;
        #endif

        // Do all the notchy adjustments
        // TOP NAVS
        foreach (RectTransform tfm in topNavs)
        {
            SetY(tfm, -131f);
        }
        foreach (RectTransform tfm in botNavs)
        {
            SetY(tfm, 166f);
        }

        // LOBBY
        SetY(facebookButton, -270f);
        SetY(centerContent, -66f);
        SetY(freeBucksButton, 427f);
        SetY(devFen, -448);

    }

    void SetY(RectTransform tfm, float y)
    {
        Vector2 anchoredPos = tfm.anchoredPosition;
        anchoredPos.y = y;
        tfm.anchoredPosition = anchoredPos;
    }

    void SetX(RectTransform tfm, float x)
    {
        Vector3 localPos = tfm.localPosition;
        localPos.x = x;
        tfm.localPosition = localPos;
    }

}
