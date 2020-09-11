/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using DG.Tweening;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class TopNavView : View
    {
        // Services
        [Inject] public IAudioService audioService { get; set; }

        public Button supportButton;
        public Button settingsButton;
        public Button addGemsButton;
        public Button inboxButton;
        public Text gemsCount;
        public Text boughtGemsCount;
        public Text messagesCount;
        public Image inboxNotification;

        public Signal settingsButtonClickedSignal = new Signal();
        public Signal supportButtonClicked = new Signal();
        public Signal addGemsButtonClickedSignal = new Signal();
        public Signal inboxButtonClickedSignal = new Signal();

        private Color originalColor;

        public void Init()
        {
            supportButton.onClick.AddListener(OnSupportButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            addGemsButton.onClick.AddListener(OnAddGemsButtonClicked);
            inboxButton.onClick.AddListener(OnInboxButtonClicked);

            if (boughtGemsCount != null)
            {
                boughtGemsCount.gameObject.SetActive(false);
                originalColor = boughtGemsCount.color;
            }
        }

        private void OnSupportButtonClicked()
        {
            audioService.PlayStandardClick();
            supportButtonClicked.Dispatch();
        }

        private void OnSettingsButtonClicked()
        {
            audioService.PlayStandardClick();
            settingsButtonClickedSignal.Dispatch();
        }

        private void OnAddGemsButtonClicked()
        {
            audioService.PlayStandardClick();
            addGemsButtonClickedSignal.Dispatch();
        }

        private void OnInboxButtonClicked()
        {
            audioService.PlayStandardClick();
            inboxButtonClickedSignal.Dispatch();
        }

        public void UpdateGemsCount(long gems)
        {
            if (boughtGemsCount != null && gameObject.activeInHierarchy)
            {
                boughtGemsCount.text = $"+{gems - long.Parse(gemsCount.text)}";
                boughtGemsCount.transform.localPosition = Vector3.zero;
                boughtGemsCount.gameObject.SetActive(true);
                DOTween.ToAlpha(() => boughtGemsCount.color, x => boughtGemsCount.color = x, 0.0f, 3.0f).OnComplete(() => OnFadeComplete(gems));
                boughtGemsCount.transform.DOMoveY(Screen.height, 3.0f);
            }

            gemsCount.text = gems.ToString();
        }

        public void UpdateMessagesCount(long messages)
        {
            if (messages > 0)
            {
                messagesCount.text = messages.ToString();
                messagesCount.enabled = true;
                inboxNotification.enabled = true;
            }
            else
            {
                inboxNotification.enabled = false;
                messagesCount.enabled = false;
            }
        }

        private void OnFadeComplete(long gems)
        {
            boughtGemsCount.color = originalColor;
            boughtGemsCount.gameObject.SetActive(false);
        }
    }
}