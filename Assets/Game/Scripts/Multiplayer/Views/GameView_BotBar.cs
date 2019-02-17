/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine.UI;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using UnityEngine;
using strange.extensions.signal.impl;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Bot Bar")]
        public Text backToFriendsLabel;
        public Button backToFriendsButton;
        private bool showAdOnBack;

        public void InitBotBar()
        {
            backToFriendsLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_FRIENDS);
            backToFriendsButton.onClick.AddListener(OnBackToFriendsClicked);
        }

        void OnParentShowBotBar()
        {
            backToFriendsButton.gameObject.SetActive(false);
            backToFriendsLabel.gameObject.SetActive(false);
            showAdOnBack = false;
        }

        void UpdateBotBar()
        {
            backToFriendsButton.gameObject.SetActive(isLongPlay);
            backToFriendsLabel.gameObject.SetActive(isLongPlay);
        }

        void OnBackToFriendsClicked()
        {
            if (showAdOnBack)
            {
                //showAdSignal.Dispatch(AdType.Interstitial, GSBackendKeys.ClaimReward.NONE);
                showAdOnBack = false;

                analyticsService.Event(AnalyticsEventId.ads_friends_back);
            }
            backToFriendsSignal.Dispatch(); 
        }
    }
}
