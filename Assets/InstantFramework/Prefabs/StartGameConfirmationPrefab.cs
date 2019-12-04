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
        public Button confirmFriendlyGameBtn;
        public Text confirmFriendlyGameBtnText;
        public Button ToggleRankButton;
        public GameObject ToggleRankON;
        public GameObject ToggleRankOFF;
        public Image onlineStatus;
        public Text opponentStatusText;

        [HideInInspector] public bool toggleRankButtonState;
        [HideInInspector] public string playerId;
    }
}
