/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using DG.Tweening;
using UnityEngine;
using System;
using TurboLabz.InstantGame;
using TMPro;
using DG.Tweening;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class TopNavView : View
    {
        // Services
        [Inject] public IAudioService audioService { get; set; }

        public Button supportButton;
        public Button settingsButton;
        public Button addGemsButton;
        public Button addCoinsButton;
        public Button addCollectilesButton;
        public Button inboxButton;
        public Text gemsCount;
        public Text coinsCount;
        public Text boughtGemsCount;
        public Text boughtCoinsCount;
        public Text messagesCount;
        public Image inboxNotification;

        public Text ticketsCount;
        public Text ratingBoostersCount;
        public Text hintsCount;
        public Text keysCount;

        public Signal settingsButtonClickedSignal = new Signal();
        public Signal supportButtonClicked = new Signal();
        public Signal addGemsButtonClickedSignal = new Signal();
        public Signal inboxButtonClickedSignal = new Signal();
        public Signal addCollectilesButtonClickedSignal = new Signal();
        public Signal addCoinsButtonClickedSignal = new Signal();

        private Color originalColor;
        private long totalCoins;

        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }

        public RectTransform gems;
        public TextMeshProUGUI textGems;
        public Transform startPivot;
        public Transform endPivot;

        [Tooltip("Color to fade from")]
        [SerializeField]
        private Color StartColor = Color.white;

        public void GemsAddedAnimation()
        {
            textGems.text = "+" + rewardsSettingsModel.personalisedAdsGemReward;
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            textGems.color = StartColor;
            textGems.gameObject.transform.position = startPivot.position;
            gems.gameObject.SetActive(true);
            StartCoroutine(GemsAddedCR());
        }

        IEnumerator GemsAddedCR()
        {
            yield return new WaitForSeconds(0.5f);

            textGems.DOFade(0f, 4.5f);
            textGems.transform.DOMoveY(endPivot.position.y, 4.5f);

            yield return new WaitForSeconds(6.2f);

            gems.gameObject.SetActive(false);
        }

        public void Init()
        {
            supportButton.onClick.AddListener(OnSupportButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            addGemsButton.onClick.AddListener(OnAddGemsButtonClicked);
            inboxButton.onClick.AddListener(OnInboxButtonClicked);
            addCollectilesButton.onClick.AddListener(OnAddCollectiblesButtonClicked);
            addCoinsButton.onClick.AddListener(OnAddGemsButtonClicked);
            originalColor = Colors.WHITE;

            if (boughtGemsCount != null)
            {
                boughtGemsCount.gameObject.SetActive(false);
            }

            if (boughtCoinsCount != null)
            {
                boughtCoinsCount.gameObject.SetActive(false);
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

        private void OnAddCoinsButtonClicked()
        {
            audioService.PlayStandardClick();
            addCoinsButtonClickedSignal.Dispatch();
        }

        private void OnInboxButtonClicked()
        {
            audioService.PlayStandardClick();
            inboxButtonClickedSignal.Dispatch();
        }

        private void OnAddCollectiblesButtonClicked()
        {
            audioService.PlayStandardClick();
            addCollectilesButtonClickedSignal.Dispatch();
        }

        public void UpdateGemsCount(long gems)
        {
            if (boughtGemsCount != null && gameObject.activeInHierarchy)
            {
                var addedGems = gems - long.Parse(gemsCount.text);

                if (addedGems > 0)
                {
                    boughtGemsCount.text = $"+{addedGems}";
                    boughtGemsCount.transform.localPosition = Vector3.zero;
                    boughtGemsCount.gameObject.SetActive(true);
                    DOTween.ToAlpha(() => boughtGemsCount.color, x => boughtGemsCount.color = x, 0.0f, 3.0f).OnComplete(OnGemsFadeComplete);
                    boughtGemsCount.transform.DOMoveY(Screen.height, 3.0f);
                }
            }

            gemsCount.text = gems.ToString();
        }

        public void UpdateCoinsCount(long coins)
        {
            if (boughtCoinsCount != null && gameObject.activeInHierarchy)
            {
                var addedCoins = coins - totalCoins;

                if (addedCoins > 0)
                {
                    boughtCoinsCount.text = $"+{addedCoins}";
                    boughtCoinsCount.transform.localPosition = Vector3.zero;
                    boughtCoinsCount.gameObject.SetActive(true);
                    DOTween.ToAlpha(() => boughtCoinsCount.color, x => boughtCoinsCount.color = x, 0.0f, 3.0f).OnComplete(OnCoinsFadeComplete);
                    boughtCoinsCount.transform.DOMoveY(Screen.height, 3.0f);
                }
            }

            coinsCount.text = coins.ToString("N0");
            totalCoins = coins;
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

        private void OnGemsFadeComplete()
        {
            boughtGemsCount.color = originalColor;
            boughtGemsCount.gameObject.SetActive(false);
        }

        private void OnCoinsFadeComplete()
        {
            boughtCoinsCount.color = originalColor;
            boughtCoinsCount.gameObject.SetActive(false);
        }

        public void UpdateCollectiblesCount()
        {
            ticketsCount.text = playerModel.GetInventoryItemCount("SpecialItemTicket").ToString();
            ratingBoostersCount.text = playerModel.GetInventoryItemCount("SpecialItemRatingBooster").ToString();
            if (!playerModel.HasSubscription()) {
                keysCount.resizeTextMaxSize = 35;
                keysCount.fontSize = 35;
                keysCount.text = playerModel.GetInventoryItemCount("SpecialItemKey").ToString();
                hintsCount.resizeTextMaxSize = 35;
                hintsCount.fontSize = 35;
                hintsCount.text = playerModel.GetInventoryItemCount("SpecialItemHint").ToString();
            }
            else
            {
               
                keysCount.resizeTextMaxSize = 70;
                keysCount.fontSize = 70;
                keysCount.text = "∞";
                hintsCount.resizeTextMaxSize = 70;
                hintsCount.fontSize = 70;
                hintsCount.text = "∞";
            }
    
        }

        public void UpdateRatingBoostersCount(long count)
        {
            ratingBoostersCount.text = count.ToString();
        }

        public void UpdateKeysCount(long count)
        {
            keysCount.text = count.ToString();
        }

        public void UpdateHintsCount(long count)
        {
            hintsCount.text = count.ToString();
        }
    }
}