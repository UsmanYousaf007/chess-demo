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

        public DisplayPictureView profile;
        public GameObject playerPanel;
        public SkinLink skinLink;

        [HideInInspector]
        public TournamentEntry entry;
        [HideInInspector]
        public TournamentReward reward;

        public void Populate(TournamentEntry tournamentEntry, TournamentReward entryReward, bool isPlayer)
        {
            entry = tournamentEntry;
            this.reward = entryReward;

            playerRankCountText.text = tournamentEntry.rank.ToString();
            trophiesRewardCountText.text = entryReward.trophies.ToString();

            playerNameText.text = !isPlayer ? entry.publicProfile.name : "You";
            playerNameText.gameObject.SetActive(true);
            playerPanel.SetActive(isPlayer);

            playerScoreCountText.text = entry.score.ToString();

            profile.gameObject.SetActive(true);
            profile.UpdateView(entry.publicProfile);

            if (entryReward.chestType == null || entryReward.chestType == "")
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

            SetRankIcon(tournamentEntry.rank);
            skinLink.InitPrefabSkin();
        }

        public void SetRankIcon(int rank)
        {
            rankIcon.enabled = true;

            if (rank == 1)
            {
                rankIcon.color = Colors.GOLD;
                playerRankCountText.color = Colors.GOLD;
            }
            else if (rank == 2)
            {
                rankIcon.color = Colors.SILVER;
                playerRankCountText.color = Colors.SILVER;
            }
            else if (rank == 3)
            {
                rankIcon.color = Colors.BRONZE;
                playerRankCountText.color = Colors.BRONZE;
            }
            else
            {
                rankIcon.enabled = false;
            }
        }

        public void Populate(int rank, TournamentReward entryReward)
        {
            entry = null;
            this.reward = entryReward;

            playerRankCountText.text = rank.ToString();
            trophiesRewardCountText.text = entryReward.trophies.ToString();

            playerNameText.gameObject.SetActive(false);
            profile.gameObject.SetActive(false);

            rankIcon.enabled = true;
            if (rank == 1)
            {
                rankIcon.color = Colors.GOLD;
                playerRankCountText.color = Colors.GOLD;
            }
            else if (rank == 2)
            {
                rankIcon.color = Colors.SILVER;
                playerRankCountText.color = Colors.SILVER;
            }
            else if (rank == 3)
            {
                rankIcon.color = Colors.BRONZE;
                playerRankCountText.color = Colors.BRONZE;
            }
            else
            {
                rankIcon.enabled = false;
            }

            if (entryReward.chestType == "")
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

            skinLink.InitPrefabSkin();
        }
    }
}
