using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.InstantGame;

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
        public Image chestImage;

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
            playerNameText.gameObject.SetActive(true);

            playerScoreCountText.text = entry.score.ToString();

            profile.SetActive(true);

            if (entryReward.chestType == null)
            {
                chestImage.enabled = false;
                chestButton.interactable = false;
            }
            else
            {
                ChestIconsContainer container = ChestIconsContainer.Load();
                chestImage.sprite = container.GetChest(entryReward.chestType);
                chestImage.enabled = true;
                chestButton.interactable = true;
            }
        }

        public void Populate(int rank, TournamentReward entryReward)
        {
            entry = null;

            playerRankCountText.text = rank.ToString();
            trophiesRewardCountText.text = entryReward.trophies.ToString();

            playerNameText.gameObject.SetActive(false);
            profile.SetActive(false);

            rankIcon.enabled = true;
            if(rank == 1) {
                rankIcon.color = Colors.GOLD;
            }
            else if (rank == 2){
                rankIcon.color = Colors.SILVER;
            }
            else if (rank == 3){
                rankIcon.color = Colors.BRONZE;
            }
            else{
                rankIcon.enabled = false;
            }

            if (entryReward.chestType == null)
            {
                chestImage.enabled = false;
                chestButton.interactable = false;
            }
            else
            {
                ChestIconsContainer container = ChestIconsContainer.Load();
                chestImage.sprite = container.GetChest(entryReward.chestType);
                chestImage.enabled = true;
                chestButton.interactable = true;
            }
        }
    }
}
