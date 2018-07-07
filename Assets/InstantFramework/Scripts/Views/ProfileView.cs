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
            facebookButton.onClick.AddListener(OnFacebookButtonClicked);
        }

        public void CleanUp()
        {
            facebookButton.onClick.RemoveAllListeners();
        }

        public void UpdateView(ProfileVO vo)
        {
            facebookButtonLabel.text = localizationService.Get(LocalizationKey.FACEBOOK_LOGIN);

            profileName.text = vo.playerName;
            eloScoreLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
            eloScoreValue.text = vo.eloScore.ToString();
            playerFlag.sprite = Flags.GetFlag(vo.countryId);

            SetProfilePic(vo.playerPic);

            if (vo.isFacebookLoggedIn)
            {
                facebookButton.gameObject.SetActive(false);
            }

            facebookConnectAnim.SetActive(false);
        }

        public void FacebookAuthResult(bool isSuccessful, Sprite pic, string name)
        {
            if (isSuccessful)
            {
                SetProfilePic(pic);
                profileName.text = name;
                facebookButton.gameObject.SetActive(false);
            }
            else
            {
                facebookButton.enabled = true;
            }

            facebookConnectAnim.SetActive(false);
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

        private void OnFacebookButtonClicked()
        {
            facebookButtonClickedSignal.Dispatch();
            facebookConnectAnim.SetActive(true);
            facebookButton.enabled = false;
        }

    }
}

/*
        public void UpdateDuration(CPULobbyVO vo)
        {
            int duration = vo.durationMinutes[vo.selectedDurationIndex];

            if (duration == 0)
            {
                infinityIcon.SetActive(true);
                currentDurationLabel.gameObject.SetActive(false);
            }
            else
            {
                infinityIcon.SetActive(false);
                currentDurationLabel.gameObject.SetActive(true);
                currentDurationLabel.text = 
                    localizationService.Get(LocalizationKey.GM_ROOM_DURATION, vo.durationMinutes[vo.selectedDurationIndex]);
            }

            if (vo.selectedDurationIndex == 0)
            {
                decDurationButton.interactable = false;
                incDurationButton.interactable = true;
            }
            else if (vo.selectedDurationIndex == (vo.durationMinutes.Length - 1))
            {
                decDurationButton.interactable = true;
                incDurationButton.interactable = false;
            }
            else
            {
                decDurationButton.interactable = true;
                incDurationButton.interactable = true;
            }
        }

        public void UpdatePlayerColor(CPULobbyVO vo)
        {
            randomKing.SetActive(false);
            whiteKing.SetActive(false);
            blackKing.SetActive(false);

            if (vo.selectedPlayerColorIndex == 0)
            {
                whiteKing.SetActive(true);
                incPlayerColorButton.interactable = true;
                decPlayerColorButton.interactable = false;
            }
            else if (vo.selectedPlayerColorIndex == 1)
            {
                blackKing.SetActive(true);
                incPlayerColorButton.interactable = true;
                decPlayerColorButton.interactable = true;
            }
            else if (vo.selectedPlayerColorIndex == 2)
            {
                randomKing.SetActive(true);
                incPlayerColorButton.interactable = false;
                decPlayerColorButton.interactable = true;
            }
        }

        public void UpdateTheme(CPULobbyVO vo)
        {
            currentThemeLabel.text = vo.activeSkinDisplayName;

            int index = vo.playerVGoods.IndexOf(vo.activeSkinId);

            if (index == (vo.playerVGoods.Count - 1)) 
            {
                incThemeButton.interactable = false;
                decThemeButton.interactable = true;
            } 
            else if (index == 0) 
            {
                incThemeButton.interactable = true;
                decThemeButton.interactable = false;
            } 
            else 
            {
                incThemeButton.interactable = true;
                decThemeButton.interactable = true;
            }
        }
  */      

/*
private void OnAudioIsOnButtonClicked()
{
    audioService.ToggleAudio(false);
    RefreshAudioButtons();
}

private void OnAudioIsOffButtonClicked()
{
    audioService.ToggleAudio(true);
    audioService.PlayStandardClick();
    RefreshAudioButtons();
}

private void RefreshAudioButtons()
{
    audioIsOnButton.gameObject.SetActive(audioService.IsAudioOn());
    audioIsOffButton.gameObject.SetActive(!audioService.IsAudioOn());
}
*/