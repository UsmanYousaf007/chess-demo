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
        public Text ticketsCountText;
        public Button enterButton;

        public Text enterButtonFreePlayLabel;
        public Text enterButtonTicketPlayLabel;

        public Text enterButtonTicketPlayCountText;

        public GameObject freePlayButtonGroup;
        public GameObject ticketPlayButtonGroup;

        [HideInInspector]
        public Signal enterButtonClickedSignal = new Signal();

        private void Awake()
        {
            enterButton.onClick.AddListener(() => enterButtonClickedSignal.Dispatch());
        }

    }
}
