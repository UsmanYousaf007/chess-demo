/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;
using System.Collections.Generic;
//using TurboLabz.Multiplayer;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System;

namespace TurboLabz.InstantFramework
{
    public class LobbyMediator : Mediator
    {
        // View injection
        [Inject] public LobbyView view { get; set; }

        // Dispatch signals
        [Inject] public AdjustStrengthSignal adjustStrengthSignal { get; set; }
        [Inject] public AuthFaceBookSignal authFacebookSignal { get; set; }
        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }
        [Inject] public ShowProfileDialogSignal showProfileDialogSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public ShareAppSignal shareAppSignal { get; set; }
        [Inject] public TapLongMatchSignal tapLongMatchSignal { get; set; }
        [Inject] public SetActionCountSignal setActionCountSignal { get; set; }
        [Inject] public AcceptSignal acceptSignal { get; set; }
        [Inject] public DeclineSignal declineSignal { get; set; }
        [Inject] public CloseStripSignal closeStripSignal { get; set; }
        [Inject] public TurboLabz.Multiplayer.ResignSignal resignSignal { get; set; }
        [Inject] public RemoveCommunityFriendSignal removeCommunityFriendSignal { get; set; }
        [Inject] public LoadStatsSignal loadStatsSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public LoadChatSignal loadChatSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.playMultiplayerButtonClickedSignal.AddListener(OnQuickMatchBtnClicked);
            view.playMultiplayerClassicButtonClickedSignal.AddListener(OnClassicMatchBtnClicked);
            view.playCPUButtonClickedSignal.AddListener(OnPlayComputerMatchBtnClicked);
            view.upgradeToPremiumButtonClickedSignal.AddListener(OnUpgradeToPremiumClicked);

            view.facebookButtonClickedSignal.AddListener(OnFacebookButtonClicked);
            view.reloadFriendsSignal.AddOnce(OnReloadFriends);
            view.showProfileDialogSignal.AddListener(OnShowProfileDialog);
            view.refreshCommunityButton.onClick.AddListener(OnRefreshCommunity);
            //view.defaultInviteFriendsButton.onClick.AddListener(OnShareApp);
            view.playButtonClickedSignal.AddListener(OnPlayButtonClicked);
            view.quickMatchFriendButtonClickedSignal.AddListener(OnQuickMatchFriendButtonClicked);
            view.actionCountUpdatedSignal.AddListener(OnActionCountUpdated);
            view.acceptButtonClickedSignal.AddListener(OnAcceptButtonClicked);
            view.declineButtonClickedSignal.AddListener(OnDeclineButtonClicked);
            view.cancelButtonClickedSignal.AddListener(OnCancelButtonClicked);
            view.okButtonClickedSignal.AddListener(OnOkButtonClicked);
            view.removeCommunityFriendSignal.AddListener(OnRemoveCommunityFriend);
            view.decStrengthButtonClickedSignal.AddListener(OnDecStrengthButtonClicked);
            view.incStrengthButtonClickedSignal.AddListener(OnIncStrengthButtonClicked);
            view.showChatSignal.AddListener(OnShowChat);
        }

        private void OnDecStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(false);
        }

        private void OnIncStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(true);
        }

        [ListensTo(typeof(UpdateStrengthSignal))]
        public void OnUpdateStrength(LobbyVO vo)
        {
            view.UpdateStrength(vo);
        }

        [ListensTo(typeof(UpdateMenuViewSignal))]
        public void OnUpdateView(LobbyVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.lobby, facebookService.isLoggedIn());
            }
            if (viewId == NavigatorViewId.CREATE_MATCH_LIMIT_REACHED_DIALOG)
            {
                view.ShowCreateMatchLimitReacDlg();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Hide();
            }

            if (viewId == NavigatorViewId.CREATE_MATCH_LIMIT_REACHED_DIALOG)
            {
                view.HideCreateMatchLimitDlg();
            }
        }

        [ListensTo(typeof(AddFriendsSignal))]
        public void OnUpdateFriends(Dictionary<string, Friend> friends, FriendCategory friendCategory)
        {
            bool isCommunity = false;
            bool isSearched = false;
            if (friendCategory == FriendCategory.COMMUNITY)
            {
                isCommunity = true;
            }
            else if (friendCategory == FriendCategory.SEARCHED)
            {
                isCommunity = true;
                isSearched = true;
                return;
            }

            view.AddFriends(friends, isCommunity, isSearched);
        }

        [ListensTo(typeof(NewFriendAddedSignal))]
        public void OnNewFriendAdded(string friendId)
        {
            view.NewFriendAdded(friendId);
        }

        [ListensTo(typeof(UpdateFriendPicSignal))]
        public void OnUpdateFriendPic(string playerId, Sprite sprite)
        {
            view.UpdateFriendPic(playerId, sprite);
        }

        [ListensTo(typeof(UpdateEloScoresSignal))]
        public void OnUpdateEloScoresSignal(EloVO vo)
        {
            view.UpdateEloScores(vo);
        }

        [ListensTo(typeof(UpdateFriendBarStatusSignal))]
        public void OnUpdateFriendBarStatus(LongPlayStatusVO vo)
        {
            view.UpdateFriendBarStatus(vo);
        }

        [ListensTo(typeof(UpdateFriendOnlineStatusSignal))]
        public void OnUpdateFriendOnlineStatusSignal(ProfileVO vo)
        {
            view.UpdateFriendOnlineStatusSignal(vo);
            view.UpdateStartGameConfirmationDlg(vo);
        }

        [ListensTo(typeof(FriendBarBusySignal))]
        public void OnFriendBarBusy(string playerId, bool busy, CreateLongMatchAbortReason reason)
        {
            view.UpdateFriendBarBusy(playerId, busy, reason);
        }

        [ListensTo(typeof(SortFriendsSignal))]
        public void OnSortFriends()
        {
            
            view.SortFriends();
        }

        [ListensTo(typeof(SortCommunitySignal))]
        public void OnSortCommunity()
        {
            view.SortCommunity();
        }

        [ListensTo(typeof(ClearCommunitySignal))]
        public void OnClearCommunity()
        {
            view.ClearCommunity();
        }

        [ListensTo(typeof(ClearFriendsSignal))]
        public void OnClearFriends()
        {
            view.ClearFriends();
        }

        [ListensTo(typeof(ClearFriendSignal))]
        public void OnClearFriend(string friendId)
        {
            view.ClearFriend(friendId);
        }

        [ListensTo(typeof(AddUnreadMessagesToBarSignal))]
        public void OnAddUnreadMessages(string friendId, int messagesCount)
        {
            view.AddUnreadMessages(friendId, messagesCount);
        }

        [ListensTo(typeof(ClearUnreadMessagesFromBarSignal))]
        public void OnClearUnreadMessages(string friendId)
        {
            view.ClearUnreadMessages(friendId);
        }

        [ListensTo(typeof(PlayerProfilePicTappedSignal))]
        public void OnPlayerProfileButtonTapped()
        {
            if (gameObject.activeSelf)
            {
                loadStatsSignal.Dispatch();
            }
        }

        [ListensTo(typeof(ChessboardBlockerEnableSignal))]
        public void OnUIBlockerEnable(bool enable)
        {
            if (gameObject.activeSelf)
            {
                view.uiBlocker.SetActive(enable);
            }
        }

        [ListensTo(typeof(SkinUpdatedSignal))]
        public void OnSkinUpdated()
        {
            view.UpdateBarsSkin();
        }

        private void OnFacebookButtonClicked()
        {
            authFacebookSignal.Dispatch();
        }

        private void OnReloadFriends()
        {
            loadFriendsSignal.Dispatch();
        }

        private void OnShowProfileDialog(string playerId, FriendBar bar)
        {
            showProfileDialogSignal.Dispatch(playerId, bar);
        }

        private void OnShowChat(string playerId)
        {
            loadChatSignal.Dispatch(playerId, false);
        }

        private void OnRefreshCommunity()
        {
            refreshCommunitySignal.Dispatch();

            // Analytics
            analyticsService.Event(AnalyticsEventId.refresh_community);
        }

        private void OnShareApp()
        {
            shareAppSignal.Dispatch();
        }

        private void OnPlayButtonClicked(string playerId, bool isRanked)
        {
            if (!playerModel.isPremium)
            {
                if (CanShowPregameAd())
                {
                    playerModel.adContext = AnalyticsContext.interstitial_pregame;
                    ResultAdsVO vo = new ResultAdsVO();
                    vo.adsType = AdType.Interstitial;
                    vo.isRanked = isRanked;
                    vo.friendId = playerId;
                    vo.actionCode = "ChallengeClassic";
                    analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
                    showAdSignal.Dispatch(vo);
                    return;
                }
            }
            tapLongMatchSignal.Dispatch(playerId, isRanked);
        }

        private void OnQuickMatchFriendButtonClicked(string playerId, bool isRanked, string actionCode)
        {
            var friend = playerModel.GetFriend(playerId);

            if (!playerModel.isPremium) 
            {
                if (CanShowPregameAd())
                {
                    playerModel.adContext = AnalyticsContext.interstitial_pregame;
                    ResultAdsVO vo = new ResultAdsVO();
                    vo.adsType = AdType.Interstitial;
                    vo.actionCode = actionCode;
                    vo.friendId = playerId;
                    vo.isRanked = isRanked;
                    analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
                    showAdSignal.Dispatch(vo);
                    return;
                }
             }

            FindMatchAction.Challenge(findMatchSignal, isRanked, playerId, actionCode);
        }

        private void OnAcceptButtonClicked(string playerId)
        {
            acceptSignal.Dispatch(playerId);
        }

        private void OnDeclineButtonClicked(string playerId)
        {
            declineSignal.Dispatch(playerId);
        }

        private void OnCancelButtonClicked(string playerId)
        {
            resignSignal.Dispatch(playerId);
        }

        private void OnOkButtonClicked(string playerId)
        {
            closeStripSignal.Dispatch(playerId);
        }

        private void OnActionCountUpdated(int count)
        {
            setActionCountSignal.Dispatch(count);
        }

        private void OnRemoveCommunityFriend(string opponentId)
        {
            removeCommunityFriendSignal.Dispatch(opponentId);
        }

        private void OnPlayComputerMatchBtnClicked()
        {
            if (!playerModel.isPremium)
            {
                if (CanShowPregameAd())
                {
                    playerModel.adContext = AnalyticsContext.interstitial_pregame;
                    ResultAdsVO vo = new ResultAdsVO();
                    vo.adsType = AdType.Interstitial;
                    analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
                    showAdSignal.Dispatch(vo);
                    return;
                }
            }
            startCPUGameSignal.Dispatch();
        }

        private void OnQuickMatchBtnClicked(string actionCode)
        {
            if (!playerModel.isPremium)
            {
                if (CanShowPregameAd())
                {
                    playerModel.adContext = AnalyticsContext.interstitial_pregame;
                    ResultAdsVO vo = new ResultAdsVO();
                    vo.adsType = AdType.Interstitial;
                    vo.actionCode = actionCode;
                    analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
                    showAdSignal.Dispatch(vo);
                    return;
                }
            }

            FindMatchAction.Random(findMatchSignal, actionCode);
        }

        private void OnClassicMatchBtnClicked()
        {
            if (!playerModel.isPremium)
            {
                if (CanShowPregameAd())
                {
                    playerModel.adContext = AnalyticsContext.interstitial_pregame;
                    ResultAdsVO vo = new ResultAdsVO();
                    vo.adsType = AdType.Interstitial;
                    vo.actionCode = FindMatchAction.ActionCode.RandomLong.ToString();
                    showAdSignal.Dispatch(vo);
                    return;
                }
            }
            
            //analyticsService.Event("classic_" + AnalyticsEventId.match_find_random, AnalyticsContext.start_attempt);
            FindMatchAction.RandomLong(findMatchSignal);
        }

        private bool CanShowPregameAd()
        {
            bool retVal = false;

            IPreferencesModel preferencesModel = view.preferencesModel;
            IAdsSettingsModel adsSettingsModel = view.adsSettingsModel;

            double minutesBetweenLastAdShown = (DateTime.Now - preferencesModel.intervalBetweenPregameAds).TotalMinutes;

            if (preferencesModel.sessionsBeforePregameAdCount > adsSettingsModel.sessionsBeforePregameAd &&
                preferencesModel.pregameAdsPerDayCount < adsSettingsModel.maxPregameAdsPerDay &&
                (preferencesModel.intervalBetweenPregameAds == DateTime.MaxValue || (preferencesModel.intervalBetweenPregameAds != DateTime.MaxValue &&
                minutesBetweenLastAdShown >= adsSettingsModel.intervalsBetweenPregameAds)))
            {
                retVal = true;
            }

            return retVal;
        }

        [ListensTo(typeof(ShowPromotionSignal))]
        public void OnShowPromotion(PromotionVO vo)
        {
            view.ShowPromotion(vo);
        }

        [ListensTo(typeof(ShowCoachTrainingDailogueSignal))]
        public void OnShowCoachTrainingDailogue()
        {
            view.ShowCoachTrainingDailogue();
        }

        [ListensTo(typeof(ShowStrengthTrainingDailogueSignal))]
        public void OnShowStrengthTrainingDailogue()
        {
            view.ShowStrengthTrainingDailogue();
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnRemoveLobbyPromotion(StoreItem item)
        {
            view.RemovePromotion(item.key);
        }

        [ListensTo(typeof(ShowProcessingSignal))]
        public void OnShowProcessingUI(bool show, bool showProcessingUi)
        {
            view.ShowProcessing(show, showProcessingUi);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.SetPriceOfIAPBanner(isAvailable);
        }

        [ListensTo(typeof(RewardUnlockedSignal))]
        public void OnRewardUnlocked(string key, int quantity)
        {
            view.OnRewardUnlocked(key, quantity);
        }

        [ListensTo(typeof(ShowAdSkippedDlgSignal))]
        public void OnShowAdSkippedDlg()
        {
            view.ShowAdSkippedDailogue(true);
        }

        void OnUpgradeToPremiumClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
            hAnalyticsService.LogEvent("upgrade_subscription_clicked", "menu", "lobby");
        }

        [ListensTo(typeof(UpdateOfferDrawSignal))]
        public void OfferDrawStatusUpdate(OfferDrawVO offerDrawVO)
        {
            if (offerDrawVO.challengeId != view.matchInfoModel.activeChallengeId)
            {
                view.UpdateFriendBarDrawOfferStatus(offerDrawVO.status, offerDrawVO.offeredBy, offerDrawVO.opponentId);
                return;
            }
        }
    }
}

