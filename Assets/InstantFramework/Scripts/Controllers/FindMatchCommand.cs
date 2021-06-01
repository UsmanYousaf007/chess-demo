/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using TurboLabz.InstantGame;
using strange.extensions.command.impl;
using System.Collections;
using TurboLabz.TLUtils;
using GameAnalyticsSDK;

namespace TurboLabz.InstantFramework
{
    public class FindMatchCommand : Command
    {
        // Paramaters
        [Inject] public string action { get; set; }
        private IRoutineRunner routineRunner;

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public ShowFindMatchSignal showFindMatchSignal { get; set; }
        [Inject] public GetGameStartTimeSignal getGameStartTimeSignal { get; set; }
        [Inject] public MatchFoundSignal matchFoundSignal { get; set; }
        [Inject] public UpdateOpponentProfileSignal updateOpponentProfileSignal { get; set; }
        [Inject] public UpdateChatOpponentPicSignal updateChatOpponentPicSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }
        [Inject] public NewFriendSignal newFriendSignal { get; set; }
        [Inject] public MatchAnalyticsSignal matchAnalyticsSignal { get; set; }
        [Inject] public UpdateOfferDrawSignal updateOfferDrawSignal { get; set; }
        [Inject] public ShowProcessingSignal showProcessingSignal { get; set; }
        [Inject] public UpdatePlayerInventorySignal updatePlayerInventorySignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // Listen to signal
        [Inject] public FindMatchCompleteSignal findMatchCompleteSignal { get; set; }
        [Inject] public FindMatchRequestCompleteSignal findMatchRequestCompleteSignal { get; set; }
        [Inject] public FindRandomLongMatchCompleteSignal findRandomLongMatchCompleteSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        public override void Execute()
        {
            Retain();
            routineRunner = new NormalRoutineRunner();
            OfferDrawVO offerDrawVO = new OfferDrawVO();
            offerDrawVO.status = null;
            offerDrawVO.offeredBy = null;
            offerDrawVO.opponentId = null;
            updateOfferDrawSignal.Dispatch(offerDrawVO);

            // This sends the backend request
            backendService.FindMatch(action).Then(HandleFindMatchErrors);
            findRandomLongMatchCompleteSignal.RemoveAllListeners();
            matchAnalyticsSignal.Dispatch(GetFindMatchAnalyticsVO(AnalyticsContext.start_attempt));
            findMatchRequestCompleteSignal.AddOnce(OnFindMatchRequestCompleted);
        }

        private void OnFindMatchRequestCompleted(string opponentStatus)
        {
            if (opponentStatus.Equals("busy"))
            {
                //show dailogue
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CONFIRM_DLG);

                var vo = new ConfirmDlgVO
                {
                    title = localizationService.Get(LocalizationKey.QUICK_MATCH_FAILED),
                    desc = localizationService.Get(LocalizationKey.QUICK_MATCH_FAILED_REASON),
                    yesButtonText = localizationService.Get(LocalizationKey.LONG_PLAY_OK),
                    onClickYesButton = delegate
                    {
                        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                    }
                };

                updateConfirmDlgSignal.Dispatch(vo);
                matchAnalyticsSignal.Dispatch(GetFindMatchAnalyticsVO(AnalyticsContext.failed));
            }
            else
            {
                showFindMatchSignal.Dispatch(action);

                // The actual found match message arrives through a different pipeline
                // from the backend
                findMatchCompleteSignal.AddOnce(OnFindMatchComplete);
                findRandomLongMatchCompleteSignal.AddOnce(OnFindRandomLongMatchComplete);
            }
        }

        private void OnFindRandomLongMatchComplete()
        {
            matchAnalyticsSignal.Dispatch(GetFindMatchAnalyticsVO(AnalyticsContext.success));
        }

        private void OnFindMatchComplete(string challengeId)
        {
            if (matchInfoModel.activeChallengeId == challengeId)
            {
                Release();
                return;
            }

            matchInfoModel.activeChallengeId = challengeId;

            if (matchInfoModel.activeMatch.isBotMatch == false)
            {
                StartGame(challengeId);
            }
            else
            {
                routineRunner.StartCoroutine(WaitBeforeGameStart(challengeId));
            }

            /*matchInfoModel.activeChallengeId = challengeId;

            // Create and fill the opponent profile
            ProfileVO pvo = GetOpponentProfile();

            // Set the opponent info in the game view
            updateOpponentProfileSignal.Dispatch(pvo);

            // Set the finding match view to a found match state
            matchFoundSignal.Dispatch(pvo);

            // add friend
            if (matchInfoModel.activeMatch.isBotMatch == false)
            {
                newFriendSignal.Dispatch(pvo.playerId, false);
            }

            // For quick match games, the flow continues from the get game start time signal
            // where both clients start at a synch time stamp

            getGameStartTimeSignal.Dispatch();

            //Analytics
            preferencesModel.gameStartCount++;
            hAnalyticsService.LogMultiplayerGameEvent(AnalyticsEventId.game_started.ToString(), "gameplay", matchInfoModel.activeMatch.isLongPlay ? "long_match" : "quick_match", challengeId);
            appsFlyerService.TrackLimitedEvent(AnalyticsEventId.game_started, preferencesModel.gameStartCount);
            matchAnalyticsSignal.Dispatch(GetFindMatchAnalyticsVO(matchInfoModel.activeMatch.isBotMatch ? AnalyticsContext.success_bot : AnalyticsContext.success));

            // Grab the opponent profile pic if any
            if (matchInfoModel.activeMatch.opponentPublicProfile.facebookUserId != null)
            {
                PublicProfile opponentPublicProfile = matchInfoModel.activeMatch.opponentPublicProfile;
                if (opponentPublicProfile.facebookUserId != null)
                {
                    facebookService.GetSocialPic(opponentPublicProfile.facebookUserId, opponentPublicProfile.playerId).Then(OnGetOpponentProfilePicture);
                }
            }
            else
            {
                Release();
            }*/
        }

        private void OnGetOpponentProfilePicture(FacebookResult result, Sprite sprite, string facebookUserId)
        {
            // Todo: create a separate signal for just updating the opponent picture.
            if (result == FacebookResult.SUCCESS)
            {
                //in case of abandon it will be null
                if (matchInfoModel.activeMatch != null)
                {
                    matchInfoModel.activeMatch.opponentPublicProfile.profilePicture = sprite;

                    ProfileVO pvo = GetOpponentProfile();
                    updateOpponentProfileSignal.Dispatch(pvo);

                    updateChatOpponentPicSignal.Dispatch(sprite);
                }
            }

            Release();
        }

        int randomVal = 0;

        IEnumerator WaitBeforeGameStart(string challengeId)
        {
            randomVal = Random.Range(0, settingsModel.matchmakingRandomRange);
            yield return new WaitForSeconds(randomVal);
            StartGame(challengeId);
        }

        private void StartGame(string challengeId)
        {
            // Create and fill the opponent profile

            if (matchInfoModel.activeMatch == null)
            {
                return;
            }

            ProfileVO pvo = GetOpponentProfile();

            // Set the opponent info in the game view
            updateOpponentProfileSignal.Dispatch(pvo);

            // Set the finding match view to a found match state
            matchFoundSignal.Dispatch(pvo);

            // add friend
            if (matchInfoModel.activeMatch.isBotMatch == false)
            {
                newFriendSignal.Dispatch(pvo.playerId, false);
            }

            // For quick match games, the flow continues from the get game start time signal
            // where both clients start at a synch time stamp

            getGameStartTimeSignal.Dispatch();

            //Analytics
            preferencesModel.gameStartCount++;
            hAnalyticsService.LogMultiplayerGameEvent(AnalyticsEventId.game_started.ToString(), "gameplay", matchInfoModel.activeMatch.isLongPlay ? "long_match" : "quick_match", challengeId);
            appsFlyerService.TrackLimitedEvent(AnalyticsEventId.game_started, preferencesModel.gameStartCount);
            matchAnalyticsSignal.Dispatch(GetFindMatchAnalyticsVO(matchInfoModel.activeMatch.isBotMatch ? AnalyticsContext.success_bot : AnalyticsContext.success));

            // Grab the opponent profile pic if any
            if (matchInfoModel.activeMatch.opponentPublicProfile.facebookUserId != null)
            {
                PublicProfile opponentPublicProfile = matchInfoModel.activeMatch.opponentPublicProfile;
                if (opponentPublicProfile.facebookUserId != null)
                {
                    facebookService.GetSocialPic(opponentPublicProfile.facebookUserId, opponentPublicProfile.playerId).Then(OnGetOpponentProfilePicture);
                }
            }
            else
            {
                Release();
            }
        }

        private ProfileVO GetOpponentProfile()
        {
            PublicProfile publicProfile = matchInfoModel.activeMatch.opponentPublicProfile;

            var friend = playerModel.GetFriend(publicProfile.playerId);
            if (friend != null)
            {
                publicProfile = friend.publicProfile;
            }

            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = publicProfile.profilePicture;
            pvo.playerName = publicProfile.name;
            pvo.eloScore = publicProfile.eloScore;
            pvo.countryId = publicProfile.countryId;
            pvo.playerId = publicProfile.playerId;
            pvo.avatarColorId = publicProfile.avatarBgColorId;
            pvo.avatarId = publicProfile.avatarId;
            pvo.isOnline = true;
            pvo.isActive = publicProfile.isActive;
            pvo.isPremium = publicProfile.isSubscriber;
            pvo.leagueBorder = publicProfile.leagueBorder;
            pvo.trophies2 = publicProfile.trophies2;

            if (pvo.playerPic == null)
            {
                pvo.playerPic = picsModel.GetPlayerPic(publicProfile.playerId);
            }

            return pvo;
        }

        // Handle errors if any from the original backend request
        // Release ONLY on error condition
        private void HandleFindMatchErrors(BackendResult result)
        {
            //-- Hide UI blocker and spinner here
            showProcessingSignal.Dispatch(false, false);

            if (result == BackendResult.CANCELED)
            {
                Release();
            }
            else if (result != BackendResult.SUCCESS)
            {
                loadLobbySignal.Dispatch();
                backendErrorSignal.Dispatch(result);
                Release();
            }
            else
            {
                var betValue = FindMatchAction.actionData.betValue;
                playerModel.coins -= betValue;
                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.COINS, (int)betValue, "championship_coins", "bet_placed");
            }
        }

        private MatchAnalyticsVO GetFindMatchAnalyticsVO(AnalyticsContext context)
        {
            var matchAnalyticsVO = new MatchAnalyticsVO();
            matchAnalyticsVO.eventID = AnalyticsEventId.match_find;
            matchAnalyticsVO.context = context;

            var actionCode = FindMatchAction.actionData.action.Equals(FindMatchAction.ActionCode.Accept.ToString()) ? FindMatchAction.actionData.acceptActionCode : FindMatchAction.actionData.action;

            if (actionCode == FindMatchAction.ActionCode.RandomLong.ToString())
            {
                matchAnalyticsVO.matchType = "classic";
            }
            else if (actionCode == FindMatchAction.ActionCode.Challenge1.ToString() ||
                     actionCode == FindMatchAction.ActionCode.Random1.ToString())
            {
                matchAnalyticsVO.matchType = "1m";
            }
            else if (actionCode == FindMatchAction.ActionCode.Challenge3.ToString() ||
                     actionCode == FindMatchAction.ActionCode.Random3.ToString())
            {
                matchAnalyticsVO.matchType = "3m";
            }
            else if (actionCode == FindMatchAction.ActionCode.Challenge.ToString() ||
                     actionCode == FindMatchAction.ActionCode.Random.ToString())
            {
                matchAnalyticsVO.matchType = "5m";
            }
            else if (actionCode == FindMatchAction.ActionCode.Challenge10.ToString() ||
                     actionCode == FindMatchAction.ActionCode.Random10.ToString())
            {
                matchAnalyticsVO.matchType = "10m";
            }
            else if (actionCode == FindMatchAction.ActionCode.Challenge30.ToString() ||
                     actionCode == FindMatchAction.ActionCode.Random30.ToString())
            {
                matchAnalyticsVO.matchType = "30m";
            }


            if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random1.ToString()  ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random3.ToString()  ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random.ToString()   ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random10.ToString() ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random30.ToString() ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.RandomLong.ToString())
            {
                matchAnalyticsVO.friendType = "random";
            }
            else if (playerModel.friends.ContainsKey(FindMatchAction.actionData.opponentId))
            {
                var friendType = playerModel.friends[FindMatchAction.actionData.opponentId].friendType;
                if (friendType.Equals(GSBackendKeys.Friend.TYPE_SOCIAL))
                {
                    if (FindMatchAction.actionData.notificationStatus == FindMatchAction.NotificationStatus.InGame)
                    {
                        matchAnalyticsVO.friendType = "friends_facebook_notification_in_app";
                    }
                    else if (FindMatchAction.actionData.notificationStatus == FindMatchAction.NotificationStatus.OutGame)
                    {
                        matchAnalyticsVO.friendType = "friends_facebook_notification_out_app";
                    }
                    else
                    {
                        matchAnalyticsVO.friendType = "friends_facebook";
                    }
                }
                else if (friendType.Equals(GSBackendKeys.Friend.TYPE_FAVOURITE))
                {
                    if (FindMatchAction.actionData.notificationStatus == FindMatchAction.NotificationStatus.InGame)
                    {
                        matchAnalyticsVO.friendType = "friends_community_notification_in_app";
                    }
                    else if (FindMatchAction.actionData.notificationStatus == FindMatchAction.NotificationStatus.OutGame)
                    {
                        matchAnalyticsVO.friendType = "friends_community_notification_out_app";
                    }
                    else
                    {
                        matchAnalyticsVO.friendType = "friends_community";
                    }
                }
                else
                {
                    if (FindMatchAction.actionData.notificationStatus == FindMatchAction.NotificationStatus.InGame)
                    {
                        matchAnalyticsVO.friendType = "community_notification_in_app";
                    }
                    else if (FindMatchAction.actionData.notificationStatus == FindMatchAction.NotificationStatus.OutGame)
                    {
                        matchAnalyticsVO.friendType = "community_notification_out_app";
                    }
                    else
                    {
                        matchAnalyticsVO.friendType = "community";
                    }
                }
            }
            else
            {
                if (FindMatchAction.actionData.notificationStatus == FindMatchAction.NotificationStatus.InGame)
                {
                    matchAnalyticsVO.friendType = "community_notification_in_app";
                }
                else if (FindMatchAction.actionData.notificationStatus == FindMatchAction.NotificationStatus.OutGame)
                {
                    matchAnalyticsVO.friendType = "community_notification_out_app";
                }
                else
                {
                    matchAnalyticsVO.friendType = "community";
                }
            }
            

            return matchAnalyticsVO;
        }
    }
}
