using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

public class NotchHandler : MonoBehaviour {

    [Header("Navs")]
    public RectTransform[] topNavs;
    public RectTransform[] botNavs;

    [Header("Lobby")]
    public RectTransform lobbyScrollView;
    public RectTransform lobbyViewPort;
    public RectTransform lobbyScrollViewPivotForBanner;
    public LobbyView lobbyView;
    public RectTransform lobbyScrollViewTopShadow;
    public RectTransform quickMatchButton;

    [Header("Profile")]
    public RectTransform profileFacebookButton;
    public RectTransform profileCenterContent;

    [Header("Friends")]
    public RectTransform friendsScrollView;
    public RectTransform friendsViewPort;
    public RectTransform friendsScrollViewTopShadow;

    [Header("CPU Game")]
    public RectTransform cpuTopBar;
    public RectTransform cpuBotBar;
    public RectTransform[] cpuLeft;
    public RectTransform[] cpuRight;

    [Header("Multiplayer Game")]
    public RectTransform mpTopBar;
    public RectTransform mpBotBar;
    public RectTransform[] mpLeft;
    public RectTransform[] mpRight;

    [Header("Chat New")]
    public RectTransform chatNewTopBar;
    public RectTransform chatNewBotBar;
    public RectTransform chatNewScrollView;
    public RectTransform chatNewViewPort;

    [Header("Notifications")]
    public RectTransform dummyPosition;

    [Header("Editor")]
    public GameObject notchOverlay;

    [Header("Reconnecting")]
    public RectTransform reconnectingPopup;
    public RectTransform maintenanceWarningPopup;

    [Header("Settings")]
    public RectTransform topBar;

    [Header("Promotion")]
    public RectTransform promotionTopNav;
    public RectTransform promotionBottomNav;

    void Awake()
    {
        notchOverlay.SetActive(false);
        #if UNITY_EDITOR
        // Detect iphoneX emulation in editor
        if (!((Screen.width == 1125 && Screen.height == 2436) || (Screen.width == 1242 && Screen.height == 2688))) return;
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
        SetY(lobbyScrollView, -63f - 100f);
        SetTop(lobbyViewPort, 9f);
        SetBottom(lobbyViewPort, 108f + 100f);
        SetY(lobbyScrollViewPivotForBanner, -66f - 100f -141);
        lobbyView.setScorllViewportBottomTo = 351;
        SetY(lobbyScrollViewTopShadow, -2);
        SetY(quickMatchButton, 347f);

        // PROFILE
        SetY(profileFacebookButton, -270f);
        SetY(profileCenterContent, -66f);

        // FRIENDS
        SetY(friendsScrollView, -31f - 100f);
        SetTop(friendsViewPort, 0f);
        SetBottom(friendsViewPort, 100f + 100f);
        SetY(friendsScrollViewTopShadow, -10);

        // CPU GAME
        SetY(cpuTopBar, -148f);
        SetY(cpuBotBar, 141f);
        foreach (RectTransform tfm in cpuLeft)
            ShiftX(tfm, 10f);
        foreach (RectTransform tfm in cpuRight)
            ShiftX(tfm, -10f);

        // MULTIPLAYER GAME
        SetY(mpTopBar, -148f);
        SetY(mpBotBar, 141f);
        foreach (RectTransform tfm in mpLeft)
            ShiftX(tfm, 10f);
        foreach (RectTransform tfm in mpRight)
            ShiftX(tfm, -10f);

        // CHAT NEW
        SetY(chatNewTopBar, -193.1f);
        SetY(chatNewScrollView, -45.07f);
        SetHeight(chatNewScrollView, 1405.84f);
        SetBottom(chatNewViewPort, 0f);
        SetY(chatNewBotBar, 141f);

        // NOTIFICATIONS
        SetY(dummyPosition, -200f);

        // RECONNECTING
        SetY(reconnectingPopup, -130f);
        SetY(maintenanceWarningPopup, -130f);

        //SETTINGS
        SetY(topBar, -106f);

        //PROMOTION
        SetY(promotionTopNav, -85f);
        SetY(promotionBottomNav, 94f);
    }

    void SetY(RectTransform tfm, float y)
    {
        Vector2 anchoredPos = tfm.anchoredPosition;
        anchoredPos.y = y;
        tfm.anchoredPosition = anchoredPos;
    }

    void SetX(RectTransform tfm, float x)
    {
        Vector2 anchoredPos = tfm.anchoredPosition;
        anchoredPos.x = x;
        tfm.anchoredPosition = anchoredPos;
    }

    void ShiftX(RectTransform tfm, float x)
    {
        Vector2 anchoredPos = tfm.anchoredPosition;
        anchoredPos.x += x;
        tfm.anchoredPosition = anchoredPos;
    }

    void SetTop(RectTransform tfm, float top)
    {
        tfm.offsetMax = new Vector2(tfm.offsetMax.x, top);
    }

    void SetBottom(RectTransform tfm, float bottom)
    {
        tfm.offsetMin = new Vector2(tfm.offsetMin.x, bottom);
    }

    void SetLocalScale(RectTransform tfm, float scale)
    {
        tfm.localScale = new Vector3(scale, scale, tfm.localScale.z);
    }

    void SetHeight(RectTransform tfm, float height)
    {
        tfm.sizeDelta = new Vector2(tfm.sizeDelta.x, height);
    }
}
