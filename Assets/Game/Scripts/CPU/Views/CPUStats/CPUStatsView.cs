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
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        // Scene references
        public Text titleLabel;
        public Text timeLimitLabel;
        public Button timeDecButton;
        public Text timeCurrentLabel;
        public Button timeIncButton;
        public Text strengthLabel;
        public Text performanceLabel;
        public Text[] winLabels;
        public Text[] lossLabels;
        public Text[] drawLabels;
        public RectTransform[] winBars;
        public RectTransform[] lossBars;
        public RectTransform[] drawBars;
        public Text[] winCountLabels;
        public Text[] lossCountLabels;
        public Text[] drawCountLabels;
        public Button backButton;
        public Button resetButton;
        public Text resetLabel;
        public Button decDurationButton;
        public Button incDurationButton;

        // View signals
        public Signal backButtonClickedSignal = new Signal();
        public Signal resetButtonClickedSignal = new Signal();

        private const float BAR_MAX_WIDTH = 462;
        private const float BAR_MIN_WIDTH = 9;

        private int durationIndex;
        private int[] durationMinutes;
        private Dictionary<int, Performance> stats;

        public void Init()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
            resetButton.onClick.AddListener(OnResetButtonClicked);
            decDurationButton.onClick.AddListener(OnDecDurationButtonClicked);
            incDurationButton.onClick.AddListener(OnIncDurationButtonClicked);

            titleLabel.text = localizationService.Get(LocalizationKey.STATS_TITLE);
            timeLimitLabel.text = localizationService.Get(LocalizationKey.STATS_TIME_LIMIT);
            strengthLabel.text = localizationService.Get(LocalizationKey.STATS_STRENGTH);
            performanceLabel.text = localizationService.Get(LocalizationKey.STATS_PERFORMANCE);
            string W = localizationService.Get(LocalizationKey.STATS_W);
            for (int i = 0; i < winLabels.Length; i++) winLabels[i].text = W;
            string L = localizationService.Get(LocalizationKey.STATS_L);
            for (int i = 0; i < lossLabels.Length; i++) lossLabels[i].text = L;
            string D = localizationService.Get(LocalizationKey.STATS_D);
            for (int i = 0; i < drawLabels.Length; i++) drawLabels[i].text = D;
            resetLabel.text = localizationService.Get(LocalizationKey.STATS_RESET);
        }

        public void CleanUp()
        {
  //          backButton.onClick.RemoveAllListeners();
        }

        public void Show() 
        { 
            gameObject.SetActive(true); 
        }

        public void Hide()
        { 
            gameObject.SetActive(false); 
        }

        public void UpdateView(CPUStatsVO vo)
        {
            durationIndex = vo.durationIndex;
            durationMinutes = vo.durationMinutes;
            stats = vo.stats;

            RefreshData();
        }

        private void UpdateStats()
        {
            for (int i = 0; i < winCountLabels.Length; i++)
            {
                Performance p;

                if (i < stats.Count)
                {
                    p = stats[i];
                }

                winCountLabels[i].text = p.wins.ToString();
                lossCountLabels[i].text = p.losses.ToString();
                drawCountLabels[i].text = p.draws.ToString();

                int[] vals = { p.wins, p.losses, p.draws };
                int max = Mathf.Max(vals);

                SetBarWidth(winBars[i], (BAR_MAX_WIDTH * p.wins /  max));
                SetBarWidth(lossBars[i], (BAR_MAX_WIDTH * p.losses /  max));
                SetBarWidth(drawBars[i], (BAR_MAX_WIDTH * p.draws /  max));
            }
        }

        private void SetBarWidth(RectTransform bar, float width)
        {
            Vector2 sizeDelta = bar.sizeDelta;
            sizeDelta.x = Mathf.Max(BAR_MIN_WIDTH, width);
            bar.sizeDelta = sizeDelta;
        }

        private void UpdateDuration()
        {
            int duration = durationMinutes[durationIndex];

            timeCurrentLabel.text = (duration == 0) ? 
                localizationService.Get(LocalizationKey.CPU_MENU_DURATION_NONE)
                : localizationService.Get(LocalizationKey.GM_ROOM_DURATION, durationMinutes[durationIndex]);

            if (durationIndex == 0)
            {
                decDurationButton.interactable = false;
                incDurationButton.interactable = true;
            }
            else if (durationIndex == (durationMinutes.Length - 1))
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

        private void OnBackButtonClicked()
        {
            backButtonClickedSignal.Dispatch();
        }

        private void OnResetButtonClicked()
        {
            resetButtonClickedSignal.Dispatch();
        }

        private void OnDecDurationButtonClicked()
        {
            if (durationIndex > 0)
            {
                durationIndex--;
            }

            RefreshData();
        }

        private void OnIncDurationButtonClicked()
        {
            if (durationIndex < durationMinutes.Length - 1 )
            {
                durationIndex++;
            }

            RefreshData();
        }

        private void RefreshData()
        {
            UpdateDuration();
            UpdateStats();
        }
    }
}
