/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using UnityEngine;
using strange.extensions.signal.impl;
using System;

namespace TurboLabz.InstantFramework
{
    public class LeagueProfileStripView : View
    {
        [Header ("Player Info")]
        public Text playerLeagueTitleLabel;
        public Image playerLeagueThumbnailImage;
        public Text playerTrophiesCountLabel;
        public Text playerRankCountLabel;
        public Image playerRankStatusImage;

        [Header ("Tournament Info")]
        public Text tournamentCountdownTimerLabel;

        [Header("Localization")]
        public Text leagueEndsInLabel;
        public Text tapLabel;
        public Text trophiesLabel;
        public Text rankLabel;

        [Header("Strip")]
        public Button stripButton;


        private Signal stripClickedSignal;
        private GameObject gameObjectPlayerRank;

        //Models
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        //Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        public Signal leagueProfileClickedSignal = new Signal();

        public void Init()
        {
            leagueEndsInLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_ENDS_IN);
            tapLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_TAP);
            trophiesLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_TROPHIES);
            rankLabel.text = localizationService.Get(LocalizationKey.PLAYER_LEAGUE_PROFILE_STRIP_RANK);

            stripButton.onClick.AddListener(OnLeagueProfileButtonClicked);

            gameObjectPlayerRank = playerRankStatusImage.gameObject;
        }

        public void UpdateView(LeagueProfileStripVO vo)
        {
            playerLeagueTitleLabel.text = vo.playerLeagueTitle;
            playerLeagueThumbnailImage = vo.playerLeagueThumbnailImage;
            playerTrophiesCountLabel.text = vo.playerTrophiesCount.ToString();
            playerRankCountLabel.text = vo.playerRankCount.ToString();
            playerRankStatusImage = vo.playerRankStatusImage;
            tournamentCountdownTimerLabel.text = vo.tournamentCountdownTimer;

            gameObjectPlayerRank.SetActive(vo.playerRankStatusImage != null);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void SetStripClickedSignal(Signal signal)
        {
            stripClickedSignal = signal;
        }

        public void OnStripButtonClicked()
        {
            TLUtils.LogUtil.Log("LeagueProfileStripView::OnStripButtonClicked()");
            if (stripClickedSignal != null)
            {
                stripClickedSignal.Dispatch();
            }
        }

        public void OnLeagueProfileButtonClicked()
        {
            /*ProfileVO pvo = new ProfileVO();
            pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            pvo.playerName = playerModel.name;
            pvo.eloScore = playerModel.eloScore;
            pvo.countryId = playerModel.countryId;
            //pvo.isFacebookLoggedIn = facebookService.isLoggedIn();
            //pvo.isAppleSignedIn = signInWithAppleService.IsSignedIn();
            //pvo.isAppleSignInSupported = signInWithAppleService.IsSupported();
            pvo.playerId = playerModel.id;
            pvo.avatarId = playerModel.avatarId;
            pvo.avatarColorId = playerModel.avatarBgColorId;
            pvo.isPremium = playerModel.HasSubscription();

            if (pvo.isFacebookLoggedIn && pvo.playerPic == null)
            {
                pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            }*/

            leagueProfileClickedSignal.Dispatch();
        }
    }
}
