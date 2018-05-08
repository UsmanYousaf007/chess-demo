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

        private int selectedDurationIndex;
        private Dictionary<int, int> stats;

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

        public void UpdateView(CPUStatsVO vo)
        {
            
        }

        public void OnDurationDecButtonClicked()
        {
        }

        public void OnDurationIncButtonClicked()
        {
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

        private void UpdateStats()
        {

        }
        /*
        public void UpdateDuration(CPUMenuVO vo)
        {
            int duration = vo.durationMinutes[vo.selectedDurationIndex];

            if (duration == 0)
            {
                infinityIcon.SetActive(true);
                currentDurationLabel.gameObject.SetActive(false);
            }
            else
            {
                infinityIcon.SetActive(false);
                currentDurationLabel.gameObject.SetActive(true);
                currentDurationLabel.text = 
                    localizationService.Get(LocalizationKey.GM_ROOM_DURATION, vo.durationMinutes[vo.selectedDurationIndex]);
            }

            if (vo.selectedDurationIndex == 0)
            {
                decDurationButton.interactable = false;
                incDurationButton.interactable = true;
            }
            else if (vo.selectedDurationIndex == (vo.durationMinutes.Length - 1))
            {
                decDurationButton.interactable = true;
                incDurationButton.interactable = false;
            }
            else
            {
                decDurationButton.interactable = true;
                incDurationButton.interactable = true;
            }
        }
        */

        private void OnBackButtonClicked()
        {
            backButtonClickedSignal.Dispatch();
        }


    }
}
