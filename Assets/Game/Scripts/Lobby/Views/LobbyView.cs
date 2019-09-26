/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public partial class LobbyView : View
    {
        private const long RECENTLY_COMPLETED_THRESHOLD_DAYS = 2;

        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }
        [Inject] public ClearCommunitySignal clearCommunitySignal { get; set; }
        [Inject] public NewFriendSignal newFriendSignal { get; set; }
        [Inject] public SearchFriendSignal searchFriendSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public UpdatePlayerNotificationCountSignal updatePlayerNotificationCountSignal { get; set; }


        private SpritesContainer defaultAvatarContainer;

        public Transform listContainer;
        public GameObject friendBarPrefab;

        public Button quickMatchBtn;
        public Text quickMatchTitleTxt;
        public GameObject quickMatchPlay;
        public Text quickMatchPlayTxt;
        public Text quickMatchDescriptionTxt;

        public Button playComputerMatchBtn;
        public Text playComputerMatchTitleTxt;
        public GameObject playComputerMatchPlay;
        public Text playComputerMatchPlayTxt;
        public Text playComputerMatchDescriptionTxt;
        public Text playComputerLevelTxt;

        public Text noActiveMatchesText;
        public Text waitingForPlayersText;

        public Transform sectionActiveMatches;
        public GameObject sectionActiveMatchesEmpty;
        public Transform sectionRecentlyCompletedMatches;
        public Transform sectionPlaySomeoneNew;
        public GameObject sectionPlaySomeoneNewEmpty;

        public Text sectionActiveMatchesTitle;
        public Text sectionPlaySomeoneNewTitle;

        public Text sectionRecentlyCompletedMatchesTitle;

        public Button refreshCommunityButton;
        public Text refreshText;
        public GameObject confirmDlg;

        public ScrollRect scrollRect;
        public GameObject uiBlocker;

        public Image notificationTagImage;
        public Text notificationTagNumber;

        public Text onlinePlayersCountLabel;

        [Header("Choose computer difficulty dialog")]
        public GameObject chooseComputerDifficultyDlg;
        public Button decStrengthButton;
        public Text prevStrengthLabel;
        public Text easyLabel;
        public Text hardLabel;
        public Text currentStrengthLabel;
        public Text nextStrengthLabel;
        public Button incStrengthButton;
        public Button computerDifficultyDlgStartGameButton;
        public Button computerDifficultyDlgCloseButton;

        [Header("Confirm new game dialog")]
        public GameObject confirmNewGameDlg;
        public Button confirmGameCloseBtn;
        public Text confirmNewGameDlgTitleText;
        public Image opponentProfilePic;
        public Image opponentAvatarBg;
        public Image opponentAvatarIcon;
        public Text opponentProfileName;
        public Text opponentEloLabel;
        public Image opponentFlag;
        public Button confirmRankedGameBtn;
        public Text confirmRankedGameBtnText;
        public Button confirmFriendlyGameBtn;
        public Text confirmFriendlyGameBtnText;
        public Text classicGameTimeText;

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


        public Signal facebookButtonClickedSignal = new Signal();
        public Signal reloadFriendsSignal = new Signal();
        public Signal<string> showProfileDialogSignal = new Signal<string>();
        public Signal<string, bool> playButtonClickedSignal = new Signal<string, bool>();
        public Signal<string> acceptButtonClickedSignal = new Signal<string>();
        public Signal<string> declineButtonClickedSignal = new Signal<string>();
        public Signal<string> cancelButtonClickedSignal = new Signal<string>();
        public Signal<string> okButtonClickedSignal = new Signal<string>();
        public Signal<int> actionCountUpdatedSignal = new Signal<int>();
        public Signal<string> removeCommunityFriendSignal = new Signal<string>();
        public Signal playCPUButtonClickedSignal = new Signal();
        public Signal playMultiplayerButtonClickedSignal = new Signal();
        public Signal decStrengthButtonClickedSignal = new Signal();
        public Signal incStrengthButtonClickedSignal = new Signal();

        private Dictionary<string, FriendBar> bars = new Dictionary<string, FriendBar>();
        private List<GameObject> defaultInvite = new List<GameObject>();
        private FriendBar actionBar;
        private string eloPrefix;
        private string startGameFriendId;
        private bool startGameRanked;
        private List<GameObject> cacheEnabledSections;
        private bool isCPUGameInProgress;
        List<FriendBar> recentlyCompleted = new List<FriendBar>();

        public void Init()
        {
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            noActiveMatchesText.text = localizationService.Get(LocalizationKey.FRIENDS_SECTION_ACTIVE_MATCHES_EMPTY);
            waitingForPlayersText.text = localizationService.Get(LocalizationKey.FRIENDS_WAITING_FOR_PLAYERS);
            quickMatchPlayTxt.text = localizationService.Get(LocalizationKey.PLAY);
            playComputerMatchPlayTxt.text = localizationService.Get(LocalizationKey.PLAY);
            refreshText.text = localizationService.Get(LocalizationKey.FRIENDS_REFRESH_TEXT);

            sectionActiveMatchesTitle.text = localizationService.Get(LocalizationKey.FRIENDS_SECTION_ACTIVE_MATCHES);
            
            sectionPlaySomeoneNewTitle.text = localizationService.Get(LocalizationKey.FRIENDS_SECTION_PLAY_SOMEONE_NEW);
            sectionRecentlyCompletedMatchesTitle.text = localizationService.Get(LocalizationKey.FRIENDS_SECTION_RECENTLY_COMPLETED_MATCHES);

            confirmRankedGameBtnText.text = localizationService.Get(LocalizationKey.NEW_GAME_CONFIRM_RANKED);
            confirmFriendlyGameBtnText.text = localizationService.Get(LocalizationKey.NEW_GAME_CONFIRM_FRIENDLY);
            confirmNewGameDlgTitleText.text = localizationService.Get(LocalizationKey.NEW_GAME_CONFIRM_TITLE);
            eloPrefix = localizationService.Get(LocalizationKey.ELO_SCORE);

            removeCommunityFriendYesBtnText.text = localizationService.Get(LocalizationKey.REMOVE_COMMUNITY_FRIEND_YES);
            removeCommunityFriendNoBtnText.text = localizationService.Get(LocalizationKey.REMOVE_COMMUNITY_FRIEND_NO);
            removeCommunityFriendTitleText.text = localizationService.Get(LocalizationKey.REMOVE_COMMUNITY_FRIEND_TITLE);

            confirmRankedGameBtn.onClick.AddListener(ConfirmRankedGameBtnClicked);
            confirmFriendlyGameBtn.onClick.AddListener(ConfirmFriendlyGameBtnClicked);
            confirmGameCloseBtn.onClick.AddListener(ConfirmNewGameDlgNo);

            removeCommunityFriendYesBtn.onClick.AddListener(RemoveCommunityFriendDlgYes);
            removeCommunityFriendNoBtn.onClick.AddListener(RemoveCommunityFriendDlgNo);

            createMatchLimitReachedCloseBtn.onClick.AddListener(CreateMatchLimitReachedCloseBtnClicked);

            quickMatchTitleTxt.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY_ONLINE);

            playComputerMatchTitleTxt.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY_CPU);
            playComputerMatchDescriptionTxt.text = localizationService.Get(LocalizationKey.CPU_MENU_SINGLE_PLAYER_GAME);
            classicGameTimeText.text = localizationService.Get(LocalizationKey.CLASSIC_MODE_TIME);

            quickMatchBtn.onClick.AddListener(OnQuickMatchBtnClicked);

            playComputerMatchBtn.onClick.AddListener(OnPlayComputerMatchBtnClicked);

            decStrengthButton.onClick.AddListener(OnDecStrengthButtonClicked);
            incStrengthButton.onClick.AddListener(OnIncStrengthButtonClicked);
            computerDifficultyDlgStartGameButton.onClick.AddListener(OnComputerDifficultyDlgStartGameClicked);
            computerDifficultyDlgCloseButton.onClick.AddListener(OnComputerDifficultyDlgCloseClicked);
            easyLabel.text = localizationService.Get(LocalizationKey.EASY);
            hardLabel.text = localizationService.Get(LocalizationKey.HARD);
            notificationTagImage.gameObject.SetActive(false);


            cacheEnabledSections = new List<GameObject>();
        }

        void OnDecStrengthButtonClicked()
        {
            decStrengthButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        private void OnIncStrengthButtonClicked()
        {
            incStrengthButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        public void UpdateView(LobbyVO vo)
        {
            if (uiBlocker.activeSelf)
            {
                uiBlocker.SetActive(false);
            }

            UpdateStrength(vo);
            if (vo.inProgress)
            {
                playComputerLevelTxt.gameObject.SetActive(true);
                playComputerLevelTxt.text = localizationService.Get(LocalizationKey.PLAYING_LEVEL ) + vo.selectedStrength;
                playComputerMatchPlay.SetActive(false);
               
            }
            else
            {
                playComputerLevelTxt.gameObject.SetActive(false);
                playComputerMatchPlay.SetActive(true);
            }

            onlinePlayersCountLabel.text = "Active Players " + vo.onlineCount.ToString();

    }

    public void UpdateStrength(LobbyVO vo)
        {
            isCPUGameInProgress = vo.inProgress;
            int selectedStrength = vo.selectedStrength;
            int minStrength = vo.minStrength;
            int maxStrength = vo.maxStrength;

            UpdateStrength(selectedStrength,minStrength,maxStrength);
        }

        private void UpdateStrength(int selectedStrength, int minStrength, int maxStrength)
        {
            currentStrengthLabel.text = selectedStrength.ToString();

            if (selectedStrength == minStrength)
            {
                prevStrengthLabel.text = "";
                nextStrengthLabel.text = (selectedStrength + 1).ToString();
                incStrengthButton.interactable = true;
                decStrengthButton.interactable = false;
            }
            else if (selectedStrength == maxStrength)
            {
                prevStrengthLabel.text = (selectedStrength - 1).ToString();
                nextStrengthLabel.text = "";
                incStrengthButton.interactable = false;
                decStrengthButton.interactable = true;
            }
            else
            {
                prevStrengthLabel.text = (selectedStrength - 1).ToString();
                nextStrengthLabel.text = (selectedStrength + 1).ToString();
                incStrengthButton.interactable = true;
                decStrengthButton.interactable = true;
            }
        }

        void OnQuickMatchBtnClicked()
        {
            Debug.Log("OnQuickMatchBtnClicked");
            playMultiplayerButtonClickedSignal.Dispatch();
        }

        void OnPlayComputerMatchBtnClicked()
        {
            Debug.Log("OnPlayComputerMatchBtnClicked");
            if (!isCPUGameInProgress)
            {
                chooseComputerDifficultyDlg.SetActive(true);
            }
            else
            {
                playCPUButtonClickedSignal.Dispatch();
            }
        }

        void OnComputerDifficultyDlgCloseClicked()
        {
            chooseComputerDifficultyDlg.SetActive(false) ;
        }

        void OnComputerDifficultyDlgStartGameClicked()
        {
            playCPUButtonClickedSignal.Dispatch();
        }

        void CacheEnabledSections()
        {
            cacheEnabledSections.Clear();

            if (sectionActiveMatches.gameObject.activeSelf) cacheEnabledSections.Add(sectionActiveMatches.gameObject);
            if (sectionActiveMatchesEmpty.gameObject.activeSelf) cacheEnabledSections.Add(sectionActiveMatchesEmpty);
            if (sectionRecentlyCompletedMatches.gameObject.activeSelf) cacheEnabledSections.Add(sectionRecentlyCompletedMatches.gameObject);
            if (sectionPlaySomeoneNew.gameObject.activeSelf) cacheEnabledSections.Add(sectionPlaySomeoneNew.gameObject);
            if (sectionPlaySomeoneNewEmpty.gameObject.activeSelf) cacheEnabledSections.Add(sectionPlaySomeoneNewEmpty);
        }

        public void CreateGame(string friendId, bool isRanked)
        {
            TLUtils.LogUtil.LogNullValidation(friendId, "friendId");

            // Friend needs to be added
            if (friendId != null && !bars.ContainsKey(friendId))
            {
                startGameFriendId = friendId;
                startGameRanked = isRanked;
                newFriendSignal.Dispatch(friendId);
                return;
            }

            // Start this game
            if (friendId != null)
            {
                playButtonClickedSignal.Dispatch(friendId, isRanked);
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
        }

        void AddFriend(Friend friend, bool isCommunity, bool isSearched)
        {
            TLUtils.LogUtil.LogNullValidation(friend.playerId, "friend.playerId");
            
            if (friend.playerId != null && bars.ContainsKey(friend.playerId))
            {
                return;
            }

            // create bar
            GameObject friendBarObj = Instantiate(friendBarPrefab);
            SkinLink[] objects = friendBarObj.GetComponentsInChildren<SkinLink>();
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].InitPrefabSkin();
            }

            // update bar values
            FriendBar friendBar = friendBarObj.GetComponent<FriendBar>();
            friendBar.Init(localizationService);
            friendBar.lastMatchTimeStamp = friend.lastMatchTimestamp;
            friendBar.viewProfileButton.onClick.AddListener(() => ViewProfile(friend.playerId));
            friendBar.stripButton.onClick.AddListener(() => PlayButtonClicked(friendBar));
            friendBar.acceptButton.onClick.AddListener(() => AcceptButtonClicked(friend.playerId, friendBar.acceptButton));
            friendBar.notNowButton.onClick.AddListener(() => DeclineButtonClicked(friend.playerId, friendBar.notNowButton));
            friendBar.cancelButton.onClick.AddListener(() => CancelButtonClicked(friend.playerId, friendBar.cancelButton));
            friendBar.okButton.onClick.AddListener(() => OkButtonClicked(friend.playerId, friendBar.okButton));
            friendBar.viewButton.onClick.AddListener(() => PlayButtonClicked(friendBar));
            friendBar.removeCommunityFriendButton.onClick.AddListener(() => RemoveCommunityFriendButtonClicked(friend.playerId));
            friendBar.friendInfo = friend;
            friendBar.profileNameLabel.text = friend.publicProfile.name;
            friendBar.eloScoreLabel.text = friend.publicProfile.eloScore.ToString();
            friendBar.isCommunity = isCommunity;
            friendBar.isSearched = isSearched;
            friendBar.isCommunityFriend = friend.friendType == Friend.FRIEND_TYPE_COMMUNITY;
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
                if (publicProfile.avatarId!= null)
                {
                    barData.avatarIcon.gameObject.SetActive(true);
                    barData.avatarBG.gameObject.SetActive(true);

                    barData.avatarBG.color = Colors.Color(publicProfile.avatarBgColorId);
                    barData.avatarIcon.sprite = defaultAvatarContainer.GetSprite(publicProfile.avatarId) ;
                }
            }
        }

        public void UpdateEloScores(EloVO vo)
        {
            TLUtils.LogUtil.LogNullValidation(vo.opponentId, "vo.opponentId");

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
            friendBar.UpdateStatus();

            if (recentlyCompleted.Contains(friendBar))
            {
                friendBar.removeCommunityFriendButton.gameObject.SetActive(false);
            }

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
        
            if (friendId != null && !bars.ContainsKey(friendId))
            {
                return;
            }

            FriendBar friendBar = bars[friendId].GetComponent<FriendBar>();
            friendBar.onlineStatus.sprite = isOnline ? friendBar.online : friendBar.activeStatus;
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

            if (reason == CreateLongMatchAbortReason.LimitReached)
            {
                createMatchLimitReachedDlg.SetActive(true);
                friendBar.playArrow.SetActive(true);
                friendBar.playArrowButton.SetActive(false);
            }
            else if (reason == CreateLongMatchAbortReason.CreateFailed)
            {
                friendBar.playArrow.SetActive(true);
                friendBar.playArrowButton.SetActive(false);
            }
            else // Match successfully created
            {
                friendBar.playArrow.SetActive(false);
                friendBar.playArrowButton.SetActive(false);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            confirmNewGameDlg.SetActive(false);
            removeCommunityFriendDlg.SetActive(false);
            createMatchLimitReachedDlg.SetActive(false);
            chooseComputerDifficultyDlg.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void ClearCommunity()
        {
            ClearType(FriendCategory.COMMUNITY);
            waitingForPlayersText.gameObject.SetActive(true);
        }

        public void ClearFriends()
        {
            ClearType(FriendCategory.FRIEND);
            DefaultInviteSetActive(true);
        }

        public void ClearFriend(string friendId)
        {
            TLUtils.LogUtil.LogNullValidation(friendId, "friendId");

            if (friendId != null && bars.ContainsKey(friendId))
            {
                Destroy(bars[friendId].gameObject);
                bars.Remove(friendId);
            }
        }

        public void AddUnreadMessages(string friendId)
        {
            TLUtils.LogUtil.LogNullValidation(friendId, "friendId");

            if (friendId != null && bars.ContainsKey(friendId))
            {
                bars[friendId].unreadChat.SetActive(true);
            }
        }

        public void ClearUnreadMessages(string friendId)
        {
            TLUtils.LogUtil.LogNullValidation(friendId, "friendId");

            if (friendId != null && bars.ContainsKey(friendId))
            {
                bars[friendId].unreadChat.SetActive(false);
            }
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
                if (friendCategory == FriendCategory.FRIEND)
                {
                    destroyMe.Add(entry.Key);
                }
            }

            foreach (string key in destroyMe)
            {
                Destroy(bars[key].gameObject);
                bars.Remove(key);
            }

            Resources.UnloadUnusedAssets();
        }

        void DefaultInviteSetActive(bool active)
        {
            foreach (GameObject obj in defaultInvite)
            {
                obj.SetActive(active);
            }
        }

        void ViewProfile(string playerId)
        {
            audioService.PlayStandardClick();
            showProfileDialogSignal.Dispatch(playerId);
        }

        void PlayButtonClicked(FriendBar bar)
        {
            audioService.PlayStandardClick();
            actionBar = null;

            if (bar.longPlayStatus == LongPlayStatus.DEFAULT)
            {
                actionBar = bar;
                ShowConfirmGameDlg(actionBar);
            }
            else
            {
                playButtonClickedSignal.Dispatch(bar.friendInfo.playerId, bar.isRanked);
            }
        }


        void RemoveCommunityFriendButtonClicked(string playerId)
        {
            audioService.PlayStandardClick();
            actionBar = bars[playerId];
            removeCommunityFriendDlg.SetActive(true);
        }

        void RemoveCommunityFriendDlgYes()
        {
            removeCommunityFriendDlg.SetActive(false);
            removeCommunityFriendSignal.Dispatch(actionBar.friendInfo.playerId);
            analyticsService.Event(AnalyticsEventId.tap_long_match_remove);
        }

        void RemoveCommunityFriendDlgNo()
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

        void ShowConfirmGameDlg(FriendBar bar)
        {
            PublicProfile opponentProfile = bar.friendInfo.publicProfile;
            opponentProfilePic.sprite = null;

            if (bar.avatarImage != null)
            {
                opponentProfilePic.sprite = bar.avatarImage.sprite;
            }
            if(bar.avatarIcon!= null)
            {
                opponentAvatarIcon.sprite = bar.avatarIcon.sprite;
                opponentAvatarBg.sprite = bar.avatarBG.sprite;
                opponentAvatarBg.color = bar.avatarBG.color;
                opponentAvatarIcon.gameObject.SetActive(bar.avatarIcon.IsActive());
                opponentAvatarBg.gameObject.SetActive(bar.avatarBG.IsActive());
            }
            opponentProfileName.text = opponentProfile.name;
            opponentEloLabel.text = eloPrefix + " " + opponentProfile.eloScore;
            opponentFlag.sprite = Flags.GetFlag(opponentProfile.countryId);

            confirmNewGameDlg.SetActive(true);
        }

        void ConfirmRankedGameBtnClicked()
        {
            confirmNewGameDlg.SetActive(false);
            CreateGame(actionBar.friendInfo.playerId, true);
        }

        void ConfirmFriendlyGameBtnClicked()
        {
            confirmNewGameDlg.SetActive(false);
            CreateGame(actionBar.friendInfo.playerId, false);
        }

        void ConfirmNewGameDlgNo()
        {
            confirmNewGameDlg.SetActive(false);
        }

        void CloseNewGameDlg(string friendId)
        {
            if (confirmNewGameDlg.activeSelf && actionBar != null && actionBar.friendInfo.playerId == friendId)
            {
                confirmNewGameDlg.SetActive(false);
            }
        }

        void CreateMatchLimitReachedCloseBtnClicked()
        {
            createMatchLimitReachedDlg.SetActive(false);
        }


        // Copied that from friendss view sort

        public void SortFriends()
        {
            // Create holders
            recentlyCompleted = new List<FriendBar>();
            List<FriendBar> emptyOffline = new List<FriendBar>();
            List<FriendBar> activeMatches = new List<FriendBar>();

            int notificationCounter = 0;
            notificationTagImage.gameObject.SetActive(false);

            // Fill holders
            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                LongPlayStatus status = entry.Value.longPlayStatus;
                FriendBar bar = entry.Value;

                if (bar.isCommunity)
                {
                    continue;
                }

                if (status == LongPlayStatus.NEW_CHALLENGE ||
                    status == LongPlayStatus.WAITING_FOR_ACCEPT ||
                    status == LongPlayStatus.PLAYER_TURN ||
                    status == LongPlayStatus.OPPONENT_TURN ||
                    status == LongPlayStatus.DECLINED ||
                    entry.Value.isGameCanceled ||
                    status == LongPlayStatus.PLAYER_WON ||
                    status == LongPlayStatus.OPPONENT_WON ||
                    status == LongPlayStatus.DRAW)
                {
                    activeMatches.Add(bar);

                }
                else if ((bar.lastMatchTimeStamp > 0) &&
                    (bar.lastMatchTimeStamp > (TimeUtil.unixTimestampMilliseconds - (RECENTLY_COMPLETED_THRESHOLD_DAYS * 24 * 60 * 60 * 1000))) &&
                    status == LongPlayStatus.DEFAULT)

                {
                    recentlyCompleted.Add(bar);
                }
                else
                {
                    entry.Value.gameObject.SetActive(false);
                }

                
                if (bar.isPlayerTurn || bar.longPlayStatus == LongPlayStatus.DRAW || bar.longPlayStatus == LongPlayStatus.NEW_CHALLENGE
                        || bar.longPlayStatus == LongPlayStatus.PLAYER_WON || bar.longPlayStatus == LongPlayStatus.OPPONENT_WON)
                {
                    notificationCounter++;
                }
                    
            }

            if (notificationCounter > 0){
                notificationTagImage.gameObject.SetActive(true);
            }

            notificationTagNumber.text = notificationCounter.ToString();
            updatePlayerNotificationCountSignal.Dispatch(notificationCounter);
            
            // Sort holders
            activeMatches.Sort((x, y) => -1 * x.lastActionTime.CompareTo(y.lastActionTime));
            recentlyCompleted.Sort((x, y) => -1 * x.lastMatchTimeStamp.CompareTo(y.lastMatchTimeStamp));
            emptyOffline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));

            // Set sibling indexes
            int index = 0;
            

            sectionActiveMatchesEmpty.gameObject.SetActive(false);

            if (activeMatches.Count > 0)
            {
                int count = 0;
                int maxCount = activeMatches.Count;
                sectionActiveMatches.gameObject.SetActive(true);
                index = sectionActiveMatches.GetSiblingIndex() + 1;
                foreach (FriendBar bar in activeMatches)
                {
                    bar.gameObject.SetActive(true);
                    bar.transform.SetSiblingIndex(index);
                    index++;
                    count++;
                    bar.UpdateMasking(maxCount == count, false);
                }
            }
            else
            {
                sectionActiveMatches.gameObject.SetActive(false);
            }

            if (recentlyCompleted.Count>0)
            {
                int maxCount = 5;
                sectionRecentlyCompletedMatches.gameObject.SetActive(true);
                index = sectionRecentlyCompletedMatches.GetSiblingIndex() + 1;
                for (int i = 0; i< recentlyCompleted.Count; i++)
                {
                    if (i < maxCount)
                    {
                        recentlyCompleted[i].gameObject.SetActive(true);
                        recentlyCompleted[i].transform.SetSiblingIndex(index);
                        recentlyCompleted[i].removeCommunityFriendButton.gameObject.SetActive(false);
                        index++;
                        recentlyCompleted[i].UpdateMasking((maxCount == (i + 1) || recentlyCompleted.Count == (i + 1)), false);
                    }
                    else
                    {
                        recentlyCompleted[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                sectionRecentlyCompletedMatches.gameObject.SetActive(false);
            }

        }

        public void SortCommunity()
        {
            // Create holders
            List<FriendBar> communityOnline = new List<FriendBar>();
            List<FriendBar> communityOffline = new List<FriendBar>();
            
            // Fill holders
            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                FriendBar bar = entry.Value;

                if (!bar.isCommunity)
                {
                    continue;
                }
                if (bar.isSearched)
                {
                    continue;
                }

                if (entry.Value.isOnline)
                {
                    communityOnline.Add(bar);
                }
                else
                {
                    communityOffline.Add(bar);
                }
            }

            // Sort holders
            communityOnline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));
            communityOffline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));

            // Set sibling indexes
            int index = 0;
            sectionPlaySomeoneNewEmpty.gameObject.SetActive(false);
            sectionPlaySomeoneNew.gameObject.SetActive(true);
            int count = 0;
            int maxCount = communityOnline.Count + communityOffline.Count;


            if (communityOnline.Count > 0 || communityOffline.Count > 0)
            {
                index = sectionPlaySomeoneNew.GetSiblingIndex() + 1;

                foreach (FriendBar bar in communityOnline)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                    count++;
                    bar.UpdateMasking(maxCount == count, true);
                }

                foreach (FriendBar bar in communityOffline)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                    count++;
                    bar.UpdateMasking(maxCount == count, true);
                }
            }
            else
            {
                sectionPlaySomeoneNewEmpty.gameObject.SetActive(true);
            }
        }
    }
}
