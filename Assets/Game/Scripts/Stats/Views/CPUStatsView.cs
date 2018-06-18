/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantChess
{
    public class CPUStatsView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Scene references
        public Text titleLabel;
        public Text difficultyLabel;
        public Image[] stars;
        public Sprite noStar;
        public Sprite silverStar;
        public Sprite goldStar;
        public Text wonLabel;
        public Text wonWithHelpLabel;
        public Text wonWithHelpExplainerLabel;
        public Text durationLabel;
        public Button durationDecButton;
        public Text durationCurrentLabel;
        public Button durationIncButton;
        public GameObject infinityIcon;
        public Button backButton;

        // View signals
        public Signal backButtonClickedSignal = new Signal();

        private int[] durationMinutes;
        private int selectedDurationIndex;
        private Dictionary<int, PerformanceSet> stats;

        public void Init()
        {
            titleLabel.text = localizationService.Get(LocalizationKey.STATS_TITLE);
            difficultyLabel.text = localizationService.Get(LocalizationKey.STATS_DIFFICULTY);

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = noStar;
            }

            wonLabel.text = localizationService.Get(LocalizationKey.STATS_WON);
            wonWithHelpLabel.text = localizationService.Get(LocalizationKey.STATS_WON_WITH_HELP);
            wonWithHelpExplainerLabel.text = localizationService.Get(LocalizationKey.STATS_WON_WITH_HELP_EXPLAINER);
            durationLabel.text = localizationService.Get(LocalizationKey.STATS_DURATION);

            infinityIcon.SetActive(false);

            durationDecButton.onClick.AddListener(OnDurationDecButtonClicked);
            durationIncButton.onClick.AddListener(OnDurationIncButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        public void UpdateView(StatsVO vo)
        {
            selectedDurationIndex = vo.selectedDurationIndex;
            durationMinutes = vo.durationMinutes;
            stats = vo.stats;

            UpdateStats();
        }

        public void CleanUp()
        {
            backButton.onClick.RemoveAllListeners();
            durationDecButton.onClick.RemoveAllListeners();
            durationIncButton.onClick.RemoveAllListeners();
        }

        public void Show() 
        { 
            gameObject.SetActive(true); 
        }

        public void Hide()
        { 
            gameObject.SetActive(false); 
        }

        void OnDurationDecButtonClicked()
        {
            if (selectedDurationIndex > 0)
            {
                selectedDurationIndex--;
            }

            UpdateStats();
        }

        void OnDurationIncButtonClicked()
        {
            if (selectedDurationIndex < durationMinutes.Length - 1)
            {
                selectedDurationIndex++;
            }

            UpdateStats();
        }

        void OnBackButtonClicked()
        {
            backButtonClickedSignal.Dispatch();
        }

        void UpdateStats()
        {
            UpdateStars();
            UpdateDuration();
        }

        void UpdateStars()
        {
            List<int> pset = stats[selectedDurationIndex].performance;

            for (int i = 0; i < pset.Count; i++)
            {
                LogUtil.Log("DIFFICULTY = " + i + " SCORE = " + pset[i], "cyan"); 
                if (pset[i] == 1)
                {
                    stars[i].sprite = silverStar;
                }
                else if (pset[i] == 2)
                {
                    stars[i].sprite = goldStar;
                }
                else
                {
                    stars[i].sprite = noStar;
                }
            }
        }

        void UpdateDuration()
        {
            if (selectedDurationIndex == 0)
            {
                durationCurrentLabel.gameObject.SetActive(false);
                infinityIcon.SetActive(true);
            }
            else
            {
                durationCurrentLabel.text = localizationService.Get(
                    LocalizationKey.STATS_DURATION_TIME, durationMinutes[selectedDurationIndex]
                );

                durationCurrentLabel.gameObject.SetActive(true);
                infinityIcon.SetActive(false);
            }

            durationDecButton.interactable = true;
            durationIncButton.interactable = true;

            if (selectedDurationIndex == 0)
            {
                durationDecButton.interactable = false;
            }
            else if (selectedDurationIndex == durationMinutes.Length - 1)
            {
                durationIncButton.interactable = false;
            }
        }
    }
}
