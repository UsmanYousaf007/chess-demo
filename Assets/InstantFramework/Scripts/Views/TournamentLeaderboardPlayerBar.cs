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

        [HideInInspector]
        public TournamentEntry entry;
        [HideInInspector]
        public TournamentReward reward;

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
    }
}
