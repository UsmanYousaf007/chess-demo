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
        public Image playerIndicator;

        public Button button;
        public Button chestButton;
        public Image chestImage;
        public Image gemImage;
        public Image trophyImage;
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
            playerScoreCountText.text = TLUtils.FormatUtil.AbbreviateNumber(entry.score);
            rewardText.text = entryReward.gems.ToString();

            // Playar Name
            playerNameText.text = !isPlayer ? _entry.publicProfile.name : "You";
            playerNameText.gameObject.SetActive(true);

            // Trophies
            trophiesRewardCountText.gameObject.SetActive(entryReward.trophies != 0);
            trophyImage.gameObject.SetActive(entryReward.trophies != 0);

            // Gems
            rewardText.gameObject.SetActive(entryReward.gems != 0);
            gemImage.gameObject.SetActive(entryReward.gems != 0);

            profile.gameObject.SetActive(true);
            profile.UpdateView(_entry.publicProfile);

            // Chest
            chestImage.gameObject.SetActive(false);
            chestImage.enabled = false;
            chestButton.interactable = false;

            // Rank
            SetRankIcon(_entry.rank);

            // Player special
            playerIndicator.gameObject.SetActive(isPlayer);

            skinLink.InitPrefabSkin();
        }

        public void Populate(AllStarLeaderboardEntry entry, bool isPlayer)
        {
            playerRankCountText.text = entry.rank.ToString();

            playerNameText.text = !isPlayer ? entry.name : "You";
            playerNameText.gameObject.SetActive(true);

            playerScoreCountText.text = TLUtils.FormatUtil.AbbreviateNumber(entry.score);
            

            profile.gameObject.SetActive(true);
            profile.UpdateView(entry.publicProfile);

            if (flagImage != null)
            {
                flagImage.sprite = Flags.GetFlag(entry.publicProfile.countryId);
            }

            SetRankIcon(entry.rank);

            // Player special
            playerIndicator.gameObject.SetActive(isPlayer);

            skinLink.InitPrefabSkin();
        }

        private void SetRankIcon(int rank)
        {
            rankIcon.enabled = false;
            playerRankCountText.enabled = true;

            if (rank == 1)
            {
                rankIcon.enabled = true;
                rankIcon.sprite = rankSprites[rank - 1];
                playerRankCountText.enabled = false;
            }
            else if (rank == 2)
            {
                rankIcon.enabled = true;
                rankIcon.sprite = rankSprites[rank - 1];
                playerRankCountText.enabled = false;
            }
            else if (rank == 3)
            {
                rankIcon.enabled = true;
                rankIcon.sprite = rankSprites[rank - 1];
                playerRankCountText.enabled = false;
            }
        }
    }
}
