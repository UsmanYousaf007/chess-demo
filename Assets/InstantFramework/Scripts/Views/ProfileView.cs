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
using strange.extensions.signal.impl ;

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantGame
{
    public class ProfileView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Button facebookButton;
        public Text facebookButtonLabel;
        public GameObject facebookConnectAnim;

        public Sprite defaultAvatar;
        public Image profilePic;
        public Text profileName;
        public GameObject noProfilePicBorder;
        public GameObject hasProfilePicBorder;
        public Text eloScoreLabel;
        public Text eloScoreValue;
        public Image playerFlag;

        public Signal facebookButtonClickedSignal = new Signal();

        public void Init()
        {
            if (facebookButton != null)
            {
                facebookButton.onClick.AddListener(OnFacebookButtonClicked);
                facebookConnectAnim.SetActive(false);
            }

            eloScoreLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
        }

        public void CleanUp()
        {
            if (facebookButton != null)
            {
                facebookButton.onClick.RemoveAllListeners();
            }
        }

        public void UpdateView(ProfileVO vo)
        {
            if (facebookButtonLabel != null)
            {
                facebookButtonLabel.text = localizationService.Get(LocalizationKey.FACEBOOK_LOGIN);
            }

            profileName.text = vo.playerName;
            eloScoreValue.text = vo.eloScore.ToString();
            playerFlag.sprite = Flags.GetFlag(vo.countryId);

            SetProfilePic(vo.playerPic);

            if (facebookButton != null)
            {
                facebookButton.gameObject.SetActive(!vo.isFacebookLoggedIn);
                facebookConnectAnim.SetActive(false);
            }
        }

        public void FacebookAuthResult(bool isSuccessful, Sprite pic, string name)
        {
            if (isSuccessful)
            {
                SetProfilePic(pic);
                profileName.text = name;

                if (facebookButton != null)
                {
                    facebookButton.gameObject.SetActive(false);
                }
            }
            else
            {
                if (facebookButton != null)
                {
                    facebookButton.enabled = true;
                }
            }

            if (facebookButton != null)
            {
                facebookConnectAnim.SetActive(false);
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

        private void SetProfilePic(Sprite sprite)
        {
            noProfilePicBorder.SetActive(false);
            hasProfilePicBorder.SetActive(false);

            if (sprite == null)
            {
                profilePic.sprite = defaultAvatar;
                noProfilePicBorder.SetActive(true);
            }
            else
            {
                profilePic.sprite = sprite;
                hasProfilePicBorder.SetActive(true);
            }
        }

        private void OnFacebookButtonClicked()
        {
            facebookButtonClickedSignal.Dispatch();
            facebookConnectAnim.SetActive(true);
            facebookButton.enabled = false;
        }
    }
}