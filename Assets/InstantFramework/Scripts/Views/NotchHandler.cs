using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;

public class NotchHandler : MonoBehaviour {

    [Header("Navs")]
    public RectTransform[] topNavs;
    public RectTransform[] botNavs;

    [Header("Lobby")]
    public RectTransform lobbyScrollView;
    public RectTransform lobbyViewPort;
   
    [Header("Profile")]
    public RectTransform profileFacebookButton;
    public RectTransform profileCenterContent;

    [Header("Friends")]
    public RectTransform friendsScrollView;
    public RectTransform friendsViewPort;

    [Header("Store")]
    public RectTransform storeTitle;
    public RectTransform bundle1;
    public RectTransform bundle2;
    public RectTransform tabsBar;
    public RectTransform[] storeScrollView;
    public RectTransform[] storeViewPort;
    public RectTransform[] storeScrollContents;

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

    [Header("Chat")]
    public RectTransform chatTopBar;
    public RectTransform chatBackLabel;
    public RectTransform chatScrollView;
    public RectTransform chatViewPort;

    [Header("Notifications")]
    public RectTransform dummyPosition;

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
        SetY(lobbyScrollView, -66f - 100f);
        SetTop(lobbyViewPort, 17f);
        SetBottom(lobbyViewPort, 111f + 100f);
        
        // PROFILE
        SetY(profileFacebookButton, -270f);
        SetY(profileCenterContent, -66f);

        // FRIENDS
        SetY(friendsScrollView, -48f - 100f);
        SetTop(friendsViewPort, 17f);
        SetBottom(friendsViewPort, 111f + 100f);

        // STORE
        SetY(storeTitle, 777f);
        SetY(bundle1, -428f);
        SetY(bundle2, -677f);
        SetY(tabsBar, 179f);
        foreach (RectTransform tfm in storeScrollView)        
            SetY(tfm, -393f);
        foreach (RectTransform tfm in storeViewPort)
            SetBottom(tfm, 127f);
        foreach (RectTransform tfm in storeScrollContents)
            SetY(tfm, 517f);

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

        // CHAT
        SetY(chatTopBar, -193.1f);
        SetY(chatBackLabel, -883f);
        SetY(chatScrollView, -45.07f);
        SetHeight(chatScrollView, 1405.84f);
        SetBottom(chatViewPort, 0f);

        // NOTIFICATIONS
        SetY(dummyPosition, -200f);
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
