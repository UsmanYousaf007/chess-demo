/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:54:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using System.Collections;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class BottomNavView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public enum ButtonId
        {
            Home,
            Shop,
            Friends
        }

        public ButtonId buttonId;

        public Text homeLabel;
        public Text friendsLabel;

        public Image homeIcon;
        public Image friendsIcon;

        public Button homeButton;
        public Button friendsButton;

        public Signal homeButtonClickedSignal = new Signal();
        public Signal friendsButtonClickedSignal = new Signal();

        public void Init()
        {
            homeLabel.text = localizationService.Get(LocalizationKey.NAV_HOME);
            friendsLabel.text = localizationService.Get(LocalizationKey.NAV_FRIENDS);

            homeButton.onClick.AddListener(HomeButtonClicked);
            friendsButton.onClick.AddListener(FriendsButtonClicked);

            UpdateButtons();
        }

        void UpdateButtons()
        {
            homeButton.interactable = true;
            homeIcon.color = Colors.WHITE_100;
            homeLabel.color = Colors.WHITE_100;

            friendsButton.interactable = true;
            friendsIcon.color = Colors.WHITE_100;
            friendsLabel.color = Colors.WHITE_100;

            if (buttonId == ButtonId.Home)
            {
                homeButton.interactable = false;
                homeIcon.color = Colors.YELLOW;
                homeLabel.color = Colors.YELLOW;
            }
            else if (buttonId == ButtonId.Friends)
            {
                friendsButton.interactable = false;
                friendsIcon.color = Colors.YELLOW;
                friendsLabel.color = Colors.YELLOW;
            }
        }

        void HomeButtonClicked()
        {
            homeButtonClickedSignal.Dispatch();
        }

        void FriendsButtonClicked()
        {
            friendsButtonClickedSignal.Dispatch();
        }
    }
}
