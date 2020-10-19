using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class LeagueTierInfo : MonoBehaviour
    {
        public string leagueID;
        public Text titleText;
        public Text tagText;
        public TMP_Text youText;
        public Text dailyChestText;

        public Image borderImage;
        public Image trophyImage;
        public Image bgImage;
        public Image chestImage;
        public Image titleTextUnderlayImage;
        public Image defaultAvatar;
        public Image picBorder;

        //profile pic
        public Image profilePic;
        public Sprite defaultAvatarSprite;

        private SpritesContainer defaultAvatarContainer;

        LeagueTierIconsContainer.LeagueAsset leagueAsset;

        public void SetLeagueInfo(LeagueTierIconsContainer.LeagueAsset asset, League league)
        {
            leagueAsset = asset;
            titleText.text = league.name.ToUpper();
            trophyImage.sprite = leagueAsset.trophySprite;
            bgImage.sprite = leagueAsset.bgSprite;
            chestImage.sprite = leagueAsset.chestSprite;
            borderImage.color = leagueAsset.borderColor;
            titleTextUnderlayImage.sprite = leagueAsset.textUnderlaySprite;
            defaultAvatar.gameObject.SetActive(false);
            profilePic.sprite = defaultAvatarSprite;
            dailyChestText.text = $"Daily {league.name} Chest";
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            borderImage.enabled = false;
            youText.gameObject.SetActive(false);
            defaultAvatar.gameObject.SetActive(true);
            picBorder.sprite = leagueAsset.ringSprite;
            picBorder.SetNativeSize();
        }

        public void UpdateView(bool isPlayerLeague)
        {
            if (isPlayerLeague)
            {
                borderImage.enabled = true;
                youText.gameObject.SetActive(true);
                defaultAvatar.gameObject.SetActive(false);
            }
            else
            {
                borderImage.enabled = false;
                youText.gameObject.SetActive(false);
                defaultAvatar.gameObject.SetActive(true);
            }
        }
    }
}
