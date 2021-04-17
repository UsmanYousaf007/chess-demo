/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:37:45 UTC+05:00
/// 
/// @description
/// [add_description_here]


using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using strange.extensions.signal.impl;

using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.Chess;
using TurboLabz.InstantGame;
using strange.extensions.promise.api;
using HUFEXT.CrossPromo.Runtime.API;
using TMPro;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
		[Header("Results Dialog")]
        public Button playbackOverlay;

        public GameObject resultsDialog;
        public CanvasGroup resultsDialogCanvasGroup;

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
        public Image resultsBoostRatingIcon;
        public Image resultsBoostRatingGemIcon;
        public GameObject resultBoostSheen;

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

        public Image resultsPowerplayImage;
        public Sprite powerPlayOnSprite;
        public Sprite powerPlayOffSprite;

        public Button fullAnalysisBtn;
        public TMP_Text fullAnalysisGemsCount;
        public TMP_Text bunders;
        public TMP_Text mistakes;
        public TMP_Text perfect;

        public GameObject resultsFullAnalysisdPanel;
        public GameObject resultsFullAnalysisDisabledPanel;

        public RectTransform[] resultsLayouts;

        public GameObject rewardedVideoPanel;

        public Signal resultsStatsButtonClickedSignal = new Signal();
        public Signal showAdButtonClickedSignal = new Signal();
        public Signal resultsDialogClosedSignal = new Signal();
        public Signal resultsDialogOpenedSignal = new Signal();
        public Signal backToLobbySignal = new Signal();

        private const float RESULTS_DELAY_TIME = 1f;
        private const float RESULTS_SHORT_DELAY_TIME = 0.3f;
        private const float RESULTS_DIALOG_DURATION = 0.5f;
        private float resultsDialogHalfHeight;

        private bool playerWins;
        private bool isDraw;
        private float animDelay;

        [HideInInspector] public bool menuOpensResultsDlg;

        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        private void InitResultsCPU()
        {
            playbackOverlay.onClick.AddListener(OnPlaybackOverlayClicked);
            playbackOverlay.gameObject.SetActive(false);
            UIDlgManager.Setup(resultsDialog);
        }

        public void OnParentShowResults()
        {
            menuOpensResultsDlg = false;
            HideResultsDialog();
        }

        private void ShowResultsDialogCPU()
        {
            DeactivateThink();
        }

        public void InitResults()
        {
            resultsBoostRatingButton.onClick.AddListener(OnResultsBoostRatingButtonClicked);
            resultsViewBoardButton.onClick.AddListener(OnResultsClosed);
            resultsContinueButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);
            fullAnalysisBtn.onClick.AddListener(OnFullAnalysisButtonClicked);
            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsBoostRatingToolTipText.text = localizationService.Get(LocalizationKey.RESULTS_BOOST_FRIENDLY);
            resultsViewBoardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);

            InitResultsCPU();
            SetupRatingBoostButton();
        }

        public void CleanupResults()
        {
            resultsViewBoardButton.onClick.RemoveAllListeners();
            resultsContinueButton.onClick.RemoveAllListeners();
            resultsDoubleRewardButton.onClick.RemoveAllListeners();
        }

        public void SetGameAnalysisPanel()
        {
            resultsFullAnalysisdPanel.gameObject.SetActive(false);
            resultsFullAnalysisDisabledPanel.gameObject.SetActive(true);
        }


        public void ShowResultsDialog()
        {
            ShowResultsDialogCPU();
            SetGameAnalysisPanel();
            EnableModalBlocker();
            if (menuOpensResultsDlg)
            {
                UIDlgManager.Show(resultsDialog);
            }
            else {
                resultsDialogCanvasGroup.alpha = 0;
                resultsDialog.SetActive(true);
            }

            DisableMenuButton();
            HidePossibleMoves();

            if (!ArePlayerMoveIndicatorsVisible())
            {
                HidePlayerToIndicator();
            }

            HideSafeMoveBorder();
            ShowViewBoardResultsPanel(false);

            preferencesModel.isRateAppDialogueShown = false;
            appInfoModel.gameMode = GameMode.NONE;
        }

        public void ShowViewBoardResultsPanel(bool show)
        {
            viewBoardResultPanel.gameObject.SetActive(show);
        }

        public void HideResultsDialog()
        {
            UIDlgManager.Hide(resultsDialog);
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
                //DoPulse(false);
                return;
            }

            // Ranked Game
            resultsRatingValueLabel.gameObject.SetActive(true);
            resultsRatingChangeLabel.gameObject.SetActive(true);
            resultsRatingChangeLabel.gameObject.SetActive(false);

            resultsRatingValueLabel.text = currentEloScore.ToString();

            if (eloScoreDelta > 0)
            {
                resultsRatingChangeLabel.text = "(+" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.GREEN_LIGHT;
            }
            else if (eloScoreDelta < 0)
            {
                resultsRatingChangeLabel.text = "(" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.RED_LIGHT;
            }
        }

        private void UpdateGameEndReasonSection(GameEndReason gameEndReason)
        {
            //EnableRewarededVideoButton(true);
            //bool enablePulse = true;
            viewBoardResultPanel.reason.text = "";

            string analyName = AnalyticsEventId.cpu_end_lvl_.ToString() + cpuGameModel.cpuStrength;

            switch (gameEndReason)
            {
                case GameEndReason.TIMER_EXPIRED:
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_TIMER_EXPIRED);
                    break;

                case GameEndReason.CHECKMATE:
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_CHECKMATE);

                    if(playerWins)
                        analyticsService.Event(analyName, AnalyticsContext.won_checkmate);
                    else
                        analyticsService.Event(analyName, AnalyticsContext.lost_checkmate);
                    break;

                case GameEndReason.RESIGNATION:
                    if (!playerWins)
                    {
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_PLAYER);
                        animDelay = RESULTS_SHORT_DELAY_TIME;
                        viewBoardResultPanel.reason.text = string.Format("{0} resigned", playerInfoPanel.GetComponentInChildren<ProfileView>().profileName.text);
                        //EnableRewarededVideoButton(preferencesModel.resignCount <= adsSettingsModel.resignCap);
                        //enablePulse = preferencesModel.resignCount <= adsSettingsModel.resignCap;
                    }
                    else
                    {
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_OPPONENT);
                        viewBoardResultPanel.reason.text = "Computer resigned";

                    }

                    if (playerWins)
                        analyticsService.Event(analyName, AnalyticsContext.won_resign);
                    else
                        analyticsService.Event(analyName, AnalyticsContext.lost_resign);

                    break;

                case GameEndReason.STALEMATE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_STALEMATE);
                    analyticsService.Event(analyName, AnalyticsContext.draw_stalemate);
                    break;

                case GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL);
                    analyticsService.Event(analyName, AnalyticsContext.draw_insufficient_material);
                    break;

                case GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITH_MOVE:
                case GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE);
                    analyticsService.Event(analyName, AnalyticsContext.draw_fifty_move);
                    break;

                case GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITH_MOVE:
                case GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE);
                    analyticsService.Event(analyName, AnalyticsContext.draw_threefold_repetition);
                    break;

                case GameEndReason.PLAYER_DISCONNECTED:
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED);
                    viewBoardResultPanel.reason.text = string.Format("{0} left", playerInfoPanel.GetComponentInChildren<ProfileView>().profileName.text);
                    break;

                case GameEndReason.DRAW_BY_DRAW_OFFERED:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_OFFERED_DRAW);
                    break;

                default:
                    resultsGameResultReasonLabel.text = "Unknown Reason";
                    break;
            }

            //DoPulse(enablePulse);

            if (string.IsNullOrEmpty(viewBoardResultPanel.reason.text))
            {
                viewBoardResultPanel.reason.text = resultsGameResultReasonLabel.text;
            }
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
                viewBoardResultPanel.result.text = string.Format("{0} won", playerInfoPanel.GetComponentInChildren<ProfileView>().profileName.text);
            }
            else
            {
                resultsGameImage.sprite = defeatSprite;
                resultsGameResultLabel.sprite = defeatText;
                viewBoardResultPanel.result.text = "Computer won";
            }

            resultsGameImage.SetNativeSize();
            resultsGameResultLabel.SetNativeSize();
        }

        public void UpdateResultsDialog(GameEndReason gameEndReason, bool isPlayerWins, int powerupUsage, bool removeAds)
        {
            DisableInteraction();
            EnableModalBlocker();

            isDraw = false;
            animDelay = RESULTS_DELAY_TIME;
            playerWins = isPlayerWins;

            UpdateGameEndReasonSection(gameEndReason);
            UpdateResultRatingSection(false, 1, 0);
            UpdateGameResultHeadingSection();
            SetupResultsLayout(false);
            //BuildLayout();

            //resultsDialog.transform.localPosition = new Vector3(0f, Screen.height + resultsDialogHalfHeight, 0f);
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
        }

        public bool IsResultsDialogVisible()
        {
            return resultsDialog.activeSelf;
        }

        public void ExitPlaybackMode()
        {
            if (playbackOverlay.gameObject.activeSelf)
            {
                OnPlaybackOverlayClicked();
            }
        }

        private void AnimateResultsDialog()
        {
            //resultsDialog.transform.DOLocalMove(Vector3.zero, RESULTS_DIALOG_DURATION).SetEase(Ease.OutBack);
            UIDlgManager.Show(resultsDialog).Then(()=> BuildLayout());

            if (isDraw || !playerWins)
            {
                audioService.Play(audioService.sounds.SFX_DEFEAT);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_VICTORY);
            }
        }

        private bool IsResultsDialogActive()
        {
            return resultsDialog.activeSelf;
        }

        private void OnResultsBoostRatingButtonClicked()
        {
            audioService.PlayStandardClick();
            resultsBoostRatingToolTip.SetActive(true);
        }

        public void OnResultsSkipRewardButtonClicked()
        {
            audioService.PlayStandardClick();
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.Interstitial;
            vo.rewardType = GSBackendKeys.ClaimReward.NONE;
            vo.challengeId = "";
            vo.playerWins = playerWins;
            vo.placementId = AdPlacements.Interstitial_endgame;
            playerModel.adContext = AnalyticsContext.interstitial_endgame;
            
            showAdSignal.Dispatch(vo, false);
        }

        private void OnFullAnalysisButtonClicked()
        {
            //add code here
        }

        private void OnResultsClosed()
        {
            audioService.PlayStandardClick();
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU);
            //playbackOverlay.gameObject.SetActive(true);
            menuOpensResultsDlg = true;
            EnableMenuButton();
            DisableModalBlocker();
            ShowViewBoardResultsPanel(true);
        }

        private void OnPlaybackOverlayClicked()
        {
            playbackOverlay.gameObject.SetActive(false);
            showResultsDlgSignal.Dispatch();
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

        private void SetupRatingBoostButton()
        {
            /*
            var color = Colors.WHITE_100;
            resultsBoostRatingToolTip.gameObject.SetActive(false);
            resultsBoostRatingButton.interactable = true;
            resultsBoostRatingButton.image.color = color;
            resultsBoostRatingGemIcon.color = color;
            resultsBoostRatingGemsCost.color = color;
            resultsBoostRatingIcon.color = color;
            resultBoostSheen.SetActive(false);
            */
        }

        private void SetupResultsLayout(bool isRankedGame)
        {
            rewardedVideoPanel.SetActive(false);
            resultsBoostRatingButton.transform.parent.gameObject.SetActive(false);
            resultsBetReversedLabel.gameObject.SetActive(isDraw && isRankedGame);
            resultsRewardLabel.gameObject.SetActive(playerWins && isRankedGame);
            resultsRewardsCoins.gameObject.SetActive(isDraw || playerWins && isRankedGame);
            resultsRewardsStars.gameObject.SetActive(playerWins && isRankedGame);
            resultsDoubleRewardButton.gameObject.SetActive(playerWins && isRankedGame);

            //resultsContinueButton.gameObject.SetActive(playerWins && isRankedGame);
            //resultsContinueButton.gameObject.SetActive(!playerWins || isDraw || !isRankedGame);

            resultsRatingContainer.gameObject.SetActive(isRankedGame);
            resultsPowerplayImage.gameObject.SetActive(false);
        }

        private void BuildLayout()
        {
            foreach (var layout in resultsLayouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }
        }
    }
}
