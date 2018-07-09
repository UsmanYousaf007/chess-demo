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
    public class OpponentProfileView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Image profilePic;
        public Text profileName;
        public GameObject noProfilePicBorder;
        public GameObject hasProfilePicBorder;
        public Text eloScoreLabel;
        public Text eloScoreValue;
        public Image playerFlag;

        public void Init()
        {
            eloScoreLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
        }

        public void CleanUp()
        {
        }

        public void UpdateView(ProfileVO vo)
        {
            profileName.text = vo.playerName;
            eloScoreValue.text = vo.eloScore.ToString();
            playerFlag.sprite = Flags.GetFlag(vo.countryId);

            SetProfilePic(vo.playerPic);
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
                noProfilePicBorder.SetActive(true);
            }
            else
            {
                profilePic.sprite = sprite;
                hasProfilePicBorder.SetActive(true);
            }
        }
    }
}