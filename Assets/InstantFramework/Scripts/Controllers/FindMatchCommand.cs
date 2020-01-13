/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;


namespace TurboLabz.InstantFramework
{
    public class FindMatchCommand : Command
    {
        // Paramaters
        [Inject] public string action { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public ShowFindMatchSignal showFindMatchSignal { get; set; }
        [Inject] public GetGameStartTimeSignal getGameStartTimeSignal { get; set; }
        [Inject] public MatchFoundSignal matchFoundSignal { get; set; }
        [Inject] public UpdateOpponentProfileSignal updateOpponentProfileSignal { get; set; }
        [Inject] public UpdateChatOpponentPicSignal updateChatOpponentPicSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }

        // Listen to signal
        [Inject] public FindMatchCompleteSignal findMatchCompleteSignal { get; set; }
        [Inject] public FindMatchRequestCompleteSignal findMatchRequestCompleteSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        public override void Execute()
        {
            Retain();

            // This sends the backend request
            backendService.FindMatch(action).Then(HandleFindMatchErrors);

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

            }
            else
            {
                showFindMatchSignal.Dispatch(action);

                // The actual found match message arrives through a different pipeline
                // from the backend
                findMatchCompleteSignal.AddOnce(OnFindMatchComplete);
            }
        }

        private void OnFindMatchComplete(string challengeId)
        {
            matchInfoModel.activeChallengeId = challengeId;

            // Create and fill the opponent profile
            ProfileVO pvo = GetOpponentProfile();

            // Set the opponent info in the game view
            updateOpponentProfileSignal.Dispatch(pvo);

            // Set the finding match view to a found match state
            matchFoundSignal.Dispatch(pvo);

            // For quick match games, the flow continues from the get game start time signal
            // where both clients start at a synch time stamp
            getGameStartTimeSignal.Dispatch();

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
            if (result == BackendResult.CANCELED)
            {
                Release();
            }
            else if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
                Release();
            }
        }
    }
}
