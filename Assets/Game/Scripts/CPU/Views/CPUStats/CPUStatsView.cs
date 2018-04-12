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
        public Text strengthLabel;
        public Button strengthDecButton;
        public Text strengthCurrentLabel;
        public Button strengthIncButton;
        public Text[] durationLabels;
        public Text[] winLabels;
        public Text[] drawLabels;
        public Text[] lossLabels;
        public Text[] winAmtLabels;
        public Text[] drawAmtLabels;
        public Text[] lossAmtLabels;
        public Text[] winAmtPctLabels;
        public Text[] drawAmtPctLabels;
        public Text[] lossAmtPctLabels;
        public Text totalGamesLabel;
        public Text totalGamesCount;
        public Button backButton;

        // View signals
        public Signal backButtonClickedSignal = new Signal();

        private int selectedStrengthIndex;
        private int maxStrength;
        private int[] durationMinutes;
        private Dictionary<int, PerformanceSet> stats;

        public void Init()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
            strengthDecButton.onClick.AddListener(OnStrengthDecButtonClicked);
            strengthIncButton.onClick.AddListener(OnStrengthIncButtonClicked);

            titleLabel.text = localizationService.Get(LocalizationKey.STATS_TITLE);
            strengthLabel.text = localizationService.Get(LocalizationKey.STATS_CURRENT_STRENGTH);
            totalGamesLabel.text = localizationService.Get(LocalizationKey.STATS_TOTAL_GAMES);
            string W = localizationService.Get(LocalizationKey.STATS_W);
            for (int i = 0; i < winLabels.Length; i++) winLabels[i].text = W;
            string L = localizationService.Get(LocalizationKey.STATS_L);
            for (int i = 0; i < lossLabels.Length; i++) lossLabels[i].text = L;
            string D = localizationService.Get(LocalizationKey.STATS_D);
            for (int i = 0; i < drawLabels.Length; i++) drawLabels[i].text = D;
        }

        public void CleanUp()
        {
            backButton.onClick.RemoveAllListeners();
            strengthDecButton.onClick.RemoveAllListeners();
            strengthIncButton.onClick.RemoveAllListeners();
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
            selectedStrengthIndex = vo.selectedStrengthIndex;
            maxStrength = vo.maxStrength;
            durationMinutes = vo.durationMinutes;
            stats = vo.stats;

            RefreshData();
        }

        private void UpdateStats()
        {
            int totalGames = 0;

            for (int i = 0; i < durationLabels.Length; i++)
            {
                if (i != 0) // Since we show an infinity image instead of text for 0 
                {
                    durationLabels[i].text = localizationService.Get(LocalizationKey.STATS_DURATION, durationMinutes[i]);
                }

                Performance p = stats[i].performances[selectedStrengthIndex];
                totalGames += p.wins;
                totalGames += p.draws;
                totalGames += p.losses;

                float max = (float)Mathf.Max(p.wins, p.draws, p.losses);

                int winPct = 0;
                int drawPct = 0;
                int lossPct = 0;

                if (max > 0)
                {
                    winPct = Mathf.FloorToInt(((float)p.wins / max) * 100);
                    drawPct = Mathf.FloorToInt(((float)p.draws / max) * 100);
                    lossPct = Mathf.FloorToInt(((float)p.losses / max) * 100);
                }

                winAmtLabels[i].text = p.wins.ToString();
                winAmtPctLabels[i].text = localizationService.Get(LocalizationKey.STATS_PCT, winPct.ToString());
                drawAmtLabels[i].text = p.draws.ToString();
                drawAmtPctLabels[i].text = localizationService.Get(LocalizationKey.STATS_PCT, drawPct.ToString());
                lossAmtLabels[i].text = p.losses.ToString();
                lossAmtPctLabels[i].text = localizationService.Get(LocalizationKey.STATS_PCT, lossPct.ToString());
            }

            totalGamesCount.text = totalGames.ToString();
        }

        private void UpdateStrength()
        {
            int strength = selectedStrengthIndex + 1;

            strengthCurrentLabel.text = strength.ToString();
                

            if (selectedStrengthIndex == 0)
            {
                strengthDecButton.interactable = false;
                strengthIncButton.interactable = true;
            }
            else if (selectedStrengthIndex == (durationMinutes.Length - 1))
            {
                strengthDecButton.interactable = true;
                strengthIncButton.interactable = false;
            }
            else
            {
                strengthDecButton.interactable = true;
                strengthIncButton.interactable = true;
            }
        }

        private void OnBackButtonClicked()
        {
            backButtonClickedSignal.Dispatch();
        }

        private void OnStrengthDecButtonClicked()
        {
            if (selectedStrengthIndex > 0)
            {
                selectedStrengthIndex--;
            }

            RefreshData();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        private void OnStrengthIncButtonClicked()
        {
            if (selectedStrengthIndex < maxStrength - 2 )
            {
                selectedStrengthIndex++;
            }

            RefreshData();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        private void RefreshData()
        {
            UpdateStrength();
            UpdateStats();
        }
    }
}
