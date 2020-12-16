using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.InstantGame;
using TMPro;

namespace TurboLabz.InstantFramework
{
    [System.CLSCompliant(false)]
    public class LeaderboardPlayerBar : MonoBehaviour
    {
        public Text playerNameText;
        public Text playerScoreCountText;
        public Text playerRankCountText;
        public Text trophiesRewardCountText;

        public Image rankIcon;

        public Button button;
        public Button chestButton;
        public Image chestImage;
        public Image gemImage;
        public TMP_Text rewardText;

        public Sprite[] rankSprites;

        public DisplayPictureView profile;
        public GameObject playerPanel;

        public SkinLink skinLink;

        [HideInInspector]
        public TournamentEntry entry;
        [HideInInspector]
        public LeaderboardReward reward;

        public void Populate(TournamentEntry _entry, LeaderboardReward entryReward, bool isPlayer)
        {
            entry = _entry;
            this.reward = entryReward;

            playerRankCountText.text = entry.rank.ToString();
            trophiesRewardCountText.text = entryReward.trophies.ToString();

            playerNameText.text = !isPlayer ? entry.publicProfile.name : "You";
            playerNameText.gameObject.SetActive(true);
            //playerPanel.SetActive(isPlayer);

            playerScoreCountText.text = entry.score.ToString();

            profile.gameObject.SetActive(true);
            profile.UpdateView(entry.publicProfile);

            if (entryReward.chestType == null || entryReward.chestType == "")
            {
                chestImage.enabled = false;
                chestButton.interactable = false;
                rewardText.text = "X" + entryReward.gems;
            }
            else
            {
                ChestIconsContainer container = ChestIconsContainer.Load();
                chestImage.sprite = container.GetChest(entryReward.chestType);
                chestImage.enabled = true;
                chestButton.interactable = true;
                rewardText.text = "Level " + entryReward.quantity;
            }

            SetRankIcon(entry.rank);
            skinLink.InitPrefabSkin();
        }

        public void SetRankIcon(int rank)
        {
            rankIcon.enabled = true;
            playerRankCountText.enabled = false;
            gemImage.enabled = false;
            chestImage.enabled = true;

            if (rank == 1)
            {
                rankIcon.sprite = rankSprites[rank - 1];
            }
            else if (rank == 2)
            {
                rankIcon.sprite = rankSprites[rank - 1];
            }
            else if (rank == 3)
            {
                rankIcon.sprite = rankSprites[rank - 1];
            }
            else
            {
                rankIcon.enabled = false;
                playerRankCountText.enabled = true;
                gemImage.enabled = true;
                chestImage.enabled = false;
            }
        }

        public void Populate(int rank, LeaderboardReward entryReward)
        {
            entry = null;
            this.reward = entryReward;

            playerRankCountText.text = rank.ToString();
            trophiesRewardCountText.text = entryReward.trophies.ToString();

            playerNameText.gameObject.SetActive(false);
            profile.gameObject.SetActive(false);

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

            SetRankIcon(entry.rank);
            skinLink.InitPrefabSkin();
        }
    }
}
