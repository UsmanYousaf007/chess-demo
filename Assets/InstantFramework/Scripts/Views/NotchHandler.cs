using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using UnityEngine.UI;

public class NotchHandler : MonoBehaviour {

    [Header("Navs")]
    public RectTransform[] topNavs;
    public RectTransform[] secondTierTopNavs;
    public RectTransform[] botNavs;
    public RectTransform mainBottomNav;

    [Header("Lobby")]
    public RectTransform lobbyScrollView;
    public RectTransform lobbyViewPort;
    public RectTransform lobbyScrollViewPivotForBanner;
    public LobbyView lobbyView;
    public RectTransform lobbyScrollViewTopShadow;

    [Header("Profile")]
    public RectTransform profileFacebookButton;
    public RectTransform profileAppVersion;
    public RectTransform profileLinksPanel;
    public RectTransform profileCenterContent;
    public RectTransform profileBottomNav;

    [Header("Friends")]
    public RectTransform friendsScrollView;
    public RectTransform friendsViewPort;
    public RectTransform friendsScrollViewTopShadow;
    public RectTransform friendsSecondTierTopNav;

    [Header("CPU Game")]
    public RectTransform cpuTopBar;
    public RectTransform cpuBotBar;
    public RectTransform[] cpuLeft;
    public RectTransform[] cpuRight;

    [Header("Multiplayer Game")]
    public RectTransform mpTopBar;
    public RectTransform mpOfferDraw;
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

    [Header("Subscription")]
    public RectTransform subscriptionTitle;
    public RectTransform subscriptionOffers;

    [Header("Promotion")]
    public RectTransform promotionTitle;
    public RectTransform promotionOffers;

    [Header("Manage Subscription")]
    public RectTransform manageSubscriptionBottomNav;

    [Header("Manage Blocked Friends")]
    public RectTransform blockedFriendsScrollView;
    public RectTransform blockedFriendsScrollViewPort;
    public RectTransform blockedFriendsSecondTierTopNav;

    [Header("Lesson Views")]
    public RectTransform topicsViewBottomNav;
    public RectTransform topicsScrollView;
    public RectTransform lessonsViewBottomNav;
    public RectTransform lessonsScrollView;
    public RectTransform lessonVideoPlayerTopBar;
    public RectTransform lessonVideoPlayerBottomNav;

    [Header("Shop")]
    public RectTransform shopScrollView;
    public RectTransform shopScrollViewport;

    [Header("Inventory")]
    public RectTransform inventoryHeading;
    public RectTransform inventorySpecialItemsScrollView;
    public RectTransform inventoryThemesScrollView;
    public RectTransform inventoryTitleBar;
    public RectTransform inventoryThemesScrollViewport;
    public RectTransform inventorySpecialItemsScrollViewport;

    [Header("Inbox")]
    public RectTransform inboxItemsContainer;
    public RectTransform inboxBottomNav;

    [Header("League Perks")]
    public RectTransform leaguePerksTopNav;
    public RectTransform leaguePerksBottomNav;
    public RectTransform leaguePerksScrollView;

    [Header("Tournaments")]
    public RectTransform tournamentsLeagueHeader;
    public RectTransform tournamentsScrollView;

    [Header("Tournaments Leaderboard")]
    public RectTransform tournamentsLeaderboardBottomNav;
    public RectTransform tournamentsLeaderboardHeader;
    public RectTransform tournamentsLeaderboardFooter;
    public RectTransform tounamentsLeaderboardScrollViewport;
    public RectTransform tournamentsLeaderboardNotEnteredBar;

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
            SetY(tfm, -165f);
        }

        foreach (RectTransform tfm in secondTierTopNavs)
        {
            SetY(tfm, -321f);
        }

        foreach (RectTransform tfm in botNavs)
        {
            SetY(tfm, 192f);
        }

        //SetY(mainBottomNav, 167f);
        
        // LOBBY
        //SetY(lobbyScrollView, -240f);
        SetTop(lobbyViewPort, 600);
        SetBottom(lobbyViewPort, 263);
        SetY(lobbyScrollViewPivotForBanner, -373);

        lobbyView.scrollViewportOrginalBottom = 263;
        lobbyView.scrollViewportOrginalTop = 600;

        lobbyView.setScrollViewportBottomTo = 263;
        lobbyView.setScrollViewportTopTo = 720;

        SetY(lobbyScrollViewTopShadow, -2);

        // PROFILE156
        SetY(profileFacebookButton, -281f);
        SetY(profileLinksPanel, -786f);
        SetY(profileAppVersion, 294f);
        SetY(profileCenterContent, -95f);
        SetY(profileBottomNav, 156f);

        // FRIENDS
        //SetY(friendsScrollView, -31f - 100f);
        SetTop(friendsViewPort, -336f);
        SetBottom(friendsViewPort, 260f);
        SetY(friendsScrollViewTopShadow, -10);
        SetY(friendsSecondTierTopNav, -163);

        // CPU GAME
        SetY(cpuTopBar, -148f);
        SetY(cpuBotBar, 141f);
        foreach (RectTransform tfm in cpuLeft)
            ShiftX(tfm, 10f);
        foreach (RectTransform tfm in cpuRight)
            ShiftX(tfm, -10f);

        // MULTIPLAYER GAME
        SetY(mpTopBar, -148f);
        SetY(mpOfferDraw, -1074f);
        SetY(mpBotBar, 141f);
        foreach (RectTransform tfm in mpLeft)
            ShiftX(tfm, 10f);
        foreach (RectTransform tfm in mpRight)
            ShiftX(tfm, -10f);

        // CHAT NEW
        SetY(chatNewTopBar, -33f);
        //SetY(chatNewScrollView, -45.07f);
        SetHeight(chatNewScrollView, 1326f);
        SetBottom(chatNewViewPort, 0f);
        SetY(chatNewBotBar, 141f);

        // NOTIFICATIONS
        SetY(dummyPosition, -200f);

        // RECONNECTING
        SetY(reconnectingPopup, -130f);
        SetY(maintenanceWarningPopup, -130f);

        //SETTINGS
        SetY(topBar, -106f);

        //Subscription
        SetY(subscriptionTitle, -106f);
        SetY(subscriptionOffers, -626f);

        //Promotion
        SetY(promotionTitle, -106f);
        SetY(promotionOffers, -435f);

        //Manage Subscription
        SetY(manageSubscriptionBottomNav, 60);

        // Blocked Friends
        blockedFriendsScrollViewPort.offsetMin = new Vector2(blockedFriendsScrollViewPort.offsetMin.x, 295);
        blockedFriendsScrollViewPort.offsetMax = new Vector2(blockedFriendsScrollViewPort.offsetMax.x, -324);
        SetY(blockedFriendsSecondTierTopNav, -165);
   
        //Lesson Views
        SetY(topicsViewBottomNav, 89f);
        SetTop(topicsScrollView, -223f);
        SetBottom(topicsScrollView, 213f);
        SetY(lessonsViewBottomNav, 89f);
        SetTop(lessonsScrollView, -208f);
        SetBottom(lessonsScrollView, 213f);
        SetY(lessonVideoPlayerTopBar, -113f);
        SetY(lessonVideoPlayerBottomNav, 91f);

        //Shop
        SetTop(shopScrollViewport, -220f);
        SetBottom(shopScrollViewport, 260f);
        shopScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        //Inventory
        SetY(inventoryHeading, -258f);
        SetTop(inventorySpecialItemsScrollViewport, -400f);
        SetBottom(inventorySpecialItemsScrollViewport, 260f);
        SetTop(inventoryThemesScrollViewport, -400f);
        SetBottom(inventoryThemesScrollViewport, 260f);

        SetY(inventoryTitleBar, -318f);
        inventorySpecialItemsScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        //Inbox
        SetTop(inboxItemsContainer, -175f);
        SetBottom(inboxItemsContainer, 232f);
        SetY(inboxBottomNav, 90f);

        //League Perks
        SetY(leaguePerksTopNav, -170f);
        SetY(leaguePerksBottomNav, 90f);
        SetTop(leaguePerksScrollView, -232f);
        SetBottom(leaguePerksScrollView, 212f);

        //Tournaments
        SetY(tournamentsLeagueHeader, -330f);
        SetTop(tournamentsScrollView, -418f);
        SetBottom(tournamentsScrollView, 235f);
        tournamentsScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        //Tournaments Leaderboard
        SetY(tournamentsLeaderboardBottomNav, 90f);
        SetY(tournamentsLeaderboardHeader, -232f);
        SetY(tournamentsLeaderboardFooter, 321f);
        SetTop(tounamentsLeaderboardScrollViewport, -993f);
        SetBottom(tounamentsLeaderboardScrollViewport, 642f);

        SetY(tournamentsLeaderboardNotEnteredBar, 538f);
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
