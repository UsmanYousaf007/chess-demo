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
        }

        public void UpdateLeague(string leagueType, ProfileVO pvo)
        {
            if (leagues.ContainsKey(leagueType))
            { 
                foreach (var item in list)
                {
                    if(item.leagueType == leagueType)
                        item.UpdateView(true, pvo);
                    else
                        item.UpdateView(false, pvo);
                }
            }
        }
    }
}
