﻿using System.Collections;
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
        public string leagueType;
        public Text titleText;
        public Text tagText;
        public TMP_Text youText;

        public Image borderImage;
        public Image trophyImage;
        public Image bgImage;
        public Image chestImage;
        public Image titleTextUnderlayImage;

        //profile pic
        public Image profilePic;
        public Image avatarBG;
        public Image avatarIcon;

        public Sprite whiteAvatar;
        public Sprite defaultAvatar;

        private SpritesContainer defaultAvatarContainer;

        LeagueTierIconsContainer.LeagueAsset leagueAsset;

        public void SetLeagueInfo(LeagueTierIconsContainer.LeagueAsset asset)
        {
            leagueAsset = asset;
            titleText.text = leagueAsset.typeName;
            trophyImage.sprite = leagueAsset.trophySprite;
            bgImage.sprite = leagueAsset.bgSprite;
            chestImage.sprite = leagueAsset.chestSprite;
            borderImage.color = leagueAsset.borderColor;
            titleTextUnderlayImage.sprite = leagueAsset.textUnderlaySprite;
            profilePic.sprite = defaultAvatar;
        }

        public void UpdateView(bool isPlayerLeague, ProfileVO vo)
        {
            if(isPlayerLeague)
            {
                SetProfilePic(vo);
                borderImage.enabled = true;
                youText.gameObject.SetActive(true);
            }
            else {
                borderImage.enabled = false;
                youText.gameObject.SetActive(false);
                profilePic.sprite = defaultAvatar;
            }
        }

        private void SetProfilePic(ProfileVO vo)
        {
            avatarBG.gameObject.SetActive(false);
            avatarIcon.gameObject.SetActive(false);

            if (vo.playerPic != null)
            {
                profilePic.sprite = vo.playerPic;
            }
            else
            {
                profilePic.sprite = defaultAvatar;

                if (vo.avatarId != null)
                {
                    Sprite newSprite = defaultAvatarContainer.GetSprite(vo.avatarId);
                    if (newSprite != null)
                    {
                        avatarIcon.gameObject.SetActive(true);
                        avatarBG.gameObject.SetActive(true);
                        avatarIcon.sprite = newSprite;
                        avatarBG.sprite = whiteAvatar;
                        avatarBG.color = Colors.Color(vo.avatarColorId);
                    }
                }
            }

        }

    }
}
