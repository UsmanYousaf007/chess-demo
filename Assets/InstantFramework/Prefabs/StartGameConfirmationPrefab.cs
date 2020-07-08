using System;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class StartGameConfirmationPrefab : MonoBehaviour
    {
        public Button confirmGameCloseBtn;
        public Image opponentProfilePic;
        public Image opponentAvatarBg;
        public Image opponentAvatarIcon;
        public Text opponentProfileName;
        public Text opponentEloLabel;
        public Image opponentFlag;
        public Button confirmRankedGameBtn;
        public Text confirmRankedGameBtnText;
        public Button confirmFriendly1MinGameBtn;
        public Text confirmFriendly1MinGameBtnText;
        public Button confirmFriendly5MinGameBtn;
        public Text confirmFriendly5MinGameBtnText;
        public Button confirmFriendly10MinGameBtn;
        public Text confirmFriendly10MinGameBtnText;
        public Button confirmFriendly30MinGameBtn;
        public Text confirmFriendly30MinGameBtnText;
        public Button ToggleRankButton;
        public GameObject ToggleRankON;
        public GameObject ToggleRankOFF;
        public Image onlineStatus;
        public Text opponentActivityText;
        public GameObject premiumBorder;
        public Text startGameText;
        public Button tooltipBtn;
        public GameObject tooltip;

        [HideInInspector] public bool toggleRankButtonState;
        [HideInInspector] public string playerId;

        private void OnEnable()
        {
            tooltip.SetActive(false);
        }
    }
}
