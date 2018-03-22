/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-06-13 05:10:38 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.Gamebet
{
    public class RoomCard : MonoBehaviour
    {   
        [System.Serializable]
        public class _StandardRoomCard
        {
            public Image background;
            public Image flag;

            public Text roomNameLabel;
            public Text roomDurationLabel;

            public Image roomBanner;

            public Text prizeTitleLabel;
            public Text prizeAmountLabel;

            public Text entryFeeTitleLabel;
            public Text entryFeeLabel;

            public Text roomTitleLabel;
            public Text trophyCountLabel;

            public Text winsNeededTitleLabel;
            public Image trophyProgressFill;
            public Text trophyWinsNeededLabel;

            public Image infoBg;
            public Button infoButton;

            public GameObject roomCard;
            public GameObject lockedItems;

            public Text unlockLevelText;
            public Text mysterCardText;
        } 

        [System.Serializable]
        public class _AllInOneRoomCard
        {
            public Image background;

            public Image roomName;
            public Text roomDurationLabel;

            public Image roomBanner;

            public Text prizeTitleLabel;
            public Text prizeDoubleLabel;
            public Text prizeMultiplierLabel;

            public Text entryFeeTitleLabel;
            public Text entryFeeLabel;

            public Text roomTitleLabel;
            public Text trophyCountLabel;

            public Text winsNeededTitleLabel;
            public Image trophyProgressFill;
            public Text trophyWinsNeededLabel;

            public Image infoBg;
            public Button infoButton;

            public GameObject roomCard;
            public GameObject lockedItems;

            public Text unlockLevelText;
        }

        [System.Serializable]
        public class _RotatingRoomCard
        {
            public Image background;

            public Image roomName;
            public Text roomDurationLabel;

            public Image roomBanner;

            public Text prizeTitleLabel;
            public Text prizeAmountLabel;

            public Text entryFeeTitleLabel;
            public Text entryFeeLabel;

            public Text rewardedGoalsLabel;
            public Text progressLabel;

            public Text timeRemainingLabel;

            public Image goalsButtonBg;
            public Button goalsButton;

            public Image infoBg;
            public Button infoButton;

            public GameObject roomCard;
            public GameObject lockedItems;

            public Text unlockLevelText;

            public GameObject nextRoomCountdown; 
            public Text nextRoomText;
            public Text nextRoomCountdownText;
        }

        [System.Serializable]
        public class _RoomInfo
        {
            [System.Serializable]
            public class _RoomTitle
            {
                public Text trophiesNeededLabel;
                public Text roomTitleLabel;
            }

            public GameObject card;
            public Image background;
            public Text rulesLabel;
            public _RoomTitle roomTitle1;
            public _RoomTitle roomTitle2;
            public _RoomTitle roomTitle3;
            public Button backButton;
        }

        [System.Serializable]
        public class _RotatingRoomInfo
        {
            public GameObject card;
            public Image background;
            public Text description;
            public Button backButton;
        }

        public GameObject roomCard;
        public Button roomButton;

        public _StandardRoomCard standardRoomCard;

        public _AllInOneRoomCard allInOneCard;

        public _RotatingRoomCard rotatingRoomCard;

        public _RotatingRoomInfo rotatingRoomInfoCard;

        public _RoomInfo AllInOneInfoCard;

        public _RoomInfo infoCard;

        public GameObject lockedCard;
        public GameObject mysteryCard;
        public GameObject TapBlocker;
    }
}
