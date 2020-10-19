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
using System;

namespace TurboLabz.InstantGame
{
    [CLSCompliant(false)]
    public class LeaguePerksView : View
    {
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Dispatch Signal
        public Signal OnBackButtonClickedSignal = new Signal();
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }

        public Text titleText;
        public Text backButtonText;
        public Button backButton;
        public ScrollRect scrollRect;
        public List<LeagueTierInfo> list;
        public Dictionary<string ,LeagueTierInfo> leagues = new Dictionary<string, LeagueTierInfo>();

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public ILeaguesModel leaguesModel { get; set; }

        private float playerLeague = 0;

        public void Init()
        {
            titleText.text = localizationService.Get(LocalizationKey.LEAGUE_PERKS_TITLE);
            backButtonText.text = localizationService.Get(LocalizationKey.BACK_TEXT);
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        public void Show()
        {
            showBottomNavSignal.Dispatch(false);
            gameObject.SetActive(true);
            AnimateScrollViewToPlayerLeague();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnBackButtonClicked()
        {
            audioService.PlayStandardClick();
            OnBackButtonClickedSignal.Dispatch();
        }

        public void SetAllLeagueInfo()
        {
            foreach(var item in list)
            {
                LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(item.leagueID);
                leaguesModel.leagues.TryGetValue(leagueAssets.typeID, out League value);
                item.SetLeagueInfo(leagueAssets, value);
                leagues.Add(item.leagueID, item);
            }
        }

        public void UpdateLeague(string leagueID)
        {
            if (leagues.ContainsKey(leagueID))
            { 
                foreach (var item in list)
                {
                    if (item.leagueID == leagueID)
                    {
                        playerLeague = float.Parse(item.leagueID);
                        item.UpdateView(true);
                    }
                    else
                    {
                        item.UpdateView(false);
                    }
                }
            }
        }

        private void AnimateScrollViewToPlayerLeague()
        {
            scrollRect.verticalNormalizedPosition = 1.0f;

            iTween.ValueTo(gameObject,
             iTween.Hash(
                 "from", 1.0f,
                 "to", ((playerLeague + 1) / list.Count) - 0.1f,
                 "time", 1,
                 "onupdate", "UpdateScrollView",
                 "onupdatetarget", gameObject,
                 "oncomplete", "OnAnimationComplete",
                 "oncompletetarget", gameObject
             ));
        }

        private void UpdateScrollView(float value)
        {
            scrollRect.verticalNormalizedPosition = value;
        }

        private void OnAnimationComplete()
        {
            scrollRect.verticalNormalizedPosition = playerLeague == 0 ? 0 : playerLeague == list.Count - 1 ? 1 : scrollRect.verticalNormalizedPosition;
        }
    }
}
