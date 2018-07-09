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

namespace TurboLabz.InstantGame
{
    public class StatsView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Scene references

        public Text onlineTitle;
        public Text onlineWinPct;
        public Text onlineWon;
        public Text onlineLost;
        public Text onlineDrawn;
        public Text onlineTotal;

        public Text onlineWinPctVal;
        public Text onlineWonVal;
        public Text onlineLostVal;
        public Text onlineDrawnVal;
        public Text onlineTotalVal;

        public Text computerTitle;
        public Text legendGold;
        public Text legendSilver;

        public Image[] stars;
        public Sprite noStar;
        public Sprite silverStar;
        public Sprite goldStar;

        public void Init()
        {
            onlineTitle.text = localizationService.Get(LocalizationKey.STATS_ONLINE_TITLE);
            onlineWinPct.text = localizationService.Get(LocalizationKey.STATS_ONLINE_WIN_PCT);
            onlineWon.text = localizationService.Get(LocalizationKey.STATS_ONLINE_WON);
            onlineLost.text = localizationService.Get(LocalizationKey.STATS_ONLINE_LOST);
            onlineDrawn.text = localizationService.Get(LocalizationKey.STATS_ONLINE_DRAWN);
            onlineTotal.text = localizationService.Get(LocalizationKey.STATS_ONLINE_TOTAL);
            computerTitle.text = localizationService.Get(LocalizationKey.STATS_COMPUTER_TITLE);
            legendGold.text = localizationService.Get(LocalizationKey.STATS_LEGEND_GOLD);
            legendSilver.text = localizationService.Get(LocalizationKey.STATS_LEGEND_SILVER);

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = noStar;
            }
        }

        public void UpdateView(StatsVO vo)
        {
            List<int> pset = vo.stats[0].performance;

            for (int i = 0; i < pset.Count; i++)
            {
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

            /// Update online stats
            onlineWinPctVal.text = vo.onlineWinPct.ToString() + " %";
            onlineWonVal.text = vo.onlineWon.ToString();
            onlineLostVal.text = vo.onlineLost.ToString();
            onlineDrawnVal.text = vo.onlineDrawn.ToString();
            onlineTotalVal.text = vo.onlineTotal.ToString();
        }

        public void Show() 
        { 
            gameObject.SetActive(true); 
        }

        public void Hide()
        { 
            gameObject.SetActive(false); 
        }
    }
}
