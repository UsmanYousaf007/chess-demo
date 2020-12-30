using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;

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
        public Image flagImage;
        public TMP_Text rewardText;

        public Sprite[] rankSprites;

        public DisplayPictureView profile;
        public GameObject playerPanel;

        public SkinLink skinLink;

        private TournamentEntry _entry;
        private TournamentReward _reward;

        public void Populate(TournamentEntry entry, TournamentReward entryReward, bool isPlayer)
        {
            _entry = entry;
            _reward = entryReward;

            playerRankCountText.text = _entry.rank.ToString();
            trophiesRewardCountText.text = entryReward.trophies.ToString();

            playerNameText.text = !isPlayer ? _entry.publicProfile.name : "You";
            playerNameText.gameObject.SetActive(true);
            //playerPanel.SetActive(isPlayer);

            playerScoreCountText.text = _entry.score.ToString();

            profile.gameObject.SetActive(true);
            profile.UpdateView(_entry.publicProfile);

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

            SetRankIcon(_entry.rank);
            //skinLink.InitPrefabSkin();
        }

        public void Populate(AllStarLeaderboardEntry entry, bool isPlayer)
        {
            playerRankCountText.text = entry.rank.ToString();

            playerNameText.text = !isPlayer ? entry.name : "You";
            playerNameText.gameObject.SetActive(true);

            playerScoreCountText.text = entry.score.ToString();

            profile.gameObject.SetActive(true);

            PublicProfile profileVO = new PublicProfile();
            profileVO.countryId = entry.countryId;
            profileVO.playerId = entry.playerId;
            profileVO.league = entry.league;
            profileVO.name = entry.name;
            profileVO.uploadedPicId = entry.uploadedPicId;
            profile.UpdateView(profileVO);

            if (flagImage != null)
            {
                flagImage.sprite = Flags.GetFlag(entry.countryId);
            }

            SetRankIcon(entry.rank);
            //skinLink.InitPrefabSkin();
        }

        private void SetRankIcon(int rank)
        {
            rankIcon.enabled = true;
            playerRankCountText.enabled = false;

            if (gemImage != null)
            {
                gemImage.enabled = false;
            }
            if (chestImage != null)
            {
                chestImage.enabled = true;
            }

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

                if (gemImage != null)
                {
                    gemImage.enabled = true;
                }
                if (chestImage != null)
                {
                    chestImage.enabled = false;
                }
            }
        }
    }
}
