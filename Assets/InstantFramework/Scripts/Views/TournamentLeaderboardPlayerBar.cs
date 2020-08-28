using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class TournamentLeaderboardPlayerBar : MonoBehaviour
    {
        public Text playerNameText;
        public Text playerScoreCountText;
        public Text playerRankCountText;
        public Text trophiesRewardCountText;

        public Image rankIcon;

        public Button button;
        public Button chestButton;

        public GameObject profile;

        private TournamentEntry entry;
        private TournamentReward reward;

        [Inject] NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] UpdateChestInfoDlgViewSignal UpdateChestInfoDlgViewSignal { get; set; }


        public void Populate(TournamentEntry tournamentEntry, TournamentReward entryReward)
        {
            entry = tournamentEntry;
            this.reward = entryReward;

            playerRankCountText.text = tournamentEntry.rank.ToString();
            trophiesRewardCountText.text = entryReward.trophies.ToString();

            playerNameText.text = entry.publicProfile.name;
            playerScoreCountText.text = entry.score.ToString();
            profile.SetActive(true);
        }

        public void Populate(int rank, TournamentReward entryReward)
        {
            entry = null;

            playerRankCountText.text = rank.ToString();
            trophiesRewardCountText.text = entryReward.trophies.ToString();

            playerNameText.gameObject.SetActive(false);
            profile.SetActive(false);
        }

        public void OnChestButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CHEST_INFO_DLG);
            UpdateChestInfoDlgViewSignal.Dispatch(reward);
        }
    }
}
