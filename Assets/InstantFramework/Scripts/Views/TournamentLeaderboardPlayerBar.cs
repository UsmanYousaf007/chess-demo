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

        public void Populate(TournamentEntry tournamentEntry, int trophies)
        {
            entry = tournamentEntry;

            playerRankCountText.text = tournamentEntry.rank.ToString();
            trophiesRewardCountText.text = trophies.ToString();

            playerNameText.text = entry.publicProfile.name;
            playerScoreCountText.text = entry.score.ToString();
            profile.SetActive(true);
        }

        public void Populate(int rank, int trophies)
        {
            entry = null;

            playerRankCountText.text = rank.ToString();
            trophiesRewardCountText.text = trophies.ToString();

            playerNameText.gameObject.SetActive(false);
            profile.SetActive(false);
        }
    }
}
