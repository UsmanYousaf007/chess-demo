/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.Chess;
using TurboLabz.InstantGame;
using strange.extensions.promise.api;
using HUFEXT.CrossPromo.Runtime.API;
using TMPro;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Declined Dialog")]
        public GameObject declinedDialog;
        public Text declinedHeading;
        public Text declinedReason;
        public Button declinedLobbyButton;
        public Text declinedLobbyButtonLabel;

        [Header("Results Dialog")]
        public GameObject resultsDialog;

        public Image resultsGameImage;
        public Sprite winSprite;
        public Sprite defeatSprite;
        public Sprite drawSprite;

        public Image resultsGameResultLabel;
        public Sprite winText;
        public Sprite defeatText;
        public Sprite drawText;

        public Text resultsGameResultReasonLabel;
        public Text resultsFriendlyLabel;
        public Text resultsRatingValueLabel;
        public Text resultsRatingChangeLabel;
        public GameObject resultsRatingContainer;

        public Button resultsBoostRatingButton;
        public Text resultsBoostRatingAddedCount;
        public GameObject resultsBoostRatingToolTip;
        public Text resultsBoostRatingToolTipText;
        public Text resultsBoostRatingGemsCost;
        public TMP_Text resultsBoostRatingText;
        public Image resultsBoostRatingIcon;
        public Image resultsBoostRatingGemIcon;
        public GameObject resultsBoostSheen;
        public GameObject resultsBoostGlow;

        public Button resultsViewBoardButton;
        public Text resultsViewBoardButtonLabel;
        public ViewBoardResults viewBoardResultPanel;
        public Button showCrossPromoButton;

        public Text resultsRewardLabel;
        public Text resultsBetReversedLabel;
        public Text resultsEarnedCoinsLabel;
        public Text resultsEarnedStarsLabel;
        public GameObject resultsRewardsCoins;
        public GameObject resultsRewardsStars;

        public Button resultsDoubleRewardButton;
        public Text resultsDoubleRewardGemsCost;
        public Text resultsDoubleRewardText;
        public Image resultsDoubleRewardGemIcon;

        public Button resultsContinueButton;
        public Button resultsContinueButton2;

        public Image resultsPowerplayImage;
        public Sprite powerPlayOnSprite;
        public Sprite powerPlayOffSprite;

        public RectTransform[] resultsLayouts;

        public WinResultAnimSequence _winAnimationSequence;

        public Signal resultsDialogClosedSignal = new Signal();
        public Signal resultsDialogOpenedSignal = new Signal();
        public Signal backToLobbySignal = new Signal();
        public Signal<string> boostRatingSignal = new Signal<string>();
        public Signal refreshLobbySignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();
        public Signal backToArenaSignal = new Signal();
        public Signal<VirtualGoodsTransactionVO> doubleRewardSignal = new Signal<VirtualGoodsTransactionVO>();

        private const float RESULTS_DELAY_TIME = 1f;
        private const float RESULTS_SHORT_DELAY_TIME = 0.3f;
        private const float RESULTS_DIALOG_DURATION = 0.5f;
        private float resultsDialogHalfHeight;
        private float declinedDialogHalfHeight;
        private Tweener addedAnimation;
        private bool playerWins;
        private bool isDraw;
        private float animDelay;
        private string playerName;
        private string opponentName;
        private string challengeId;
        private Color originalColor;
        private StoreItem ratingBoosterStoreItem;
        private bool haveEnoughGemsForRatingBooster;
        private StoreItem rewardDoublerStoreItem;
        private bool haveEnoughGemsForRewardDoubler;
        private long resultsBetValue;
        private bool animationPlayed = false;

        [Inject] public IPreferencesModel preferencesModel { get; set; }
        
        public void InitResults()
        {
            // Declined dialog
            declinedLobbyButton.onClick.AddListener(OnResultsDeclinedButtonClicked);
            declinedHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DECLINED);
            declinedReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DECLINED);
            declinedLobbyButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_RESULTS_BACK);
            declinedDialogHalfHeight = declinedDialog.GetComponent<RectTransform>().rect.height / 2f;

            resultsBoostRatingButton.onClick.AddListener(OnResultsBoostRatingButtonClicked);
            resultsViewBoardButton.onClick.AddListener(OnResultsClosed);
            resultsContinueButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);
            resultsContinueButton2.onClick.AddListener(OnResultsSkipRewardButtonClicked);
            showCrossPromoButton.onClick.AddListener(OnCrossPromoButtonClicked);
            resultsDoubleRewardButton.onClick.AddListener(OnRewardDoublerClicked);
            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsViewBoardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);

            originalColor = resultsBoostRatingAddedCount.color;
        }

        public void CleanupResults()
        {
            declinedLobbyButton.onClick.RemoveAllListeners();
            resultsViewBoardButton.onClick.RemoveAllListeners();
            resultsContinueButton.onClick.RemoveAllListeners();
            resultsContinueButton2.onClick.RemoveAllListeners();
            showCrossPromoButton.onClick.RemoveAllListeners();
            resultsDoubleRewardButton.onClick.RemoveAllListeners();
        }

        public void OnParentShowResults()
        {
            HideResultsDialog();
        }

        public void ShowResultsDialog()
        {
            EnableModalBlocker(Colors.UI_BLOCKER_DARK_ALPHA);
            resultsDialog.SetActive(true);

            HidePossibleMoves();
            HideOpponentConnectionMonitor();

            if (!ArePlayerMoveIndicatorsVisible())
            {
                HidePlayerToIndicator();
            }

            HideSafeMoveBorder();
            ShowViewBoardResultsPanel(false);

            preferencesModel.isRateAppDialogueShown = false;

            if (HCrossPromo.service != null)
            {
                showCrossPromoButton.gameObject.SetActive(HCrossPromo.service.hasContent);
            }
            appInfoModel.gameMode = GameMode.NONE;
        }

        public void UpdateResultsDialog(ResultsVO vo)
        {
            DisableInteraction();

            if (vo.reason == GameEndReason.DECLINED)
            {
                HandleDeclinedDialog();
                return;
            }

            playerWins = vo.playerWins;
            playerName = vo.playerName;
            opponentName = vo.opponentName;
            isDraw = false;
            animDelay = RESULTS_DELAY_TIME;
            GameEndReason gameEndReason = vo.reason;
            ratingBoosterStoreItem = vo.ratingBoostStoreItem;
            rewardDoublerStoreItem = vo.rewardDoubleStoreItem;
            resultsBetValue = vo.betValue;
            challengeId = vo.challengeId;

            if (!animationPlayed)
            {
                var coinsRewarded = playerWins ? vo.betValue * (Settings.ABTest.COINS_TEST_GROUP.Equals(Settings.ABTest.COINS_TEST_GROUP_DEFAULT) ? 2.0f : 1.5f) : vo.betValue;
                _winAnimationSequence.Reset((long)coinsRewarded, vo.earnedStars, vo.powerMode == true ? vo.earnedStars : 0, playerWins, vo.isRanked);
            }

            UpdateGameEndReasonSection(vo.reason);
            UpdateResultRatingSection(vo.isRanked, vo.currentEloScore, vo.eloScoreDelta);
            UpdateGameResultHeadingSection();
            SetupResultsLayout();
            SetupBoostPrice();
            SetupRewardDoublerPrice();
            UpdateRewards(vo.betValue, vo.earnedStars, vo.powerMode);
            BuildLayout();

            resultsDialog.transform.localPosition = new Vector3(0f, Screen.height + resultsDialogHalfHeight, 0f);
            Invoke("AnimateResultsDialog", animDelay);

            // TODO: move this call to the clock partial class
            if (gameEndReason == GameEndReason.TIMER_EXPIRED)
            {
                if (playerWins)
                {
                    ExpireOpponentTimer();
                }
                else
                {
                    ExpirePlayerTimer();
                }
            }

            if (isRankedGame && !isDraw)
            {
                analyticsService.Event(AnalyticsEventId.booster_shown, AnalyticsContext.rating_booster);
            }
        }

        public void ShowViewBoardResultsPanel(bool show)
        {
            viewBoardResultPanel.gameObject.SetActive(show);
        }

        public void HideResultsDialog()
        {
            resultsDialog.SetActive(false);
            declinedDialog.SetActive(false);
        }

        private void UpdateResultRatingSection(bool isRanked, int currentEloScore, int eloScoreDelta)
        {
            resultsFriendlyLabel.gameObject.SetActive(false);
            resultsRatingValueLabel.gameObject.SetActive(false);
            resultsRatingChangeLabel.gameObject.SetActive(false);

            if (!isRanked)
            {
                resultsFriendlyLabel.gameObject.SetActive(true);
                SetupRatingBoostButton(false);
                return;
            }

            // Ranked Game
            resultsRatingValueLabel.gameObject.SetActive(true);
            resultsRatingChangeLabel.gameObject.SetActive(false);
            resultsRatingValueLabel.text = currentEloScore.ToString();

            if (eloScoreDelta > 0)
            {
                resultsRatingChangeLabel.text = "(+" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.GREEN_DIM;
                resultsRatingChangeLabel.gameObject.SetActive(true);
            }
            else if (eloScoreDelta < 0)
            {
                resultsRatingChangeLabel.text = "(" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.RED_DIM;
                resultsRatingChangeLabel.gameObject.SetActive(true);
            }
        }

        private void UpdateGameEndReasonSection(GameEndReason gameEndReason)
        {
            viewBoardResultPanel.reason.text = "";
            offerTextDlg.SetActive(false);
            offerDrawDialog.SetActive(false);

            switch (gameEndReason)
            {
                case GameEndReason.TIMER_EXPIRED:
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_TIMER_EXPIRED);
                    break;

                case GameEndReason.CHECKMATE:
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_CHECKMATE);
                    break;

                case GameEndReason.RESIGNATION:
                    if (!playerWins)
                    {
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_PLAYER);
                        animDelay = RESULTS_SHORT_DELAY_TIME;
                        viewBoardResultPanel.reason.text = string.Format("{0} resigned", playerName);
                        //EnableRewarededVideoButton(preferencesModel.resignCount <= adsSettingsModel.resignCap);
                    }
                    else
                    {
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_OPPONENT);
                        viewBoardResultPanel.reason.text = string.Format("{0} resigned", opponentName);
                    }
                    break;

                case GameEndReason.STALEMATE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_STALEMATE);
                    break;

                case GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL);
                    break;

                case GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITH_MOVE: 
                case GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE);
                    break;

                case GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITH_MOVE:
                case GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE);
                    break;

                case GameEndReason.PLAYER_DISCONNECTED:
                    if (playerWins)
                    {
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_OPPONENT_LEFT);
                        viewBoardResultPanel.reason.text = string.Format("{0} left", opponentName);
                    }
                    else
                    {
                        //isDraw = true; when any player disconnects then the player who gets disconnected gets a draw and the opponent wins

                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED);
                        viewBoardResultPanel.reason.text = string.Format("{0} left", playerName);
                    }
                    break;

                case GameEndReason.DRAW_BY_DRAW_OFFERED:
                    isDraw = true;
                    resultsGameResultReasonLabel.color = Colors.YELLOW_DIM;
                    Color c = resultsGameResultReasonLabel.color;
                    c.a = Colors.ENABLED_TEXT_ALPHA;
                    resultsGameResultReasonLabel.color = c;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_OFFERED_DRAW);
                    break;

                default:
                    resultsGameResultReasonLabel.text = "Unknown Reason";
                    break;
            }

            if (string.IsNullOrEmpty(viewBoardResultPanel.reason.text))
            {
                viewBoardResultPanel.reason.text = resultsGameResultReasonLabel.text;
            }

            SetupRatingBoostButton(!isDraw);
        }

        private void UpdateGameResultHeadingSection()
        {
            if (isDraw)
            {
                resultsGameImage.sprite = drawSprite;
                resultsGameResultLabel.sprite = drawText;
                viewBoardResultPanel.result.text = "Drawn";
            }
            else if (playerWins)
            {
                resultsGameImage.sprite = winSprite;
                resultsGameResultLabel.sprite = winText;
                viewBoardResultPanel.result.text = string.Format("{0} won", playerName);
                SetupRewardsDoublerButton(true);
            }
            else
            {
                resultsGameImage.sprite = defeatSprite;
                resultsGameResultLabel.sprite = defeatText;
                viewBoardResultPanel.result.text = string.Format("{0} won", opponentName);
            }

            resultsGameImage.SetNativeSize();
            resultsGameResultLabel.SetNativeSize();
        }

        private void SetupResultsLayout()
        {
            resultsBoostRatingButton.gameObject.SetActive(!isDraw);
            resultsBetReversedLabel.gameObject.SetActive(isDraw && isRankedGame);
            resultsRewardLabel.gameObject.SetActive(playerWins && isRankedGame);
            resultsRewardsCoins.gameObject.SetActive(isDraw || playerWins && isRankedGame);
            resultsRewardsStars.gameObject.SetActive(playerWins && isRankedGame);
            resultsDoubleRewardButton.gameObject.SetActive(false/*playerWins && isRankedGame*/);
            resultsContinueButton.gameObject.SetActive(false/*playerWins && isRankedGame*/);
            resultsContinueButton2.gameObject.SetActive(!playerWins || isDraw || !isRankedGame);
            resultsRatingContainer.gameObject.SetActive(isRankedGame);
            resultsPowerplayImage.gameObject.SetActive(playerWins && isRankedGame);
        }

        private void UpdateRewards(long betValue, int stars, bool powerMode)
        {
            //resultsEarnedCoinsLabel.text = playerWins ? $"{betValue * 2}" : betValue.ToString();
            //resultsEarnedStarsLabel.text = powerMode ? $"{stars * 2}" : stars.ToString();
            resultsPowerplayImage.enabled = powerMode;
            resultsPowerplayImage.sprite = powerMode ? powerPlayOnSprite : powerPlayOffSprite;
        }

        private void BuildLayout()
        {
            foreach (var layout in resultsLayouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }
        }

        private void AnimateResultsDialog()
        {
            resultsDialog.transform.DOLocalMove(Vector3.zero, RESULTS_DIALOG_DURATION).SetEase(Ease.OutBack).OnComplete(OnAnimateResultsComplete);

            if (isDraw || !playerWins)
            {
                audioService.Play(audioService.sounds.SFX_DEFEAT);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_VICTORY);
            }
        }

        private void OnAnimateResultsComplete()
        {
            if (playerWins && !animationPlayed && isRankedGame)
            {
                _winAnimationSequence.PlayAnimation();
                animationPlayed = true;
            }
        }

        public bool IsResultsDialogVisible()
        {
            return resultsDialog.activeSelf;
        }

        private void HandleDeclinedDialog()
        {
            resultsDialog.SetActive(false);
            declinedDialog.SetActive(true);
            declinedDialog.transform.localPosition = new Vector3(0f, Screen.height + declinedDialogHalfHeight, 0f);
            Invoke("AnimateDeclinedDialog", RESULTS_SHORT_DELAY_TIME);
        }

        private void AnimateDeclinedDialog()
        {
            declinedDialog.transform.DOLocalMove(Vector3.zero, RESULTS_DIALOG_DURATION).SetEase(Ease.OutBack);
            audioService.Play(audioService.sounds.SFX_DEFEAT);
        }

        private bool IsResultsDialogActive()
        {
            return resultsDialog.activeSelf;
        }

        private void OnResultsBoostRatingButtonClicked()
        {
            audioService.PlayStandardClick();

            if (!isRankedGame)
            {
                resultsBoostRatingToolTip.SetActive(true);
                resultsBoostRatingToolTipText.text = localizationService.Get(LocalizationKey.RESULTS_BOOST_FRIENDLY);
            }
            else if (isDraw)
            {
                resultsBoostRatingToolTip.SetActive(true);
                resultsBoostRatingToolTipText.text = localizationService.Get(LocalizationKey.RESULTS_BOOST_DRAW);
            }
            else
            {
                if (haveEnoughGemsForRatingBooster)
                {
                    boostRatingSignal.Dispatch(challengeId);
                    SetupRatingBoostButton(false);
                    resultsBoostRatingButton.interactable = false;
                }
                else
                {
                    SpotPurchaseMediator.analyticsContext = "rating_booster";
                    notEnoughGemsSignal.Dispatch();
                }
            }
        }

        private void OnRewardDoublerClicked()
        {
            audioService.PlayStandardClick();
            if (haveEnoughGemsForRewardDoubler)
            {
                var transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
                transactionVO.consumeQuantity = rewardDoublerStoreItem.currency3Cost;
                transactionVO.buyItemShortCode = GSBackendKeys.PlayerDetails.COINS;
                transactionVO.buyQuantity = (int)resultsBetValue * 2;
                doubleRewardSignal.Dispatch(transactionVO);
                SetupRewardsDoublerButton(false);
            }
            else
            {
                SpotPurchaseMediator.analyticsContext = "coin_doubler";
                notEnoughGemsSignal.Dispatch();
            }
        }

        private void OnResultsDeclinedButtonClicked()
        {
            audioService.PlayStandardClick();
            if (isLongPlay)
            {
                backToLobbySignal.Dispatch();
            }
            else
            {
                backToLobbySignal.Dispatch();
            }
        }

        private void OnResultsClosed()
        {
            audioService.PlayStandardClick();
            uiBlocker.SetActive(false);
            resultsDialogClosedSignal.Dispatch();
            ShowViewBoardResultsPanel(true);
        }

        public void OnResultsSkipRewardButtonClicked()
        {
            audioService.PlayStandardClick();
            animationPlayed = false;

            ShowInterstitialOnBack(AnalyticsContext.interstitial_endgame, AdPlacements.Interstitial_endgame);
        }


        private void OnCrossPromoButtonClicked()
        {
            //toggleBannerSignal.Dispatch(false);
            hAnalyticsService.LogEvent(AnalyticsEventId.cross_promo_clicked.ToString());
            HCrossPromo.OnCrossPromoPanelClosed += ToggleBannerSignalFunc;
            HCrossPromo.OpenPanel();
            appInfoModel.internalAdType = InternalAdType.INTERAL_AD;
        }

        private void ToggleBannerSignalFunc()
        {
            appInfoModel.internalAdType = InternalAdType.NONE;
            HCrossPromo.OnCrossPromoPanelClosed -= ToggleBannerSignalFunc;
            //toggleBannerSignal.Dispatch(true);
        }

        private void SetupRatingBoostButton(bool enable)
        {
            var color = enable ? Colors.WHITE : Colors.WHITE_100;
            resultsBoostRatingToolTip.gameObject.SetActive(false);
            resultsBoostRatingButton.interactable = true;
            resultsBoostRatingButton.image.color = color;
            resultsBoostRatingGemIcon.color = color;
            resultsBoostRatingGemsCost.color = color;
            resultsBoostRatingIcon.color = color;
            resultsBoostRatingText.color = color;
            resultsBoostSheen.SetActive(enable);
            resultsBoostGlow.SetActive(enable);
        }

        private void SetupRewardsDoublerButton(bool enable)
        {
            var color1 = enable ? Colors.WHITE : Colors.WHITE_100;
            var color2 = enable ? Colors.BLACK : Colors.BLACK_DIM;
            resultsDoubleRewardButton.interactable = enable;
            resultsDoubleRewardGemIcon.color = color1;
            resultsDoubleRewardGemsCost.color = color2;
            resultsDoubleRewardText.color = color2;
        }

        public void OnRatingBoosted(int boostedRating)
        {
            PlayEloBoostedAnimation(boostedRating);
        }

        public void OnRewardDoubled()
        {
            resultsEarnedCoinsLabel.text = $"{resultsBetValue * 4}";
        }

        private void PlayEloBoostedAnimation(int ratingBoosted)
        {
            if (addedAnimation != null)
            {
                addedAnimation.Kill();
                DOTween.Kill(resultsBoostRatingAddedCount.transform);
            }

            resultsBoostRatingAddedCount.text = $"+{ratingBoosted}";
            resultsBoostRatingAddedCount.transform.localPosition = Vector3.zero;
            resultsBoostRatingAddedCount.color = originalColor;
            resultsBoostRatingAddedCount.gameObject.SetActive(true);
            addedAnimation = DOTween.ToAlpha(() => resultsBoostRatingAddedCount.color, x => resultsBoostRatingAddedCount.color = x, 0.0f, 3.0f).OnComplete(OnFadeComplete);
            resultsBoostRatingAddedCount.transform.DOMoveY(resultsBoostRatingAddedCount.transform.position.y + 100, 3.0f);
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
        }

        private void OnFadeComplete()
        {
            resultsBoostRatingAddedCount.gameObject.SetActive(false);
        }

        public void SetupBoostPrice()
        {
            if (ratingBoosterStoreItem == null)
            {
                return;
            }

            haveEnoughGemsForRatingBooster = playerModel.gems >= ratingBoosterStoreItem.currency3Cost;
            resultsBoostRatingGemsCost.text = ratingBoosterStoreItem.currency3Cost.ToString();
        }

        public void SetupRewardDoublerPrice()
        {
            if (rewardDoublerStoreItem == null)
            {
                return;
            }

            haveEnoughGemsForRewardDoubler = playerModel.gems >= rewardDoublerStoreItem.currency3Cost;
            resultsDoubleRewardGemsCost.text = rewardDoublerStoreItem.currency3Cost.ToString();
        }

        private void ShowInterstitialOnBack(AnalyticsContext analyticsContext, AdPlacements placementId)
        {
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.Interstitial;
            vo.rewardType = GSBackendKeys.ClaimReward.NONE;
            vo.challengeId = challengeId;
            vo.playerWins = playerWins;
            vo.placementId = placementId;
            playerModel.adContext = analyticsContext;

            showAdSignal.Dispatch(vo, false);
        }
    }
}
