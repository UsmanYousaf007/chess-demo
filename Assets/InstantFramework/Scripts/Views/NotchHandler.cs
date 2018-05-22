using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;

public class NotchHandler : MonoBehaviour {

    public RectTransform[] scaleDownSet;
    public RectTransform[] stretchWidth;
    public RectTransform[] gameTopStrip;

    public GameObject notchOverlay;

    const float SCALE_DOWN = 0.92f;
    const float GAME_TOP_STRIP_ADJUST = -58f;

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

        // Make the adjustments
        foreach (RectTransform tfm in scaleDownSet)
        {
            tfm.localScale *= SCALE_DOWN;
        }

        foreach (RectTransform tfm in stretchWidth)
        {
            Vector3 localScale = tfm.localScale;
            localScale.y *= SCALE_DOWN;
            localScale.x *= (1/SCALE_DOWN);
            tfm.localScale = localScale;
        }

        foreach (RectTransform tfm in gameTopStrip)
        {
            Vector3 localPos = tfm.localPosition;
            localPos.y += GAME_TOP_STRIP_ADJUST;
            tfm.localPosition = localPos;
        }
    }
}
