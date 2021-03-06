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
using System.Collections.Generic;
using System;
using System.Collections;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Game Analyzing Dialog")]
        public Image[] loadingBars;
        public GameObject analyzingDlg;
        public CanvasGroup analyzingDlgCanvasGroup;
        float averageHeightOfAnalyzingBar;
        bool animateBarsEnabled = false;
        public RectTransform analyzingProgress;

        [Header("End Game Declined Dialog")]
        public GameObject declinedDialog;
        public Text declinedHeading;
        public Text declinedReason;
        public Button declinedLobbyButton;
        public Text declinedLobbyButtonLabel;
        private bool hasDeclined;

        [Header("End Game Results Dialog")]
        public GameObject resultsDialog;
        public CanvasGroup resultsCanvasGroup;
        public GameObject gameEndDlgContainer;

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
        public ToolTip resultsBoostRatingButtonAnim;

        public Button resultsViewBoardButton;
        public Text resultsViewBoardButtonLabel;
        public ViewBoardResults viewBoardResultPanel;

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

        public Button fullAnalysisBtn;
        public TMP_Text fullAnalysisGemsCount;
        public TMP_Text bunders;
        public TMP_Text mistakes;
        public TMP_Text perfect;
        public GameObject resultsFullAnalysisFreeTag;
        public GameObject resultsFullAnalysisFreeTitle;
        public GameObject resultsFullAnalysisSparkle;
        public GameObject resultsFullAnalysisGemIcon;
        public GameObject resultsFullAnalysisProcessing;
        public GameObject resultsFullAnalysisdPanel;
        public GameObject resultsFullAnalysisDisabledPanel;
        public GameObject resultsRVAnalysisPanel;
        public Button resultsRVAnalysisButton;
        public Button resultsRVFullAnalysisButton;
        public TMP_Text resultsRVAnalysisGemsCost;
        public TMP_Text resultsRVAnalysisTimer;
        public GameObject resultsRVAnalysisWatchVideo;
        public GameObject resultsRVAnalysisTimerRunning;
        public GameObject resultsRVAnalysisNotAvailableTooltip;
        public GameObject resultsRVAnalysisTimerTooltip;

        public Image resultsPowerplayImage;
        public Sprite powerPlayOnSprite;
        public Sprite powerPlayOffSprite;

        public Image sparkles;

        public RectTransform[] resultsLayouts;

        public GameObject ratingBoosterRvPanel;
        public Button rewardedVideoBtn;
        public Image rewardedVideoBtnImg;
        public GameObject ratingBoosterTimer;
        public TMP_Text remainingCoolDownTime;
        public GameObject getRV;
        public GameObject timerRunningTooltip;
        public GameObject videoNotAvailableTooltip;
        public ToolTip rewardedVideoButtonAnimWithRv;

        public Button resultsBoostRatingButtonWithRv;
        public Image resultsBoostRatingButtonWithRvImg;
        public GameObject resultsBoostRatingToolTipWithRv;
        public Text resultsBoostRatingToolTipTextWithRv;
        public Text resultsBoostRatingGemsCostWithRv;
        public TMP_Text resultsBoostRatingTextWithRv;
        public Image resultsBoostRatingGemIconWithRv;
        public GameObject resultsBoostSheenWithRv;
        public ToolTip resultsBoostRatingButtonAnimWithRv;

        public IServerClock serverClock;

        public Signal resultsDialogClosedSignal = new Signal();
        public Signal resultsDialogOpenedSignal = new Signal();
        public Signal backToLobbySignal = new Signal();
        public Signal<string> boostRatingSignal = new Signal<string>();
        public Signal refreshLobbySignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();
        public Signal backToArenaSignal = new Signal();
        public Signal<VirtualGoodsTransactionVO> doubleRewardSignal = new Signal<VirtualGoodsTransactionVO>();
        public Signal fullAnalysisButtonClickedSignal = new Signal();
        public Signal ratingBoosterRewardSignal = new Signal();
        public Signal<bool> schedulerSubscription = new Signal<bool>();
        public Signal showGameAnalysisSignal = new Signal();
        public Signal rvAnalysisWatchVideoSignal = new Signal();

        private float declinedDialogHalfHeight;
        private Tweener addedAnimation;
        private bool playerWins;
        private bool isDraw;
        private string playerName;
        private string opponentName;
        private string challengeId;
        private Color originalColor;
        private StoreItem ratingBoosterStoreItem;
        private bool haveEnoughGemsForRatingBooster;
        private StoreItem rewardDoublerStoreItem;
        private bool haveEnoughGemsForRewardDoubler;
        private long resultsBetValue;

        private List<MoveAnalysis> moveAnalysisList;
        private StoreItem fullGameAnalysisStoreItem;
        private bool haveEnoughGemsForFullAnalysis;
        private bool freeGameAnalysisAvailable;
        private MatchAnalysis matchAnalysis;
        private int movesCount;

        private bool isRatingBoosterRVEnabled;
        private bool isAnalysisRVEnabled;
        private long coolDownTimeUTC;

        Sequence animSequence;

        public void InitResults()
        {
            // Declined dialog
            declinedLobbyButton.onClick.AddListener(OnResultsDeclinedButtonClicked);
            declinedHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DECLINED);
            declinedReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DECLINED);
            declinedLobbyButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_RESULTS_BACK);
            declinedDialogHalfHeight = declinedDialog.GetComponent<RectTransform>().rect.height / 2f;

            resultsBoostRatingButton.onClick.AddListener(OnResultsBoostRatingButtonClicked);
            resultsBoostRatingButtonWithRv.onClick.AddListener(OnResultsBoostRatingButtonClicked);
            resultsViewBoardButton.onClick.AddListener(OnResultsClosed);
            resultsContinueButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);
            resultsDoubleRewardButton.onClick.AddListener(OnRewardDoublerClicked);
            fullAnalysisBtn.onClick.AddListener(OnFullAnalysisButtonClicked);
            rewardedVideoBtn.onClick.AddListener(OnPlayRewardedVideoClicked);
            resultsRVFullAnalysisButton.onClick.AddListener(OnFullAnalysisButtonClicked);
            resultsRVAnalysisButton.onClick.AddListener(OnRVAnalytisWatchVideoButtonClicked);

            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsViewBoardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);

            originalColor = resultsBoostRatingAddedCount.color;
            averageHeightOfAnalyzingBar = loadingBars[0].rectTransform.sizeDelta.y;

            UIDlgManager.Setup(gameEndDlgContainer);
            UIDlgManager.Setup(analyzingDlg);

            animSequence = DOTween.Sequence();
            gameEndDlgContainer.SetActive(false);
        }

        public void CleanupResults()
        {
            declinedLobbyButton.onClick.RemoveAllListeners();
            resultsViewBoardButton.onClick.RemoveAllListeners();
            resultsContinueButton.onClick.RemoveAllListeners();
            resultsDoubleRewardButton.onClick.RemoveAllListeners();
        }

        public void OnParentShowResults()
        {
            //HideResultsDialog();
            HideGameEndDialog();
            resultsFullAnalysisProcessing.SetActive(false);
            //analyzingDlg.gameObject.SetActive(false);
            matchAnalysis = new MatchAnalysis();
        }

        public void ShowResultsDialog()
        {
            if(hasDeclined)
            {
                HandleDeclinedDialog();
                return;
            }

            EnableModalBlocker();
            Invoke("AnimateResultsDialog", animDelay);
        }

        private void AnimateResultsDialog()
        {
            // Hide reward glass
            rewardsDialog.SetActive(false);

            // Setup layout
            resultsDialog.SetActive(true);
            BuildLayout();
            resultsDialog.SetActive(false);

            // Animate the results dialog
            UIDlgManager.EnableBlurBlg(gameEndDlgContainer);
            UIDlgManager.AnimateDlg(resultsDialog);
            AnimateSparkes();

            // Play sound effect
            if (isDraw || !playerWins)
            {
                audioService.Play(audioService.sounds.SFX_DEFEAT);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_VICTORY);
            }

            resultsBoostRatingButtonAnim.enabled = true;
            resultsBoostRatingButtonAnimWithRv.enabled = true;
            rewardedVideoButtonAnimWithRv.enabled = true;
        }

        public void UpdateResultsDialog(ResultsVO vo)
        {
            DisableInteraction();
            DisableFreeHint();

            if (vo.reason == GameEndReason.DECLINED)
            {
                hasDeclined = true;
                return;
            }

            hasDeclined = false;
            playerWins = vo.playerWins;
            playerName = vo.playerName;
            opponentName = vo.opponentName;
            isDraw = false;
            animDelay = RESULTS_DELAY_TIME;
            GameEndReason gameEndReason = vo.reason;
            ratingBoosterStoreItem = vo.ratingBoostStoreItem;
            rewardDoublerStoreItem = vo.rewardDoubleStoreItem;
            fullGameAnalysisStoreItem = vo.fullGameAnalysisStoreItem;
            resultsBetValue = vo.betValue;
            challengeId = vo.challengeId;
            movesCount = vo.movesCount;
            freeGameAnalysisAvailable = vo.freeGameAnalysisAvailable;
            isRatingBoosterRVEnabled = vo.isRatingBoosterRVEnabled;
            isAnalysisRVEnabled = vo.isAnalysisRVEnabled;
            coolDownTimeUTC = vo.coolDownTimeUTC;

            UpdateGameEndReasonSection(vo.reason);
            UpdateGameResultHeadingSection();
            UpdateResultRatingSection(vo.isRanked, vo.currentEloScore, vo.eloScoreDelta);
            SetupResultsLayout();
            SetupBoostPrice();
            SetupRewardDoublerPrice();
            UpdateMatchAnalysis();
            SetGameAnalysisPanel();

            if (!isLongPlay)
            {
                SetupFullAnalysisTab(vo.freeGameAnalysisAvailable);
                SetupFullAnalysisPrice(vo.freeGameAnalysisAvailable);
                SetupAnalysisRV(vo.isAnalysisRVEnabled);
            }

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

            resultsDialog.transform.localScale = new Vector3(0f, 0, 0f);
        }

        public void SetGameAnalysisPanel()
        {
            resultsFullAnalysisdPanel.gameObject.SetActive(!isLongPlay && movesCount > 0);
            resultsFullAnalysisDisabledPanel.gameObject.SetActive(isLongPlay || movesCount == 0);
        }

        public void HideResultsDialog()
        {
            resultsDialog.SetActive(false);
            declinedDialog.SetActive(false);

            animSequence.Kill();
        }

        private void UpdateResultRatingSection(bool isRanked, int currentEloScore, int eloScoreDelta)
        {
            resultsFriendlyLabel.gameObject.SetActive(false);
            resultsRatingValueLabel.gameObject.SetActive(false);
            resultsRatingChangeLabel.gameObject.SetActive(false);

            if (!isRanked)
            {
                resultsFriendlyLabel.gameObject.SetActive(true);
                SetupRatingBoostButtonsSection(false);
                return;
            }

            // Ranked Game
            resultsRatingValueLabel.gameObject.SetActive(true);
            resultsRatingChangeLabel.gameObject.SetActive(false);
            resultsRatingValueLabel.text = currentEloScore.ToString();

            if (eloScoreDelta > 0)
            {
                resultsRatingChangeLabel.text = "(+" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.GREEN_LIGHT;
                resultsRatingChangeLabel.gameObject.SetActive(true);
            }
            else if (eloScoreDelta < 0)
            {
                resultsRatingChangeLabel.text = "(" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.RED_LIGHT;
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
                        //animDelay = RESULTS_SHORT_DELAY_TIME;
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
                    resultsGameResultReasonLabel.color = Colors.BLUE;
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

            SetupRatingBoostButtonsSection(!isDraw);
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
            bool enableRewardedVideoPanel = isRatingBoosterRVEnabled && isRankedGame;
            bool enableRatingBoosterPanel = !isRatingBoosterRVEnabled || !isRankedGame;

            ratingBoosterRvPanel.SetActive(enableRewardedVideoPanel && !isDraw);
            resultsBoostRatingButton.gameObject.SetActive(!enableRewardedVideoPanel && !isDraw);

            if (enableRewardedVideoPanel)
            {
                ratingBoosterRvPanel.SetActive(!isDraw);
                if (IsCoolDownComplete())
                {
                    getRV.SetActive(true);
                    rewardedVideoBtn.interactable = isRankedGame;
                    ratingBoosterTimer.SetActive(false);
                }
                else
                {
                    StartTimer(coolDownTimeUTC);
                }
            }

            else if (!enableRewardedVideoPanel)
            {

                resultsBetReversedLabel.gameObject.SetActive(isDraw && isRankedGame);
                resultsRewardLabel.gameObject.SetActive(playerWins && isRankedGame);
                resultsRewardsCoins.gameObject.SetActive(isDraw || playerWins && isRankedGame);
                resultsRewardsStars.gameObject.SetActive(playerWins && isRankedGame);
                resultsDoubleRewardButton.gameObject.SetActive(false/*playerWins && isRankedGame*/);

                //resultsContinueButton.gameObject.SetActive(playerWins || isDraw || !isRankedGame);
                resultsContinueButton.gameObject.SetActive(true);
                resultsRatingContainer.gameObject.SetActive(isRankedGame);
                resultsPowerplayImage.gameObject.SetActive(playerWins && isRankedGame);
            }

        }

        private void SetupAnalysisRV(bool showRV)
        {
            resultsRVAnalysisPanel.SetActive(showRV);
            fullAnalysisBtn.gameObject.SetActive(!showRV);
            resultsFullAnalysisFreeTitle.SetActive(showRV);
            resultsFullAnalysisSparkle.SetActive(!showRV);
            resultsFullAnalysisGemIcon.SetActive(!showRV);
            fullAnalysisGemsCount.enabled = !showRV;
            resultsRVFullAnalysisButton.interactable = true;

            if (showRV)
            {
                if (IsCoolDownComplete())
                {
                    OnTimerCompleted();
                }
                else
                {
                    StartTimer(coolDownTimeUTC);
                }
            }
        }

        void BuildLayout()
        {
            //yield return new WaitForSeconds(0.4f);
            foreach (var layout in resultsLayouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }
        }

        public bool IsResultsDialogVisible()
        {
            return resultsDialog.activeSelf;
        }

        private void HandleDeclinedDialog()
        {
            resultsDialog.SetActive(false);
            UIDlgManager.EnableBlurBlg(gameEndDlgContainer);
            UIDlgManager.AnimateDlg(declinedDialog);
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

                resultsBoostRatingToolTipWithRv.SetActive(true);
                resultsBoostRatingToolTipTextWithRv.text = localizationService.Get(LocalizationKey.RESULTS_BOOST_FRIENDLY);
            }
            else if (isDraw)
            {
                resultsBoostRatingToolTip.SetActive(true);
                resultsBoostRatingToolTipText.text = localizationService.Get(LocalizationKey.RESULTS_BOOST_DRAW);

                resultsBoostRatingToolTipWithRv.SetActive(true);
                resultsBoostRatingToolTipTextWithRv.text = localizationService.Get(LocalizationKey.RESULTS_BOOST_DRAW);
            }
            else
            {
                if (haveEnoughGemsForRatingBooster)
                {
                    boostRatingSignal.Dispatch(challengeId);
                    BoostRating();
                }
                else
                {
                    SpotPurchaseMediator.analyticsContext = "rating_booster";
                    notEnoughGemsSignal.Dispatch();
                }
            }
        }

        public void BoostRating()
        {

            SetupRatingBoostButtonsSection(false);
            resultsBoostRatingButton.interactable = false;

            resultsBoostRatingButtonWithRv.interactable = false;
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
            backToLobbySignal.Dispatch();
        }

        private void OnFullAnalysisButtonClicked()
        {
            audioService.PlayStandardClick();
            if (haveEnoughGemsForFullAnalysis)
            {
                fullAnalysisButtonClickedSignal.Dispatch();
                fullAnalysisBtn.interactable = false;
                resultsRVFullAnalysisButton.interactable = false;
            }
            else
            {
                SpotPurchaseMediator.analyticsContext = "game_analysis";
                notEnoughGemsSignal.Dispatch();
            }
        }

        public void ShowAnalyzingGame()
        {
            UIDlgManager.Show(analyzingDlg, Colors.BLUR_BG_BRIGHTNESS_DARK, true);
            animateBarsEnabled = true;
            StartCoroutine(AnimateBars());
            UpdateAnalysisView(true);
        }

        public void HideGameAnalyzingDlg()
        {
            animateBarsEnabled = false;
            StopCoroutine(AnimateBars());
            UIDlgManager.Hide(analyzingDlg);
            UpdateAnalysisView();
        }

        private void OnResultsClosed()
        {
            audioService.PlayStandardClick();
            uiBlocker.SetActive(false);
            HideGameEndDialog();
            resultsDialogClosedSignal.Dispatch();

            if (isLongPlay || movesCount == 0)
            {
                ShowViewBoardResultsPanel(true);
            }
            else
            {
                UpdateAnalysisView(true);
            }
        }

        public void OnResultsSkipRewardButtonClicked()
        {
            audioService.PlayStandardClick();
            ShowWeeklyChampionshipResults();
        }

        public void SetupRatingBoostButtonsSection(bool enable)
        {
            var color = enable ? Colors.WHITE : Colors.WHITE_100;
            resultsBoostRatingToolTip.gameObject.SetActive(false);
            resultsBoostRatingButton.interactable = enable;
            resultsBoostRatingButton.image.color = color;
            resultsBoostRatingGemIcon.color = color;
            resultsBoostRatingGemsCost.color = color;
            resultsBoostRatingIcon.color = color;
            resultsBoostRatingText.color = color;
            resultsBoostSheen.SetActive(enable);
            if (enable == false) resultsBoostRatingButtonAnim.enabled = enable;

            resultsBoostRatingToolTipWithRv.gameObject.SetActive(false);
            resultsBoostRatingButtonWithRv.interactable = enable;
            resultsBoostRatingButtonWithRvImg.color = color;
            resultsBoostRatingGemIconWithRv.color = color;
            resultsBoostRatingGemsCostWithRv.color = color;
            resultsBoostRatingTextWithRv.color = color;
            rewardedVideoBtnImg.color = color;
            resultsBoostSheenWithRv.SetActive(enable);
            if (enable == false) resultsBoostRatingButtonAnimWithRv.enabled = enable;

            rewardedVideoBtn.interactable = enable;
            if (enable == false) rewardedVideoButtonAnimWithRv.enabled = enable;

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

        private void SetupFullAnalysisTab(bool availableForFree)
        {
            resultsFullAnalysisFreeTag.SetActive(availableForFree);
            resultsFullAnalysisFreeTitle.SetActive(availableForFree);
            resultsFullAnalysisSparkle.SetActive(!availableForFree);
            resultsFullAnalysisGemIcon.SetActive(!availableForFree);
            fullAnalysisGemsCount.enabled = !availableForFree;
            fullAnalysisBtn.interactable = true;

            AnimateFreeTagOnFullAnalysis(availableForFree);
        }

        public void OnRatingBoosted(int boostedRating)
        {
            analyticsService.Event(AnalyticsEventId.consumable_used, AnalyticsContext.rating_booster);
            PlayEloBoostedAnimation(boostedRating);
        }

        public void OnRewardDoubled()
        {
            resultsEarnedCoinsLabel.text = $"{resultsBetValue * 4}";
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

        public void SetupFullAnalysisPrice()
        {
            SetupFullAnalysisPrice(freeGameAnalysisAvailable);
        }

        private void SetupFullAnalysisPrice(bool availableForFree)
        {
            if (fullGameAnalysisStoreItem == null)
            {
                return;
            }

            haveEnoughGemsForFullAnalysis = availableForFree || playerModel.gems >= fullGameAnalysisStoreItem.currency3Cost;
            resultsRVAnalysisGemsCost.text = fullAnalysisGemsCount.text = fullGameAnalysisStoreItem.currency3Cost.ToString();
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

        private void UpdateMatchAnalysis()
        {
            bunders.text = matchAnalysis.blunders.ToString();
            perfect.text = matchAnalysis.perfectMoves.ToString();
            mistakes.text = matchAnalysis.mistakes.ToString();
            //resultsFullAnalysisProcessing.SetActive(!moveAnalysisCompleted);
        }

        private void OnPlayRewardedVideoClicked()
        {
            if (IsCoolDownComplete())
            {
                ratingBoosterRewardSignal.Dispatch();
                SetupRatingBoostButtonsSection(false);

            }
            else
            {
                EnableTimerTooltip();
            }
        }

        private void OnRVAnalytisWatchVideoButtonClicked()
        {
            if (IsCoolDownComplete())
            {
                rvAnalysisWatchVideoSignal.Dispatch();
            }
            else
            {
                resultsRVAnalysisTimerTooltip.SetActive(true);
                Invoke("DisableRVAnalysisTimerTooltip", 5);
            }
        }

        private void DisableRVAnalysisTimerTooltip()
        {
            resultsRVAnalysisTimerTooltip.SetActive(false);
        }

        public void OnRewardClaimed()
        {
            SetupRatingBoostButtonsSection(false);
        }

        public void EnableVideoAvailabilityTooltip()
        {
            videoNotAvailableTooltip.SetActive(true);
            Invoke("DisableVideoAvailabilityTooltip", 5);
            SetupRatingBoostButtonsSection(true);
        }

        public void EnableAnalysisRVNotAvailableTooltip()
        {
            resultsRVAnalysisNotAvailableTooltip.SetActive(true);
            Invoke("DisableAnalysisRVNotAvaillableTooltip", 5);
        }

        private void DisableAnalysisRVNotAvaillableTooltip()
        {
            resultsRVAnalysisNotAvailableTooltip.SetActive(false);
        }

        public void DisableVideoAvailabilityTooltip()
        {
            videoNotAvailableTooltip.SetActive(false);
        }

        public void EnableTimerTooltip()
        {
            timerRunningTooltip.SetActive(true);
            Invoke("DisableTimerTooltip", 5);
        }

        public void DisableTimerTooltip()
        {
            timerRunningTooltip.SetActive(false);
        }

        public bool IsCoolDownComplete()
        {
            return coolDownTimeUTC < serverClock.currentTimestamp;
        }

        public void StartTimer(long coolDownTime = 0)
        {
            coolDownTimeUTC = coolDownTime;
            UpdateTimerText();
            ratingBoosterTimer.SetActive(true);
            getRV.SetActive(false);
            resultsRVAnalysisTimerRunning.SetActive(true);
            resultsRVAnalysisWatchVideo.SetActive(false);
            schedulerSubscription.Dispatch(true);
        }

        public void SchedulerCallBack()
        {
            if (!IsCoolDownComplete())
            {
                UpdateTimerText();
            }
            else
            {
                schedulerSubscription.Dispatch(false);
                OnTimerCompleted();
            }
        }

        private void UpdateTimerText()
        {
            long timeLeft = coolDownTimeUTC - serverClock.currentTimestamp;
            if (timeLeft > 0)
            {
                timeLeft -= 1000;
                resultsRVAnalysisTimer.text = remainingCoolDownTime.text = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft));
            }
            else
            {
                resultsRVAnalysisTimer.text = remainingCoolDownTime.text = "0s";
            }
        }

        private void OnTimerCompleted()
        {
            ratingBoosterTimer.SetActive(false);
            resultsRVAnalysisTimerRunning.SetActive(false);
            getRV.SetActive(true);
            resultsRVAnalysisWatchVideo.SetActive(true);
            //rewardedVideoBtn.interactable = false;
            DisableTimerTooltip();
            resultsRVAnalysisTimerTooltip.SetActive(false);
        }

        public void UpdateRVTimer(long coolDownTimer, bool rvEnabled)
        {
            coolDownTimeUTC = coolDownTimer;
            isRatingBoosterRVEnabled = isRatingBoosterRVEnabled && rvEnabled;
            isAnalysisRVEnabled = isAnalysisRVEnabled && rvEnabled;
            SetupResultsLayout();
            SetupAnalysisRV(isAnalysisRVEnabled);
        }

        #region Animations

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

        void AnimateSparkes()
        {
            animSequence.Kill();
            animSequence = DOTween.Sequence();
            animSequence.AppendCallback(() => FadeInSparkles());
            animSequence.AppendInterval(1);
            animSequence.AppendCallback(() => FadeOutSparkes());
            animSequence.AppendInterval(4);
            animSequence.AppendCallback(() => AnimateSparkes());
        }

        void FadeInSparkles()
        {
            sparkles.DOFade(1, 1);

            sparkles.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 180), 1);
            sparkles.gameObject.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 1);
        }

        void FadeOutSparkes()
        {
            sparkles.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 0f), 1);
            sparkles.gameObject.transform.DOScale(new Vector3(1, 1, 1), 1);

            sparkles.DOFade(0, 1);
        }

        #endregion
    }
}
