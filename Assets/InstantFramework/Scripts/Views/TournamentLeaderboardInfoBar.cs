/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TurboLabz.InstantFramework
{
    public class TournamentLeaderboardInfoBar : MonoBehaviour
    {
        public Text rulesLabel;
        public Text totalScoreLabel;
        public Text gameModeText;

        public Button rulesTooltipButton;
        public Button totalScoreTooltipButton;
        public Button gameModeTooltipButton;

        public GameObject rulesTooltip;
        public GameObject totalScoresTooltip;
        public GameObject gameModesTooltip;

        public TMP_Text rulesTooltipText;
        public TMP_Text totalScoresTooltipText;
        public TMP_Text gameModeTooltipText;

        public Image rulesTooltipBG;
        public Image totalScoresTooltipBG;
        public Image gameModeTooltipBG;

        public Text columnHeaderRankLabel;
        public Text columnHeaderScoreLabel;
        public Text columnHeaderRewardsLabel;

        [HideInInspector]
        public Signal rulesButtonClickedSignal = new Signal();
        public Signal totalScoreButtonClickedSignal = new Signal();
        public Signal gameModeButtonClickedSignal = new Signal();

        void Awake()
        {
            //rulesButton.onClick.AddListener(() => rulesButtonClickedSignal.Dispatch());
            //totalScoreButton.onClick.AddListener(() => totalScoreButtonClickedSignal.Dispatch());
            //gameModeButton.onClick.AddListener(() => gameModeButtonClickedSignal.Dispatch());
            rulesTooltip.SetActive(false);
            totalScoresTooltip.SetActive(false);
            gameModesTooltip.SetActive(false);
        }
    }
}
