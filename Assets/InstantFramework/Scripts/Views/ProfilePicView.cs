/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantGame
{
    public class ProfilePicView : View
    {
        public Sprite defaultAvatar;
        public Sprite whiteAvatar;
        public Image profilePic;
        public Image avatarBG;
        public Image avatarIcon;
        public Image leagueBorder;

        //public GameObject noProfilePicBorder;
        //public GameObject hasProfilePicBorder;
        //public GameObject premiumBorder;

        private string playerId;
        private SpritesContainer defaultAvatarContainer;

        public Text profileName;
        public Button profilePicButton;

        //Signals
        public Signal profilePicButtonClickedSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            if (profilePicButton != null)
            {
                profilePicButton.onClick.AddListener(OnProfilePicButtonClicked);
            }

            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
        }

        public void UpdateView(ProfileVO vo)
        {
            if (profileName != null)
            {
                profileName.text = vo.playerName;
            }

            playerId = vo.playerId;
            SetProfilePic(vo);
        }

        public void FacebookAuthResult(AuthFacebookResultVO vo)
        {
            if (vo.isSuccessful)
            {
                if (vo.pic != null)
                {
                    SetProfilePic(vo.pic);
                }
                if (profileName != null)
                {
                    profileName.text = vo.name;
                }
            }
        }

        public void SignInWithAppleResult(AuthSignInWIthAppleResultVO vo)
        {
            if (vo.isSuccessful)
            {
                if (profileName != null)
                {
                    profileName.text = vo.name;
                }
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateProfilePic(Photo vo)
        {
            if (vo.sprite == null)
                Debug.Log("UpdateProfilePic sprite is null");

            if (vo.sprite != null)
            {
                SetProfilePic(vo.sprite);
            }
        }

        private void SetProfilePic(Sprite sprite)
        {
            Debug.Log("OnProfilePicUpdate SetProfilePic");
            //noProfilePicBorder.SetActive(false);
            //hasProfilePicBorder.SetActive(false);
            avatarIcon.gameObject.SetActive(false);
            avatarBG.gameObject.SetActive(false);
            profilePic.color = Color.white;

            if (sprite == null)
            {
                profilePic.sprite = defaultAvatar;
                //noProfilePicBorder.SetActive(true);
            }
            else
            {
                profilePic.sprite = sprite;
                //hasProfilePicBorder.SetActive(true);
            }
        }

        private void SetProfilePic(ProfileVO vo)
        {
            //noProfilePicBorder.SetActive(false);
            //hasProfilePicBorder.SetActive(false);
            avatarBG.gameObject.SetActive(false);
            avatarIcon.gameObject.SetActive(false);
            ShowPremiumBorder(vo.isPremium);

            if (leagueBorder != null)
            {
                leagueBorder.gameObject.SetActive(vo.leagueBorder != null);
                leagueBorder.sprite = vo.leagueBorder;
            }

            if (vo.playerPic != null)
            {
                profilePic.sprite = vo.playerPic;
                //hasProfilePicBorder.SetActive(true);
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

                //noProfilePicBorder.SetActive(true);
            }

        }

        private void OnProfilePicButtonClicked()
        {
            profilePicButtonClickedSignal.Dispatch();
        }

        public void ShowPremiumBorder(bool show)
        {
            //premiumBorder.SetActive(show);
        }
    }
}