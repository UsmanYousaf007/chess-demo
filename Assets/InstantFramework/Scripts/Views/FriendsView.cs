/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using System;
using TurboLabz.InstantGame;
using System.Text;
using TMPro;

namespace TurboLabz.InstantFramework
{
    public partial class FriendsView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        [Inject] public NewFriendSignal newFriendSignal { get; set; }
        [Inject] public SearchFriendSignal searchFriendSignal { get; set; }
        [Inject] public FriendBarBusySignal friendBarBusySignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        private SpritesContainer defaultAvatarContainer;

        public Transform listContainer;
		public GameObject friendBarPrefab;

        public Text inviteFriendsText;
        public Button defaultInviteFriendsButton;
        public Button InviteFriendsButton;

        public Transform sectionPlayAFriend;
        public GameObject sectionPlayAFriendEmpty;
        public GameObject sectionPlayAFriendEmptyNotLoggedIn;
        public Transform sectionSearched;
        public GameObject sectionSearchResultsEmpty;
        public Transform sectionRecentlyCompleted;

        public Text sectionRecentlyCompletedMatchesTitle;

        public Text sectionPlayAFriendTitle;
        public Text sectionSearchResultsTitle;

		public GameObject confirmDlg;
        public Text facebookLoginRewardText;
        public Button facebookLoginButton;
        public Text facebookLoginButtonText;
        public GameObject facebookConnectAnim;
        public Text facebookConnectText;
        public Button signInWithAppleButton;
        public Text signInWithAppleLabel;
        public ScrollRect scrollRect;
        public GameObject uiBlocker;
        public GameObject processingUi;
        public Button editorSubmit;
        public TMP_InputField inputField;
        public Button cancelSearchButton;
        public Button nextSearchButton;
        public Text nextSearchButtonText;
        public Image nextSearchButtonTextUnderline;
        public Text searchBoxText;


        [Header("Confirm new game dialog")]
        public StartGameConfirmationPrefab startGameConfirmationDlg;

        [Header("Online Status")]
        public Sprite online;
        public Sprite offline;
        public Sprite active;

        [Header("Confirm remove community friend")]
        public GameObject removeCommunityFriendDlg;
        public Button removeCommunityFriendYesBtn;
        public Button removeCommunityFriendNoBtn;
        public Text removeCommunityFriendYesBtnText;
        public Text removeCommunityFriendNoBtnText;
        public Text removeCommunityFriendTitleText;

        [Header("Limit Reached")]
        public GameObject createMatchLimitReachedDlg;
        public Button createMatchLimitReachedCloseBtn;
        public Text createMatchLimitReachedCloseBtnText;
        public Button createMatchLimitReachedCrossBtn;
        public Button createMatchLimitReachedUpgradeBtn;
        public Text createMatchLimitReachedUpgradeBtnText;
        public Text createMatchLimitReachedText;
        public Text createMatchLimitReachedTitleText;

        [Header("Invite Friend")]
        public GameObject inviteFriendDlg;
        public Button inviteFriendCloseBtn;
        public Button inviteFriendBtn;
        public Text inviteText;
        public Text inviteFriendTitleText;

        [Header("Find your friend")]
        public GameObject findFriendDlg;
        public Text findFriendTitle;
        public Text findFriendLoginInfoText;
        public Text findFriendSearchInfoText;
        public Text findFriendInviteInfoText;
        public Button findFriendOkButton;
        public Text findFriendOkText;

        [Header("Drop Down Menu")]
        public Button menuButton;
        public GameObject dropDownMenu;
        public Button manageBlockedPlayersButton;
        public Text manageBlockedPlayersText;

        public Signal facebookButtonClickedSignal = new Signal();
        public Signal reloadFriendsSignal = new Signal();
        public Signal<string, FriendBar> showProfileDialogSignal = new Signal<string, FriendBar>();
        public Signal<string, bool> playButtonClickedSignal = new Signal<string, bool>();
        public Signal<string> acceptButtonClickedSignal = new Signal<string>();
        public Signal<string> declineButtonClickedSignal = new Signal<string>();
        public Signal<string> cancelButtonClickedSignal = new Signal<string>();
        public Signal<string> okButtonClickedSignal = new Signal<string>();
        public Signal<int> actionCountUpdatedSignal = new Signal<int>();
        public Signal<string> removeCommunityFriendSignal = new Signal<string>();
        public Signal<string, bool, string> quickMatchFriendButtonClickedSignal = new Signal<string, bool, string>();
        public Signal<string> showChatSignal = new Signal<string>();
        public Signal upgradeToPremiumButtonClickedSignal = new Signal();
        public Signal manageBlockedFriendsButtonClickedSignal = new Signal();
        public Signal inviteFriendSignal = new Signal();
        public Signal signInWithAppleClicked = new Signal();
        public Signal localRefreshFriends = new Signal();

        private GameObjectsPool friendBarsPool;
        private Dictionary<string, FriendBar> bars = new Dictionary<string, FriendBar>();
        private List<GameObject> defaultInvite = new List<GameObject>();
        private FriendBar actionBar;
        private string eloPrefix;
        private string startGameFriendId;
        private bool startGameRanked;
        private List<GameObject> cacheEnabledSections;
        private int searchSkip;
        private bool isSearchNext;
        private bool inSearchView; 

        public void Init()
        {
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            facebookLoginRewardText.text = "";
            facebookLoginButtonText.text = localizationService.Get(LocalizationKey.FRIENDS_FACEBOOK_LOGIN_BUTTON_TEXT);
            inviteFriendsText.text = localizationService.Get(LocalizationKey.FRIENDS_NO_FRIENDS_TEXT);
            facebookConnectText.text = localizationService.Get(LocalizationKey.FRIENDS_FACEBOOK_CONNECT_TEXT);
            signInWithAppleLabel.text = localizationService.Get(LocalizationKey.SIGN_IN);

            sectionPlayAFriendTitle.text = localizationService.Get(LocalizationKey.FRIENDS_SECTION_PLAY_A_FRIEND);
            sectionSearchResultsTitle.text = localizationService.Get(LocalizationKey.FRIENDS_SECTION_SEARCH_RESULTS);
            sectionRecentlyCompletedMatchesTitle.text = localizationService.Get(LocalizationKey.FRIENDS_SECTION_RECENTLY_COMPLETED_MATCHES);

            startGameConfirmationDlg.confirmRankedGameBtnText.text = localizationService.Get(LocalizationKey.NEW_GAME_CONFIRM_RANKED);

            startGameConfirmationDlg.confirmFriendly1MinGameBtnText.text = localizationService.Get(LocalizationKey.MIN1_GAME_TEXT);
            startGameConfirmationDlg.confirmFriendly5MinGameBtnText.text = localizationService.Get(LocalizationKey.MIN5_GAME_TEXT);
            startGameConfirmationDlg.confirmFriendly10MinGameBtnText.text = localizationService.Get(LocalizationKey.MIN10_GAME_TEXT);
            startGameConfirmationDlg.confirmFriendly30MinGameBtnText.text = localizationService.Get(LocalizationKey.MIN30_GAME_TEXT);

            startGameConfirmationDlg.startGameText.text = localizationService.Get(LocalizationKey.NEW_GAME_CONFIRM_START_GAME);
            startGameConfirmationDlg.confirmRankedGameBtn.onClick.AddListener(ConfirmRankedGameBtnClicked);

            startGameConfirmationDlg.confirmFriendly1MinGameBtn.onClick.AddListener(delegate { ConfirmFriendlyGameBtnClicked(FindMatchAction.ActionCode.Challenge1.ToString()); });
            startGameConfirmationDlg.confirmFriendly5MinGameBtn.onClick.AddListener(delegate { ConfirmFriendlyGameBtnClicked(FindMatchAction.ActionCode.Challenge.ToString()); });
            startGameConfirmationDlg.confirmFriendly10MinGameBtn.onClick.AddListener(delegate { ConfirmFriendlyGameBtnClicked(FindMatchAction.ActionCode.Challenge10.ToString()); });
            startGameConfirmationDlg.confirmFriendly30MinGameBtn.onClick.AddListener(delegate { ConfirmFriendlyGameBtnClicked(FindMatchAction.ActionCode.Challenge30.ToString()); });

            startGameConfirmationDlg.tooltipBtn.onClick.AddListener(delegate { ToolTipBtnClicked(); });

            startGameConfirmationDlg.confirmGameCloseBtn.onClick.AddListener(ConfirmNewGameDlgNo);
            startGameConfirmationDlg.ToggleRankButton.onClick.AddListener(OnToggleRankButtonClicked);
            startGameConfirmationDlg.toggleRankButtonState = true;
            eloPrefix = localizationService.Get(LocalizationKey.ELO_SCORE);

            removeCommunityFriendYesBtnText.text = localizationService.Get(LocalizationKey.REMOVE_COMMUNITY_FRIEND_YES);
            removeCommunityFriendNoBtnText.text = localizationService.Get(LocalizationKey.REMOVE_COMMUNITY_FRIEND_NO);
            removeCommunityFriendTitleText.text = localizationService.Get(LocalizationKey.REMOVE_COMMUNITY_FRIEND_TITLE);

            facebookLoginButton.onClick.AddListener(OnFacebookButtonClicked);
            signInWithAppleButton.onClick.AddListener(OnSignInWithAppleClicked);

            removeCommunityFriendYesBtn.onClick.AddListener(RemoveCommunityFriendDlgYes);
            removeCommunityFriendNoBtn.onClick.AddListener(RemoveCommunityFriendDlgNo);

            createMatchLimitReachedCloseBtn.onClick.AddListener(OnCloseButtonClickedCreateMatchLimitDlg);
            createMatchLimitReachedCrossBtn.onClick.AddListener(OnCloseButtonClickedCreateMatchLimitDlg);
            createMatchLimitReachedUpgradeBtn.onClick.AddListener(OnUpgradeToPremiumButtonClicked);
            createMatchLimitReachedCloseBtnText.text = localizationService.Get(LocalizationKey.OKAY_TEXT);
            createMatchLimitReachedUpgradeBtnText.text = localizationService.Get(LocalizationKey.UPGRADE_TEXT);

            inviteText.text = localizationService.Get(LocalizationKey.FRIENDS_INVITE_BUTTON_TEXT);
            inviteFriendTitleText.text = localizationService.Get(LocalizationKey.FRIENDS_INVITE_TITLE_TEXT);
            inviteFriendCloseBtn.onClick.AddListener(InviteFriendDialogCloseButtonClicked);
            inviteFriendBtn.onClick.AddListener(InviteFriendDialogButtonClicked);
            defaultInviteFriendsButton.onClick.AddListener(OnDefaultInviteFriendsButtonClicked);
            InviteFriendsButton.onClick.AddListener(OnDefaultInviteFriendsButtonClicked);

#if UNITY_EDITOR
            editorSubmit.gameObject.SetActive(true);
            editorSubmit.onClick.AddListener(() => { OnSearchSubmit(inputField.text); });
#else
            editorSubmit.gameObject.SetActive(false);
#endif

            nextSearchButton.onClick.AddListener(OnNextSearchBtnClicked);
            cacheEnabledSections = new List<GameObject>();
            searchSkip = 0;
            cancelSearchButton.interactable = false;
            cancelSearchButton.gameObject.SetActive(false);
            inputField.onEndEdit.AddListener(OnSearchSubmit);
            cancelSearchButton.onClick.AddListener(OnCancelSearchClicked);
            nextSearchButton.interactable = false;
            ResetSearch();


            findFriendTitle.text = localizationService.Get(LocalizationKey.FRIENDS_FIND_FRIEND_TITLE); 
            findFriendLoginInfoText.text = localizationService.Get(LocalizationKey.FRIENDS_FIND_FRIEND_LOGIN_INFO); 
            findFriendSearchInfoText.text = localizationService.Get(LocalizationKey.FRIENDS_FIND_FRIEND_SEARCH_INFO);
            findFriendInviteInfoText.text = localizationService.Get(LocalizationKey.FRIENDS_FIND_FRIEND_INVITE_INFO); 
            findFriendOkButton.onClick.AddListener(FriendsHelpDialogCloseButtonClicked);
            findFriendOkText.text = localizationService.Get(LocalizationKey.LONG_PLAY_OK);

            manageBlockedPlayersText.text = localizationService.Get(LocalizationKey.FRIENDS_MANAGE_BLOCKED);
            menuButton.onClick.AddListener(() => dropDownMenu.SetActive(true));
            manageBlockedPlayersButton.onClick.AddListener(OnManageBlockedFriendsClicked);

            // Initializing Friend Bars Pool
            friendBarsPool = new GameObjectsPool(friendBarPrefab);
        }

        #region InviteFriendDialog
        private void OnDefaultInviteFriendsButtonClicked()
        {
            inviteFriendDlg.SetActive(true);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INVITE_DLG);
        }
        private void InviteFriendDialogCloseButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void InviteFriendDialogButtonClicked()
        {
            inviteFriendSignal.Dispatch();
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        public void HideInviteDlg()
        {
            inviteFriendDlg.SetActive(false);
        }
        #endregion

        void CacheEnabledSections()
        {
            cacheEnabledSections.Clear();

            if (sectionPlayAFriendEmptyNotLoggedIn.gameObject.activeSelf) cacheEnabledSections.Add(sectionPlayAFriendEmptyNotLoggedIn);
            if (sectionPlayAFriend.gameObject.activeSelf) cacheEnabledSections.Add(sectionPlayAFriend.gameObject);
            if (sectionPlayAFriendEmpty.gameObject.activeSelf) cacheEnabledSections.Add(sectionPlayAFriendEmpty);
            if (sectionRecentlyCompleted.gameObject.activeSelf) cacheEnabledSections.Add(sectionRecentlyCompleted.gameObject);

        }

        void OnSearchSubmit(string text)
        {
            if (inputField.text.Length == 0)
            {
                return;
            }

            if (!isSearchNext)
            {
                searchSkip = 0;
            }

            inSearchView = true;

            uiBlocker.gameObject.SetActive(true);
            searchBoxText.text = inputField.text;
            searchBoxText.text  = searchBoxText.text.Replace("\n", " ");
            cancelSearchButton.interactable = true;
            cancelSearchButton.gameObject.SetActive(true);
            sectionSearchResultsEmpty.gameObject.SetActive(false);

            ClearType(FriendCategory.FRIEND);
            ClearType(FriendCategory.COMMUNITY);

            ClearSearchResults();
            searchFriendSignal.Dispatch(inputField.text, searchSkip);

            if (cacheEnabledSections.Count == 0)
            {
                CacheEnabledSections();
                sectionPlayAFriend.gameObject.SetActive(false);
                sectionPlayAFriendEmpty.gameObject.SetActive(false);
                sectionPlayAFriendEmptyNotLoggedIn.gameObject.SetActive(false);
                sectionSearchResultsEmpty.gameObject.SetActive(false);
                sectionRecentlyCompleted.gameObject.SetActive(false);
            }
        }

        void OnNextSearchBtnClicked()
        {
            isSearchNext = true;
            OnSearchSubmit(inputField.text);
            isSearchNext = false;
        }

        public void ResetSearch()
        {
            ClearSearchResults();
            inSearchView = false;
            sectionSearched.gameObject.SetActive(false);
            sectionSearchResultsEmpty.gameObject.SetActive(false);
            cancelSearchButton.interactable = false;
            cancelSearchButton.gameObject.SetActive(false);
            searchSkip = 0;
            isSearchNext = false;
            inputField.text = "";
            searchBoxText.text = "Search by player display name or tag..";
            foreach (GameObject obj in cacheEnabledSections)
            {
                obj.SetActive(true);
            }
            cacheEnabledSections.Clear();
            if(playerModel.search != null)
                playerModel.search.Clear();

            // Adding all friend bars in view after clearing search friends from view.
            if (playerModel.friends != null)
            {
                localRefreshFriends.Dispatch();
            }
        }

        public void OnCancelSearchClicked()
        {
            ResetSearch();
        }

        public void ShowConnectFacebook(bool showConnectInfo)
        {
            startGameFriendId = null;

            if (showConnectInfo)
            {
                listContainer.gameObject.SetActive(true);
                facebookLoginButton.gameObject.SetActive(false);
                facebookLoginButton.enabled = false;
                signInWithAppleButton.gameObject.SetActive(false);
                signInWithAppleButton.enabled = false;
                facebookConnectText.gameObject.SetActive(false);
                facebookConnectAnim.SetActive(false);
                uiBlocker.SetActive(false);
                scrollRect.verticalNormalizedPosition = 1f;

                sectionPlayAFriendEmptyNotLoggedIn.gameObject.SetActive(true);

                //listContainer.gameObject.SetActive(false);
                facebookLoginButton.gameObject.SetActive(true);
                facebookLoginButton.enabled = true;
                signInWithAppleButton.gameObject.SetActive(signInWithAppleService.IsSupported());
                signInWithAppleButton.enabled = signInWithAppleService.IsSupported();
                facebookConnectText.gameObject.SetActive(true);
                facebookConnectAnim.SetActive(false);
            }
            else
            {
                listContainer.gameObject.SetActive(true);
                facebookLoginButton.gameObject.SetActive(false);
                facebookLoginButton.enabled = false;
                signInWithAppleButton.gameObject.SetActive(false);
                signInWithAppleButton.enabled = false;
                facebookConnectText.gameObject.SetActive(false);
                facebookConnectAnim.SetActive(false);
                uiBlocker.SetActive(false);
                scrollRect.verticalNormalizedPosition = 1f;

                sectionPlayAFriendEmptyNotLoggedIn.gameObject.SetActive(false);
            }
        }

        public void CreateGame(string friendId, bool isRanked)
        {
            //// Facebook not logged in
            //if (!facebookService.isLoggedIn())
            //{
            //    startGameFriendId = friendId;
            //    startGameRanked = isRanked;
            //    OnFacebookButtonClicked();
            //    return;
            //}

            // Friend needs to be added
            if (friendId != null && !bars.ContainsKey(friendId))
            {
                startGameFriendId = friendId;
                newFriendSignal.Dispatch(friendId, false);
                return;
            }

            // Start this game
            if (friendId != null)
            {
                playButtonClickedSignal.Dispatch(friendId, isRanked);
                startGameFriendId = null;
            }
        }

        public void SignInWithAppleResult(AuthSignInWIthAppleResultVO vo)
        {
            LogUtil.Log("SignInWithAppleAuthResult :: " + vo.isSuccessful);
            facebookConnectAnim.SetActive(false);
            uiBlocker.SetActive(false);

            if (vo.isSuccessful)
            {
                // Player attempted to start a game
                if ((startGameFriendId != null) && (vo.playerId != startGameFriendId))
                {
                    CreateGame(startGameFriendId, startGameRanked);
                }

                // Freak case where player started a game with themselves while they were a community player
                if ((startGameFriendId != null) && (vo.playerId == startGameFriendId))
                {
                    startGameFriendId = null;
                }
            }
            else
            {
                ShowConnectFacebook(true);
            }
        }

        public void SignOutSocialAccount()
        {
            ShowConnectFacebook(true);
        }

        public void FacebookAuthResult(AuthFacebookResultVO vo)
        {
            LogUtil.Log("FacebookAuthResult :: " + vo.isSuccessful);
            facebookConnectAnim.SetActive(false);
            uiBlocker.SetActive(false);

            if (vo.isSuccessful)
            {
                // Player attempted to start a game
                if ((startGameFriendId != null) && (vo.playerId != startGameFriendId))
                {
                    CreateGame(startGameFriendId, startGameRanked);
                }

                // Freak case where player started a game with themselves while they were a community player
                if ((startGameFriendId != null) && (vo.playerId == startGameFriendId))
                {
                    startGameFriendId = null;
                }
            }
            else
            {
                ShowConnectFacebook(true);
            }
        }

        public void CreateQuickMatchGame(string friendId, bool isRanked, string actionCode)
        {
            // Start a quick match 
            if (friendId != null)
            {
                quickMatchFriendButtonClickedSignal.Dispatch(friendId, isRanked, actionCode);
                startGameFriendId = null;
            }
        }

        public void NewFriendAdded(string friendId)
        {
            if (startGameFriendId == friendId)
            {
                CreateGame(startGameFriendId, startGameRanked);
            }
        }

        public void AddFriends(Dictionary<string, Friend> friends, bool isCommunity, bool isSearched)
        {
            foreach (KeyValuePair<string, Friend> entry in friends)
            {
                AddFriend(entry.Value, isCommunity, isSearched);
            }

            //UpdateAllStatus();
            //RefreshDefaultMessages();

            //AddTestBars();
        }

        void AddFriend(Friend friend, bool isCommunity, bool isSearched)
		{
            if (bars.ContainsKey(friend.playerId) || (!isSearched && (isCommunity || friend.friendType.Equals(Friend.FRIEND_TYPE_COMMUNITY))))
            {
                return;
            }

            GameObject friendBarObj = friendBarsPool.GetObject();
            FriendBar friendBar = friendBarObj.GetComponent<FriendBar>();

            SkinLink[] objects = friendBarObj.GetComponentsInChildren<SkinLink>();
            for (int i =0; i< objects.Length;i++)
            {
                objects[i].InitPrefabSkin();
            }

            // update bar values
            friendBar.Init(localizationService);
            friendBar.lastMatchTimeStamp = friend.lastMatchTimestamp;
            friendBar.viewProfileButton.onClick.AddListener(() => ViewProfile(friend.playerId, friendBar));
            friendBar.stripButton.onClick.AddListener(() => PlayButtonClicked(friendBar));
            friendBar.acceptButton.onClick.AddListener(() => AcceptButtonClicked(friend.playerId, friendBar.acceptButton));
            friendBar.notNowButton.onClick.AddListener(() => DeclineButtonClicked(friend.playerId, friendBar.notNowButton));
            friendBar.cancelButton.onClick.AddListener(() => CancelButtonClicked(friend.playerId, friendBar.cancelButton));
            friendBar.okButton.onClick.AddListener(() => OkButtonClicked(friend.playerId, friendBar.okButton));
            friendBar.viewButton.onClick.AddListener(() => PlayButtonClicked(friendBar));
            friendBar.unreadChat.onClick.AddListener(() => ShowChat(friend.playerId));
            friendBar.removeCommunityFriendButton.onClick.AddListener(() => RemoveCommunityFriendButtonClicked(friend));
            friendBar.friendInfo = friend;
            friendBar.profileNameLabel.text = friend.publicProfile.name;
            friendBar.eloScoreLabel.text = friend.publicProfile.eloScore.ToString();
            friendBar.isCommunity = isCommunity;
            friendBar.isSearched = isSearched;
            friendBar.friendType = friend.friendType;
            friendBar.isOnline = friend.publicProfile.isOnline;
            friendBar.isActive = friend.publicProfile.isActive;
            if (!friend.publicProfile.isOnline && friend.publicProfile.isActive)
            {
                friendBar.onlineStatus.sprite = friendBar.activeStatus;
            }
            else
            {
                friendBar.onlineStatus.sprite = friend.publicProfile.isOnline ? friendBar.online : friendBar.offline;
            }

            friendBarObj.transform.SetParent(listContainer, false);
            bars.Add(friend.playerId, friendBar);

            UpdateFriendPic(friend.playerId, friend.publicProfile);

            if (isCommunity)
            {
                friendBar.UpdateCommmunityStrip();
            }

            friendBarObj.gameObject.SetActive(true);
        }

        public void UpdateBarsSkin()
        {
            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                FriendBar bar = entry.Value;
                GameObject friendBarObj = bar.gameObject;
                SkinLink[] objects = friendBarObj.GetComponentsInChildren<SkinLink>();
                for (int i = 0; i < objects.Length; i++)
                {
                    objects[i].InitPrefabSkin();
                }
            }
        }

        public void UpdateFriendPic(string playerId, Sprite sprite)
        {
            if (sprite == null)
                return;

            TLUtils.LogUtil.LogNullValidation(playerId, "playerId");
            
            if (playerId != null && !bars.ContainsKey(playerId))
                return;

            FriendBar barData = bars[playerId].GetComponent<FriendBar>();
            barData.avatarIcon.gameObject.SetActive(false);
            barData.avatarBG.gameObject.SetActive(false);
            barData.avatarImage.sprite = sprite;
        }

        public void UpdateFriendPic(string playerId, PublicProfile publicProfile)
        {
        	TLUtils.LogUtil.LogNullValidation(playerId, "playerId");
        
            if (playerId != null && !bars.ContainsKey(playerId))
                return;

            FriendBar barData = bars[playerId].GetComponent<FriendBar>();
            barData.avatarIcon.gameObject.SetActive(false);
            barData.avatarBG.gameObject.SetActive(false);
            if (publicProfile.profilePicture != null)
            {
                barData.avatarImage.sprite = publicProfile.profilePicture;
            }
            else
            {
                if (publicProfile.avatarId != null)
                {
                    barData.avatarIcon.gameObject.SetActive(true);
                    barData.avatarBG.gameObject.SetActive(true);

                    barData.avatarBG.color = Colors.Color(publicProfile.avatarBgColorId);
                    barData.avatarIcon.sprite = defaultAvatarContainer.GetSprite(publicProfile.avatarId);
                }
            }

            barData.leagueBorder.gameObject.SetActive(publicProfile.leagueBorder != null);
            barData.leagueBorder.sprite = publicProfile.leagueBorder;
            //barData.premiumBorder.SetActive(publicProfile.isSubscriber);
        }

            public void UpdateEloScores(EloVO vo)
        {
            if (vo.opponentId == null || !bars.ContainsKey(vo.opponentId))
                return;
            
            FriendBar barData = bars[vo.opponentId].GetComponent<FriendBar>();
            barData.eloScoreLabel.text = vo.opponentEloScore.ToString();
        }   
            
        public void UpdateFriendBarStatus(LongPlayStatusVO vo)
        {
        	TLUtils.LogUtil.LogNullValidation(vo.playerId, "vo.playerId");
        
            if (vo.playerId != null && !bars.ContainsKey(vo.playerId))
            {
                return;
            }

            CloseNewGameDlg(vo.playerId);

            FriendBar friendBar = bars[vo.playerId].GetComponent<FriendBar>();
            friendBar.lastActionTime = vo.lastActionTime;
            friendBar.longPlayStatus = vo.longPlayStatus;
            friendBar.isGameCanceled = vo.isGameCanceled;
            friendBar.isPlayerTurn = vo.isPlayerTurn;
            friendBar.isRanked = vo.isRanked;
            friendBar.isOfferDraw = vo.offerDraw;
            friendBar.UpdateStatus();

            // Set the timer clocks
            if (friendBar.longPlayStatus == LongPlayStatus.NEW_CHALLENGE ||
                friendBar.longPlayStatus == LongPlayStatus.DECLINED ||
                friendBar.longPlayStatus == LongPlayStatus.WAITING_FOR_ACCEPT)
            {
                TimeSpan elapsedTime = DateTime.UtcNow.Subtract(friendBar.lastActionTime);

                if (elapsedTime.TotalHours < 1)
                {
                    friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_MINUTES,
                        Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalMinutes)));
                }
                else if (elapsedTime.TotalDays < 1)
                {
                    friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_HOURS,
                        Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalHours)));
                }
                else
                {
                    friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_DAYS,
                        Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalDays)));
                }
            } 
            else if (friendBar.longPlayStatus == LongPlayStatus.PLAYER_TURN)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Colors.GetColorString(TimeUtil.FormatStripClock(vo.playerTimer), Colors.WHITE));
                sb.Append(Colors.GetColorString(" | " + TimeUtil.FormatStripClock(vo.opponentTimer), Colors.WHITE_150));
                friendBar.timerLabel.text = sb.ToString();
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.OPPONENT_TURN)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Colors.GetColorString(TimeUtil.FormatStripClock(vo.playerTimer) + " | ", Colors.WHITE_150));
                sb.Append(Colors.GetColorString(TimeUtil.FormatStripClock(vo.opponentTimer), Colors.WHITE));
                friendBar.timerLabel.text = sb.ToString();
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.PLAYER_WON ||
                friendBar.longPlayStatus == LongPlayStatus.OPPONENT_WON ||
                friendBar.longPlayStatus == LongPlayStatus.DRAW)
            {
                friendBar.timerLabel.text = 
                    Colors.GetColorString(TimeUtil.FormatStripClock(vo.playerTimer) + "|" + TimeUtil.FormatStripClock(vo.opponentTimer), Colors.WHITE_150);
            }

            UpdateActionCount();
        }

        public void UpdateFriendOnlineStatusSignal(string friendId, bool isOnline)
        {
        	TLUtils.LogUtil.LogNullValidation(friendId, "friendId");
        
            if (friendId == null || !bars.ContainsKey(friendId))
            {
                return;
            }

            FriendBar friendBar = bars[friendId].GetComponent<FriendBar>();
            if (!isOnline && friendBar.friendInfo.publicProfile.isActive)
            {
                friendBar.onlineStatus.sprite = friendBar.activeStatus;
            }
            else
            {
                friendBar.onlineStatus.sprite = friendBar.friendInfo.publicProfile.isOnline ? friendBar.online : friendBar.offline;
            }
            friendBar.isOnline = isOnline;
        }

        public void UpdateFriendBarBusy(string playerId, bool busy, CreateLongMatchAbortReason reason)
        {
            // This function must be called in pairs (even if playerId becomes null or no longer friend) 
            // to ensure UI Blocker gets disabled
            uiBlocker.SetActive(busy);

            TLUtils.LogUtil.LogNullValidation(playerId, "playerId");

            if (playerId != null && !bars.ContainsKey(playerId))
            {
                return;
            }

            FriendBar friendBar = bars[playerId].GetComponent<FriendBar>();

            friendBar.thinking.SetActive(busy);
            friendBar.playArrow.SetActive(!busy);
            friendBar.playArrowButton.SetActive(!busy);

            createMatchLimitReachedTitleText.gameObject.SetActive(false);

            if (reason == CreateLongMatchAbortReason.LimitReached)
            {
                createMatchLimitReachedText.text = "Sorry, opponent max games limit reached. \nPlease Try Later";
                SetMatchLimitReachedDialogue(false);
                navigatorEventSignal.Dispatch(NavigatorEvent.CREATE_MATCH_LIMIT_REACHED_DIALOG);

                friendBar.playArrow.SetActive(true);
                friendBar.playArrowButton.SetActive(false);
            }
            else if (reason == CreateLongMatchAbortReason.SelfLimitReached)
            {
                SetMatchLimitReachedDialogue(true);
                navigatorEventSignal.Dispatch(NavigatorEvent.CREATE_MATCH_LIMIT_REACHED_DIALOG);

                friendBar.playArrow.SetActive(true);
                friendBar.playArrowButton.SetActive(false);
            }
            else if (reason == CreateLongMatchAbortReason.CreateFailed)
            {
                createMatchLimitReachedText.text = "Player is already waiting for you to accept a classic match";
                SetMatchLimitReachedDialogue(false);
                navigatorEventSignal.Dispatch(NavigatorEvent.CREATE_MATCH_LIMIT_REACHED_DIALOG);

                friendBar.playArrow.SetActive(false);
                friendBar.playArrowButton.SetActive(false);
            }
            else if (reason == CreateLongMatchAbortReason.Pending)
            {
                createMatchLimitReachedText.text = "Sorry, Game not created. Opponent has Pending Invites.";
                SetMatchLimitReachedDialogue(false);
                navigatorEventSignal.Dispatch(NavigatorEvent.CREATE_MATCH_LIMIT_REACHED_DIALOG);

                friendBar.playArrow.SetActive(true);
                friendBar.playArrowButton.SetActive(false);
            }
            else if (reason == CreateLongMatchAbortReason.Blocked)
            {
                createMatchLimitReachedText.text = "Sorry, a game could not be created. Please try later.";
                SetMatchLimitReachedDialogue(false);
                navigatorEventSignal.Dispatch(NavigatorEvent.CREATE_MATCH_LIMIT_REACHED_DIALOG);

                friendBar.playArrow.SetActive(true);
                friendBar.playArrowButton.SetActive(false);
            }
            else // Match successfully created
            {
                friendBar.playArrow.SetActive(false);
                friendBar.playArrowButton.SetActive(false);
            }
        }

        void SetMatchLimitReachedDialogue()
        {
            createMatchLimitReachedTitleText.gameObject.SetActive(true);
            createMatchLimitReachedTitleText.text = "You have reached your active games limit";
            if (playerModel.HasSubscription())
            {
                createMatchLimitReachedText.text = "Finish/clear a game or match invitation.";
                createMatchLimitReachedUpgradeBtn.gameObject.SetActive(false);
                createMatchLimitReachedCloseBtn.gameObject.SetActive(true);
            }
            else
            {
                createMatchLimitReachedText.text = "Go premium to increase the limit";
                createMatchLimitReachedUpgradeBtn.gameObject.SetActive(true);
                createMatchLimitReachedCloseBtn.gameObject.SetActive(false);
            }
        }

        void SetMatchLimitReachedDialogue(bool isSelfLimitReached)
        {
            if (isSelfLimitReached)
            {
                createMatchLimitReachedTitleText.text = "You have reached your active games limit";
                createMatchLimitReachedTitleText.gameObject.SetActive(true);
                if (playerModel.HasSubscription())
                {
                    createMatchLimitReachedText.text = "Finish/clear a game or match invitation.";
                    createMatchLimitReachedUpgradeBtn.gameObject.SetActive(false);
                    createMatchLimitReachedCloseBtn.gameObject.SetActive(true);
                }
                else
                {
                    createMatchLimitReachedText.text = "Go premium to increase the limit";
                    createMatchLimitReachedUpgradeBtn.gameObject.SetActive(true);
                    createMatchLimitReachedCloseBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                createMatchLimitReachedTitleText.gameObject.SetActive(false);
                createMatchLimitReachedUpgradeBtn.gameObject.SetActive(false);
                createMatchLimitReachedCloseBtn.gameObject.SetActive(true);
            }
        }

        public void UpdateFriendBarDrawOfferStatus(string status, string offeredBy, string opponentID)
        {
            //TLUtils.LogUtil.LogNullValidation(opponentID, "playerId");

            if (!bars.ContainsKey(opponentID))
            {
                return;
            }

            FriendBar friendBar = bars[opponentID].GetComponent<FriendBar>();

            if (status == "offered")
            {
                friendBar.generalStatus.rectTransform.anchoredPosition = new Vector2(friendBar.generalStatus.rectTransform.anchoredPosition.x, -27);
                friendBar.drawOffer.SetActive(true);
            }
            else
            {
                friendBar.generalStatus.rectTransform.anchoredPosition = new Vector2(friendBar.generalStatus.rectTransform.anchoredPosition.x, 0);
                friendBar.drawOffer.SetActive(false);
            }
        }

        void OnUpgradeToPremiumButtonClicked()
        {
            upgradeToPremiumButtonClickedSignal.Dispatch();
            audioService.PlayStandardClick();
        }

        public void Show() 
        {
            if(!playerModel.isFBConnectRewardClaimed)
            {
                facebookLoginRewardText.text = localizationService.Get(LocalizationKey.FACEBBOK_LOGIN_REWARD_TEXT, rewardsSettingsModel.facebookConnectReward);
            }
            
            gameObject.SetActive(true);
            startGameConfirmationDlg.gameObject.SetActive(false);
            removeCommunityFriendDlg.SetActive(false);
            createMatchLimitReachedDlg.SetActive(false);
            inviteFriendDlg.SetActive(false);
            findFriendDlg.SetActive(false);
            dropDownMenu.SetActive(false);

            // TODO: Better handling needed
            if (sectionSearched.gameObject.activeSelf)
            {
                OnCancelSearchClicked();
            }

            UpdateBarsSkin();
            SortFriends();
        }

        public void Hide()
        {
            if (inSearchView)
            {
                ResetSearch();
            }
            gameObject.SetActive(false); 
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void ClearSearchResults()
        {
            ClearType(FriendCategory.SEARCHED);
        }

        public void ClearFriends()
        {
            ClearType(FriendCategory.FRIEND);
            DefaultInviteSetActive(true);
        }

        public void ClearFriend(string friendId)
        {
            if (friendId != null && bars.ContainsKey(friendId) && (!bars[friendId].isSearched || bars[friendId].isRemoved))
            {
                bars[friendId].RemoveButtonListeners();
                friendBarsPool.ReturnObject(bars[friendId].gameObject);
                bars.Remove(friendId);
            }
        }

        public void AddUnreadMessages(string friendId, int messageCount)
        {
            if (friendId != null && bars.ContainsKey(friendId))
            {
                bars[friendId].unreadChat.gameObject.SetActive(true);
                bars[friendId].unreadChatCount.text = messageCount.ToString();
                bars[friendId].newMatchGreeting.gameObject.SetActive(false);
            }
        }

        public void ClearUnreadMessages(string friendId)
        {
            if (friendId != null && bars.ContainsKey(friendId))
            {
                bars[friendId].unreadChat.gameObject.SetActive(false);
            }
        }

        public void ToggleFacebookButton(bool toggle)
        {
            facebookLoginButton.interactable = toggle;
            signInWithAppleButton.interactable = toggle;
        }

        void ClearType(FriendCategory friendCategory)
        {
            List<string> destroyMe = new List<string>();

            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                if (friendCategory == FriendCategory.COMMUNITY && entry.Value.isCommunity)
                {
                    destroyMe.Add(entry.Key);
                }
                if (friendCategory == FriendCategory.SEARCHED && entry.Value.isSearched)
                {
                    destroyMe.Add(entry.Key);
                }
                if (friendCategory == FriendCategory.FRIEND && !entry.Value.isSearched)
                {
                    destroyMe.Add(entry.Key);
                }
            }

            foreach (string key in destroyMe)
            {
                bars[key].RemoveButtonListeners();
                friendBarsPool.ReturnObject(bars[key].gameObject);
                bars.Remove(key);
            }

            Resources.UnloadUnusedAssets();
        }

        void OnFacebookButtonClicked()
        {
            facebookButtonClickedSignal.Dispatch();
            facebookConnectAnim.SetActive(true);
            uiBlocker.SetActive(true);
            facebookLoginButton.enabled = false;
            signInWithAppleButton.enabled = false;
        }

        void DefaultInviteSetActive(bool active)
        {
            foreach (GameObject obj in defaultInvite)
            {
                obj.SetActive(active);
            }
        }

        void ViewProfile(string playerId, FriendBar bar)
        {
            audioService.PlayStandardClick();
            showProfileDialogSignal.Dispatch(playerId, bar);

            Debug.Log("BAR STATUS : ::: " + bar.longPlayStatus);
        }

        void ShowChat(string playerId)
        {
            showChatSignal.Dispatch(playerId);
        }

        void PlayButtonClicked(FriendBar bar)
        {
            audioService.PlayStandardClick();
            actionBar = null;

            if (bar.longPlayStatus == LongPlayStatus.DEFAULT && matchInfoModel.matches.Count >= settingsModel.maxLongMatchCount)
            {
                friendBarBusySignal.Dispatch(bar.friendInfo.playerId, false, CreateLongMatchAbortReason.SelfLimitReached);
                return;
            }

            if (bar.longPlayStatus == LongPlayStatus.DEFAULT)
            {
                actionBar = bar;
                ShowConfirmGameDlg(actionBar);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_START_GAME_DLG);
            }
            else
            {
                playButtonClickedSignal.Dispatch(bar.friendInfo.playerId, bar.isRanked);
            }
        }


        void RemoveCommunityFriendButtonClicked(Friend friend)
        {
            audioService.PlayStandardClick();
            actionBar = bars[friend.playerId];
            removeCommunityFriendTitleText.text = localizationService.Get(LocalizationKey.REMOVE_COMMUNITY_FRIEND_TITLE) + friend.publicProfile.name + "?";
            removeCommunityFriendDlg.SetActive(true);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REMOVE_FRIEND_DLG);
            analyticsService.Event(AnalyticsEventId.remove_strip_clicked, AnalyticsContext.friends);
        }

        void RemoveCommunityFriendDlgYes()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            actionBar.isRemoved = true;
            removeCommunityFriendSignal.Dispatch(actionBar.friendInfo.playerId);
        }

        void RemoveCommunityFriendDlgNo()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        public void HideRemoveCommunityFriendDlg()
        {
            removeCommunityFriendDlg.SetActive(false);
        }

        void AcceptButtonClicked(string playerId, Button button)
        {
            audioService.PlayStandardClick();
            acceptButtonClickedSignal.Dispatch(playerId);
            button.interactable = false;
        }

        void DeclineButtonClicked(string playerId, Button button)
        {
            audioService.PlayStandardClick();
            declineButtonClickedSignal.Dispatch(playerId);
            button.interactable = false;
        }

        void CancelButtonClicked(string playerId, Button button)
        {
            audioService.PlayStandardClick();
            cancelButtonClickedSignal.Dispatch(playerId);
            button.interactable = false;
        }

        void OkButtonClicked(string playerId, Button button)
        {
            audioService.PlayStandardClick();
            okButtonClickedSignal.Dispatch(playerId);
            button.interactable = false;
        }

        void UpdateActionCount()
        {
            int actionCount = 0;

            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                LongPlayStatus status = entry.Value.longPlayStatus;

                if (status == LongPlayStatus.PLAYER_TURN ||
                    status == LongPlayStatus.NEW_CHALLENGE)
                {
                    actionCount++;
                }
            }

            actionCountUpdatedSignal.Dispatch(actionCount);

        }

        #region StartGameConfirmationDialog

        void SetToggleRankButtonState(bool state)
        {
            startGameConfirmationDlg.ToggleRankON.SetActive(state);
            startGameConfirmationDlg.ToggleRankOFF.SetActive(!state);
        }

        void OnToggleRankButtonClicked()
        {
            audioService.PlayStandardClick();
            startGameConfirmationDlg.toggleRankButtonState = !startGameConfirmationDlg.toggleRankButtonState;
            SetToggleRankButtonState(startGameConfirmationDlg.toggleRankButtonState);
        }

        void ShowConfirmGameDlg(FriendBar bar)
        {
            PublicProfile opponentProfile = bar.friendInfo.publicProfile;
            startGameConfirmationDlg.opponentProfilePic.sprite = null;
            startGameConfirmationDlg.opponentActivityText.text = "";
 
            if (!bar.friendInfo.publicProfile.isOnline && bar.friendInfo.publicProfile.isActive)
            {
                startGameConfirmationDlg.onlineStatus.sprite = active;
            }
            else
            {
                startGameConfirmationDlg.onlineStatus.sprite = bar.friendInfo.publicProfile.isOnline ? online : offline;
            }

            //backendService.FriendsOpStatus(bar.friendInfo.playerId);

            if (bar.avatarImage != null)
            {
                startGameConfirmationDlg.opponentProfilePic.sprite = bar.avatarImage.sprite;
            }
            if (bar.avatarIcon != null)
            {
                startGameConfirmationDlg.opponentAvatarIcon.sprite = bar.avatarIcon.sprite;
                startGameConfirmationDlg.opponentAvatarBg.sprite = bar.avatarBG.sprite;
                startGameConfirmationDlg.opponentAvatarBg.color = bar.avatarBG.color;
                startGameConfirmationDlg.opponentAvatarIcon.gameObject.SetActive(bar.avatarIcon.IsActive());
                startGameConfirmationDlg.opponentAvatarBg.gameObject.SetActive(bar.avatarBG.IsActive());
            }
            startGameConfirmationDlg.opponentProfileName.text = opponentProfile.name;
            startGameConfirmationDlg.opponentEloLabel.text = eloPrefix + " " + opponentProfile.eloScore;
            startGameConfirmationDlg.opponentFlag.sprite = Flags.GetFlag(opponentProfile.countryId);

            startGameConfirmationDlg.toggleRankButtonState = true;
            SetToggleRankButtonState(startGameConfirmationDlg.toggleRankButtonState);

            startGameConfirmationDlg.playerId = bar.friendInfo.playerId;
            //startGameConfirmationDlg.premiumBorder.SetActive(bar.premiumBorder.activeSelf);
            startGameConfirmationDlg.leagueBorder.gameObject.SetActive(bar.leagueBorder.gameObject.activeSelf);
            startGameConfirmationDlg.leagueBorder.sprite = bar.leagueBorder.sprite;

            startGameConfirmationDlg.gameObject.SetActive(true);
        }

        void ConfirmRankedGameBtnClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            CreateGame(actionBar.friendInfo.playerId, startGameConfirmationDlg.toggleRankButtonState);
        }

        void ConfirmFriendlyGameBtnClicked(string actionCode)
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            CreateQuickMatchGame(actionBar.friendInfo.playerId, startGameConfirmationDlg.toggleRankButtonState, actionCode);
        }

        void ToolTipBtnClicked()
        {
            startGameConfirmationDlg.tooltip.SetActive(!startGameConfirmationDlg.tooltip.activeSelf);
        }

        void ConfirmNewGameDlgNo()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        void CloseNewGameDlg(string friendId)
        {
            if (startGameConfirmationDlg.gameObject.activeSelf && actionBar != null && actionBar.friendInfo.playerId == friendId)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            }
        }

        public void HideStartGameDlg()
        {
            startGameConfirmationDlg.gameObject.SetActive(false);
        }

        public void UpdateStartGameConfirmationDlg(ProfileVO vo)
        {
            TLUtils.LogUtil.LogNullValidation(vo.playerId, "friendId");

            if (vo.playerId != null && startGameConfirmationDlg.playerId != vo.playerId)
            {
                return;
            }

            if (!vo.isOnline && vo.isActive)
            {
                startGameConfirmationDlg.onlineStatus.sprite = active;
            }
            else
            {
                startGameConfirmationDlg.onlineStatus.sprite = vo.isOnline ? online : offline;
            }


            //if (vo.isOnline)
            //{
            //    startGameConfirmationDlg.opponentActivityText.text = vo.activity;
            //}
            //else
            //{
            //    startGameConfirmationDlg.opponentActivityText.text = "";
            //}
        }

        #endregion StartGameConfirmationDialog

        public void CancelSearchResult()
        {
            if (sectionSearched.gameObject.activeSelf)
            {
                OnCancelSearchClicked();
            }
        }

        public void OnCloseButtonClickedCreateMatchLimitDlg()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        public void HideCreateMatchLimitDlg()
        {
            createMatchLimitReachedDlg.SetActive(false);
        }

        public void ShowCreateMatchLimitReacDlg()
        {
            createMatchLimitReachedDlg.SetActive(true);
        }

        #region FindYourFriendDialog
        public void ShowFriendsHelpDialog()
        {
            findFriendDlg.SetActive(true);
        }

        public void HideFriendsHelpDialog()
        {
            findFriendDlg.SetActive(false);
        }

        public void FriendsHelpDialogCloseButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
        #endregion

        public void ShowProcessing(bool show, bool showProcessingUi)
        {
            processingUi.SetActive(showProcessingUi);
            uiBlocker.SetActive(show);
        }

        private void OnManageBlockedFriendsClicked()
        {
            manageBlockedFriendsButtonClickedSignal.Dispatch();
            audioService.PlayStandardClick();
        }

        private void OnSignInWithAppleClicked()
        {
            signInWithAppleClicked.Dispatch();
            facebookConnectAnim.SetActive(true);
        }
    }
}
