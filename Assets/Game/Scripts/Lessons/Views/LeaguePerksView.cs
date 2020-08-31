/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace TurboLabz.InstantGame
{
    public class LeaguePerksView : View
    {
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Dispatch Signal
        public Signal OnBackButtonClickedSignal = new Signal();

        public Text titleText;
        public Text backButtonText;
        public Button backButton;
        public List<LeagueTierInfo> list;
        public Dictionary<string ,LeagueTierInfo> leagues = new Dictionary<string, LeagueTierInfo>();

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        public void Init()
        {
            titleText.text = localizationService.Get(LocalizationKey.LEAGUE_PERKS_TITLE);
            backButtonText.text = localizationService.Get(LocalizationKey.BACK_TEXT);
            backButton.onClick.AddListener(OnBackButtonClicked);
            SetAllLeagueInfo();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnBackButtonClicked()
        {
            OnBackButtonClickedSignal.Dispatch();
        }

        public void SetAllLeagueInfo()
        {
            foreach(var item in list)
            {
                LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(item.leagueType);
                item.SetLeagueInfo(leagueAssets);
                leagues.Add(item.leagueType, item);
            }

            UpdateLeague("bronze");
        }

        public void UpdateLeague(string leagueID)
        {
            ProfileVO pvo = new ProfileVO();
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
            }

            if (leagues.ContainsKey(leagueID))
            { 
                foreach (var item in list)
                {
                    if(item.leagueType == leagueID)
                        item.UpdateView(true, pvo);
                    else
                        item.UpdateView(false, pvo);
                }
            }
        }
    }
}
