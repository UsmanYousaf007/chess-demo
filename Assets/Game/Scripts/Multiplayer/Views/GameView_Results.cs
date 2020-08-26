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
        public Text resultsGameResultLabel;
        public Text resultsGameResultReasonLabel;
        public Text resultsFriendlyLabel;

        public Text resultsRatingValueLabel;
        public Text resultsRatingChangeLabel;

        public Button resultsBoostRatingButton;
        public Text resultsBoostRatingButtonLabel;
        public Image resultsBoostRatingAdTVImage;
        public Text resultsBoostRatingAddedCount;
        public GameObject resultsBoostRatingToolTip;
        public Text resultsBoostRatingToolTipText;
        public Text resultsBoostRatingGemsCost;
        public Image resultsBoostRatingGemsBg;
        public Sprite enoughGemsSprite;
        public Sprite notEnoughGemsSprite;
        public string resultsBoostRatingShortCode;

        public Button resultsCollectRewardButton;
        public Text resultsCollectRewardButtonLabel;
        public Image resultsAdTVImage;

        public Button resultsViewBoardButton;
        public Text resultsViewBoardButtonLabel;  

        public Button resultsSkipRewardButton;
        public Text resultsSkipRewardButtonLabel;

        public Button showCrossPromoButton;

        public RectTransform rewardBar;
        public Text earnRewardsText;
        public GameObject earnRewardsSection;
        public Image dailogueBg;

        public ViewBoardResults viewBoardResultPanel;

        public Signal resultsStatsButtonClickedSignal = new Signal();
        public Signal showAdButtonClickedSignal = new Signal();
        public Signal resultsDialogClosedSignal = new Signal();
        public Signal resultsDialogOpenedSignal = new Signal();
        public Signal backToLobbySignal = new Signal();
        public Signal<string, VirtualGoodsTransactionVO> boostRatingSignal = new Signal<string, VirtualGoodsTransactionVO>();
        public Signal refreshLobbySignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();

        private const float RESULTS_DELAY_TIME = 1f;
        private const float RESULTS_SHORT_DELAY_TIME = 0.3f;
        private const float RESULTS_DIALOG_DURATION = 0.5f;
        private float resultsDialogHalfHeight;
        private float declinedDialogHalfHeight;
        private float rewardBarOriginalWidth;
        private Tweener addedAnimation;
        private bool playerWins;
        private bool isDraw;
        private string adRewardType;
        private string collectRewardType;
        private float animDelay;
        private string playerName;
        private string opponentName;
        private string challengeId;
        private bool isResultsBoostRatingButtonEnabled = false;
        private Color originalColor;
        private StoreItem ratingBoosterStoreItem;
        private bool haveEnoughGemsForRatingBooster;
        private bool haveEnoughRatingBoosters;

        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        public void InitResults()
        {
            // Button listeners
            resultsBoostRatingButton.onClick.AddListener(OnResultsBoostRatingButtonClicked);
            resultsCollectRewardButton.onClick.AddListener(OnResultsCollectRewardButtonClicked);
            declinedLobbyButton.onClick.AddListener(OnResultsDeclinedButtonClicked);
            resultsViewBoardButton.onClick.AddListener(OnResultsClosed);
            resultsSkipRewardButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);
            showCrossPromoButton.onClick.AddListener(OnCrossPromoButtonClicked);

            // Text Labels
            resultsCollectRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON);
            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsSkipRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_SKIP_REWARD_BUTTON);
            resultsViewBoardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);

            declinedHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DECLINED);
            declinedReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DECLINED);
            declinedLobbyButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_RESULTS_BACK);
            earnRewardsText.text = localizationService.Get(LocalizationKey.RESULTS_EARNED);

            resultsDialogHalfHeight = resultsDialog.GetComponent<RectTransform>().rect.height / 2f;
            declinedDialogHalfHeight = declinedDialog.GetComponent<RectTransform>().rect.height / 2f;
            rewardBarOriginalWidth = rewardBar.sizeDelta.x;
            originalColor = resultsBoostRatingAddedCount.color;
        }

        public void CleanupResults()
        {
            resultsCollectRewardButton.onClick.RemoveAllListeners();
            declinedLobbyButton.onClick.RemoveAllListeners();
            resultsViewBoardButton.onClick.RemoveAllListeners();
            resultsSkipRewardButton.onClick.RemoveAllListeners();
        }

        private void EnableRewarededVideoButton(bool enable)
        {
            DoPulse(enable);
            isResultsBoostRatingButtonEnabled = enable;
            if (enable)
            {
                resultsCollectRewardButton.interactable = true;
                resultsCollectRewardButtonLabel.color = Colors.ColorAlpha(Colors.WHITE, Colors.ENABLED_TEXT_ALPHA);
                Color c = resultsAdTVImage.color;
                c.a = Colors.FULL_ALPHA;
                resultsAdTVImage.color = c;

                resultsBoostRatingButton.interactable = true;
                resultsBoostRatingButtonLabel.color = Colors.ColorAlpha(Colors.BLACK, Colors.ENABLED_TEXT_ALPHA);
                c = resultsBoostRatingAdTVImage.color;
                c.a = Colors.FULL_ALPHA;
                resultsBoostRatingAdTVImage.color = c;
                resultsBoostRatingButton.GetComponent<Image>().color = c;
            }
            else
            {
                resultsCollectRewardButton.interactable = false;
                resultsCollectRewardButtonLabel.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                Color c = resultsAdTVImage.color;
                c.a = Colors.DISABLED_TEXT_ALPHA;
                resultsAdTVImage.color = c;

                resultsBoostRatingButton.interactable = false;
                resultsBoostRatingButtonLabel.color = Colors.ColorAlpha(Colors.BLACK_DIM, Colors.DISABLED_TEXT_ALPHA);
                c = resultsBoostRatingAdTVImage.color;
                c.a = Colors.DISABLED_TEXT_ALPHA;
                resultsBoostRatingAdTVImage.color = c;
                resultsBoostRatingButton.GetComponent<Image>().color = c;
            }
        }

        public void DoPulse(bool val)
        {
            if (val)
            {
                iTween.PunchScale(resultsBoostRatingButton.gameObject, iTween.Hash("amount", new Vector3(0.15f, 0.15f, 0f), "time", 1f, "loopType", "loop", "delay", 1));
            }
            else
            {
                iTween.Stop(resultsBoostRatingButton.gameObject);
            }
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
            showCrossPromoButton.gameObject.SetActive(HCrossPromo.service.hasContent);
            appInfoModel.gameMode = GameMode.NONE;
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
                //EnableRewarededVideoButton(false);
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
            //EnableRewarededVideoButton(true);
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

        private void UpdateGameResultHeadingSection(int eloScoreDelta)
        {
            if (isDraw)
            {
                resultsGameImage.sprite = drawSprite;
                resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DRAW);
                resultsGameResultLabel.color = Colors.YELLOW_DIM;
                viewBoardResultPanel.result.text = "Drawn";
                resultsBoostRatingButtonLabel.text = $"{localizationService.Get(LocalizationKey.RESULTS_BOOST_RATING_BUTTON)} +{rewardsSettingsModel.ratingBoostReward}";
            }
            else
            {
                if (playerWins)
                {
                    resultsGameImage.sprite = winSprite;
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN);
                    resultsGameResultLabel.color = Colors.GREEN_DIM;
                    viewBoardResultPanel.result.text = string.Format("{0} won", playerName);
                    resultsBoostRatingButtonLabel.text = $"{localizationService.Get(LocalizationKey.RESULTS_BOOST_RATING_BUTTON)} +{rewardsSettingsModel.ratingBoostReward}";
                }
                else
                {
                    int ratingBoost = rewardsSettingsModel.ratingBoostReward;
                    if (isResultsBoostRatingButtonEnabled)
                    {
                        if (eloScoreDelta < 0 && rewardsSettingsModel.ratingBoostReward > Mathf.Abs(eloScoreDelta))
                        {
                            ratingBoost = Mathf.Abs(eloScoreDelta);
                        }
                    }
                    resultsGameImage.sprite = defeatSprite;
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE);
                    resultsGameResultLabel.color = Colors.RED_DIM;
                    viewBoardResultPanel.result.text = string.Format("{0} won", opponentName);
                    resultsBoostRatingButtonLabel.text = $"{localizationService.Get(LocalizationKey.RESULTS_RECOVER_RATING_BUTTON)} +{ratingBoost}";
                }
            }
            resultsGameImage.SetNativeSize();
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
            
            UpdateGameEndReasonSection(vo.reason);
            UpdateResultRatingSection(vo.isRanked, vo.currentEloScore, vo.eloScoreDelta);
            //EnableRewarededVideoButton(adsService.IsRewardedVideoAvailable());
            UpdateGameResultHeadingSection(vo.eloScoreDelta);

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

            //resultsAdTVImage.gameObject.SetActive(!vo.removeAds);
            //resultsCollectRewardButton.gameObject.SetActive(!vo.removeAds);
            //resultsCollectRewardButtonLabel.gameObject.SetActive(!vo.removeAds);

            if (vo.removeAds)
            {
                resultsDialog.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1050.0f);
            }

            int rewardCoins = rewardsSettingsModel.getRewardCoins(AdType.Interstitial, vo.powerupUsedCount, playerWins);

            // Reward
            adRewardType = vo.playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN_AD : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN_AD;
            collectRewardType = vo.playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN;
            challengeId = vo.challengeId;

            dailogueBg.enabled = false;
            earnRewardsSection.SetActive(!playerModel.HasSubscription());
            dailogueBg.enabled = true;

            var barFillPercentage = playerModel.rewardCurrentPoints / playerModel.rewardPointsRequired;
            rewardBar.sizeDelta = new Vector2(rewardBarOriginalWidth * barFillPercentage, rewardBar.sizeDelta.y);

            ratingBoosterStoreItem = vo.ratingBoostStoreItem;
            SetupBoostPrice();

            if (isRankedGame && !isDraw)
            {
                analyticsService.Event(AnalyticsEventId.booster_shown, AnalyticsContext.rating_booster);
            }
        }

        private void AnimateResultsDialog()
        {
            resultsDialog.transform.DOLocalMove(Vector3.zero, RESULTS_DIALOG_DURATION).SetEase(Ease.OutBack);

            if (isDraw || !playerWins)
            {
                audioService.Play(audioService.sounds.SFX_DEFEAT);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_VICTORY);
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
			
        private void OnResultsCollectRewardButtonClicked()
        {
            audioService.PlayStandardClick();
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.RewardedVideo;
            vo.rewardType = adRewardType;
            vo.challengeId = challengeId;
            vo.playerWins = playerWins;
            playerModel.adContext = AnalyticsContext.rewarded;
            analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
            showRewardedAdSignal.Dispatch(vo);
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
                var transactionVO = new VirtualGoodsTransactionVO();

                if (haveEnoughRatingBoosters)
                {
                    transactionVO.consumeItemShortCode = resultsBoostRatingShortCode;
                    transactionVO.consumeQuantity = 1;
                    BoostRating(transactionVO);
                }
                else if (haveEnoughGemsForRatingBooster)
                {
                    transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
                    transactionVO.consumeQuantity = ratingBoosterStoreItem.currency3Cost;
                    BoostRating(transactionVO);
                }
                else
                {
                    notEnoughGemsSignal.Dispatch();
                }
            }
        }

        private void BoostRating(VirtualGoodsTransactionVO transactionVO)
        {
            boostRatingSignal.Dispatch(challengeId, transactionVO);
            resultsBoostRatingButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_BOOSTED);
            SetupRatingBoostButton(false);
            resultsBoostRatingButton.interactable = false;
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

            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.Interstitial;
            vo.rewardType = GSBackendKeys.ClaimReward.NONE;
            vo.challengeId = challengeId;
            vo.playerWins = playerWins;
            playerModel.adContext = AnalyticsContext.interstitial_endgame;

            if (!playerModel.HasSubscription())
            {
                analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
            }

            showAdSignal.Dispatch(vo);
        }


        private void OnCrossPromoButtonClicked()
        {
            toggleBannerSignal.Dispatch(false);
            hAnalyticsService.LogEvent(AnalyticsEventId.cross_promo_clicked.ToString());
          
            IPromise promise = HCrossPromo.OpenPanel();
            if (promise != null)
            {
                appInfoModel.internalAdType = InternalAdType.INTERAL_AD;
                promise.Then(ToggleBannerSignalFunc);
            }
        }

        private void ToggleBannerSignalFunc()
        {
            appInfoModel.internalAdType = InternalAdType.NONE;
            toggleBannerSignal.Dispatch(true);
        }

        private void SetupRatingBoostButton(bool enable)
        {
            var c = resultsBoostRatingAdTVImage.color;
            resultsBoostRatingToolTip.gameObject.SetActive(false);
            resultsBoostRatingButton.interactable = true;

            if (enable)
            {
                resultsBoostRatingButtonLabel.color = Colors.ColorAlpha(Colors.BLACK, Colors.ENABLED_TEXT_ALPHA);
                c.a = Colors.FULL_ALPHA;
                resultsBoostRatingAdTVImage.color = c;
                resultsBoostRatingButton.GetComponent<Image>().color = c;
            }
            else
            {
                resultsBoostRatingButtonLabel.color = Colors.ColorAlpha(Colors.BLACK_DIM, Colors.DISABLED_TEXT_ALPHA);
                c = resultsBoostRatingAdTVImage.color;
                c.a = Colors.DISABLED_TEXT_ALPHA;
                resultsBoostRatingAdTVImage.color = c;
                resultsBoostRatingButton.GetComponent<Image>().color = c;
            }
        }

        public void PlayEloBoostedAnimation(int ratingBoosted)
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

            resultsBoostRatingGemsCost.text = ratingBoosterStoreItem.currency3Cost.ToString();
            haveEnoughRatingBoosters = playerModel.GetInventoryItemCount(resultsBoostRatingShortCode) > 0;
            haveEnoughGemsForRatingBooster = playerModel.gems >= ratingBoosterStoreItem.currency3Cost;
            resultsBoostRatingGemsBg.sprite = haveEnoughGemsForRatingBooster ? enoughGemsSprite : notEnoughGemsSprite;
            resultsBoostRatingGemsBg.gameObject.SetActive(!haveEnoughRatingBoosters);
        }
    }
}
