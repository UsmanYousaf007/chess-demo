/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class TournamentLeaderboardFooter : MonoBehaviour
    {
        public Image bg;
        public Text youHaveLabel;
        public Button enterButton;
        public Text enterButtonFreePlayLabel;
        public Text enterButtonTicketPlayLabel;
        public Text enterButtonTicketPlayCountText;
        public GameObject ticketPlayButtonGroup;
        public string itemToConsumeShortCode;
        public Text gemsCost;
        public Image gemsBg;
        public Sprite haveEnoughGemsSprite;
        public Sprite notEnoughGemsSprite;
        public Button resultsContinueButton;
        public Text resultsContinueButtonLabel;

        public Signal enterButtonClickedSignal = new Signal();
        public Signal resultsContinueButtonClickedSignal = new Signal();

        [HideInInspector] public bool haveEnoughItems;
        [HideInInspector] public bool haveEnoughGems;

        private void Awake()
        {
            enterButton.onClick.AddListener(() => enterButtonClickedSignal.Dispatch());
            resultsContinueButton.onClick.AddListener(() => resultsContinueButtonClickedSignal.Dispatch());
        }
    }
}
