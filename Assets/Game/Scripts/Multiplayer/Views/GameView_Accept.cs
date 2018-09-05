/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using DG.Tweening;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Accept or Decline")]
        public Button acceptButton;
        public Button declineButton;

        public Signal acceptButtonClickedSignal = new Signal();
        public Signal declineButtonClickedSignal = new Signal();

        public GameObject dialog;

        public Text titleLabel;
        public Text acceptButtonLabel;
        public Text declineButtonLabel;

        private float acceptDialogHalfHeight;
        private const float ACCEPT_DIALOG_DURATION = 0.5f;

        public void InitAccept()
        {
            acceptButton.onClick.AddListener(OnAcceptButtonClicked);
            declineButton.onClick.AddListener(OnDeclineButtonClicked);

            titleLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_ACCEPT_TITLE);
            acceptButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_ACCEPT_YES);
            declineButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_ACCEPT_NO);

            titleLabel.color = Colors.YELLOW;
            acceptDialogHalfHeight = dialog.GetComponent<RectTransform>().rect.height / 2f;
        }

        public void OnParentShowAccept()
        {
            HideAccept();
        }

        public void ShowAccept()
        {
            EnableModalBlocker();

            dialog.SetActive(true);
            declineButton.interactable = true;

            DisableMenuButton();

            dialog.transform.localPosition = new Vector3(0f, Screen.height + acceptDialogHalfHeight, 0f);
            dialog.transform.DOLocalMove(Vector3.zero, ACCEPT_DIALOG_DURATION).SetEase(Ease.OutBack);
            audioService.Play(audioService.sounds.SFX_DEFEAT);
        }

        public void HideAccept()
        {
            DisableModalBlocker();

            dialog.SetActive(false);

            EnableMenuButton();
        }

        void OnAcceptButtonClicked()
        {
            acceptButtonClickedSignal.Dispatch();
        }

        void OnDeclineButtonClicked()
        {
            declineButton.interactable = false;
            declineButtonClickedSignal.Dispatch();
        }
    }
}
