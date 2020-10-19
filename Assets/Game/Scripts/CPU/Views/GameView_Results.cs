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

namespace TurboLabz.CPU
{
    public partial class GameView
    {
		[Header("Results Dialog")]
        public Button playbackOverlay;

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
        public GameObject resultsBoostRatingTooltip;
        public Text resultsBoostRatingTooltipText;

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

        private const float RESULTS_DELAY_TIME = 1f;
        private const float RESULTS_SHORT_DELAY_TIME = 0.3f;
        private const float RESULTS_DIALOG_DURATION = 0.5f;
        private float resultsDialogHalfHeight;
        private float declinedDialogHalfHeight;
        private float rewardBarOriginalWidth;

        private bool playerWins;
        private bool isDraw;
        private string adRewardType;
        private string collectRewardType;
        private float animDelay;
        public bool menuOpensResultsDlg;
        private int resultRewardCoins;

        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        private void InitResultsCPU()
        {
            playbackOverlay.onClick.AddListener(OnPlaybackOverlayClicked);
            playbackOverlay.gameObject.SetActive(false);
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
            InitResultsCPU();

            // Button listeners
            resultsCollectRewardButton.onClick.AddListener(OnResultsCollectRewardButtonClicked);
            resultsBoostRatingButton.onClick.AddListener(OnResultsBoostRatingButtonClicked);
            resultsViewBoardButton.onClick.AddListener(OnResultsClosed);
            resultsSkipRewardButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);
            showCrossPromoButton.onClick.AddListener(OnCrossPromoButtonClicked);

            // Text Labels
            resultsCollectRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON);
            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsSkipRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_SKIP_REWARD_BUTTON);
            earnRewardsText.text = localizationService.Get(LocalizationKey.RESULTS_EARNED);
            resultsViewBoardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);
            resultsBoostRatingTooltipText.text = localizationService.Get(LocalizationKey.RESULTS_BOOST_FRIENDLY);

            resultsDialogHalfHeight = resultsDialog.GetComponent<RectTransform>().rect.height / 2f;
            rewardBarOriginalWidth = rewardBar.sizeDelta.x;
            SetupRatingBoostButton();
        }

        public void CleanupResults()
        {
            resultsCollectRewardButton.onClick.RemoveAllListeners();
            resultsViewBoardButton.onClick.RemoveAllListeners();
            resultsSkipRewardButton.onClick.RemoveAllListeners();
        }

        private void EnableRewarededVideoButton(bool enable)
        {
            if (enable)
            {
                resultsCollectRewardButton.interactable = true;
                resultsCollectRewardButtonLabel.color = Colors.ColorAlpha(Colors.WHITE, Colors.ENABLED_TEXT_ALPHA);
                Color c = resultsAdTVImage.color;
                c.a = Colors.FULL_ALPHA;
                resultsAdTVImage.color = c;

                resultsBoostRatingButton.interactable = true;
                resultsBoostRatingButtonLabel.color = Colors.ColorAlpha(Colors.BLACK, 150f/255f);
                c = resultsBoostRatingAdTVImage.color;
                c.a = Colors.FULL_ALPHA;
                resultsBoostRatingAdTVImage.color = c;
            }
            else
            {
                resultsCollectRewardButton.interactable = false;
                resultsCollectRewardButtonLabel.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                Color c = resultsAdTVImage.color;
                c.a = Colors.DISABLED_TEXT_ALPHA;
                resultsAdTVImage.color = c;

                resultsBoostRatingButton.interactable = false;
                resultsBoostRatingButtonLabel.color = Colors.ColorAlpha(Colors.BLACK, 80f / 255f);
                c = resultsBoostRatingAdTVImage.color;
                c.a = Colors.DISABLED_TEXT_ALPHA;
                resultsBoostRatingAdTVImage.color = c;
            }
        }

        void DoPulse(bool val)
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

        public void ShowResultsDialog()
        {
            ShowResultsDialogCPU();

            EnableModalBlocker();
            resultsDialog.SetActive(true);
            DisableMenuButton();
            HidePossibleMoves();

            if (!ArePlayerMoveIndicatorsVisible())
            {
                HidePlayerToIndicator();
            }

            HideSafeMoveBorder();
            ShowViewBoardResultsPanel(false);
            if (HCrossPromo.service != null)
            {
                showCrossPromoButton.gameObject.SetActive(HCrossPromo.service.hasContent);
            }
            appInfoModel.gameMode = GameMode.NONE;
        }

        public void ShowViewBoardResultsPanel(bool show)
        {
            viewBoardResultPanel.gameObject.SetActive(show);
        }

        public void HideResultsDialog()
        {
            resultsDialog.SetActive(false);
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
                resultsRatingChangeLabel.color = Colors.GREEN_DIM;
            }
            else if (eloScoreDelta < 0)
            {
                resultsRatingChangeLabel.text = "(" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.RED_DIM;
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
                    viewBoardResultPanel.result.text = string.Format("{0} won", playerInfoPanel.GetComponentInChildren<ProfileView>().profileName.text);
                    resultsBoostRatingButtonLabel.text = $"{localizationService.Get(LocalizationKey.RESULTS_BOOST_RATING_BUTTON)} +{rewardsSettingsModel.ratingBoostReward}";
                }
                else
                {
                    resultsGameImage.sprite = defeatSprite;
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE);
                    resultsGameResultLabel.color = Colors.RED_DIM;
                    viewBoardResultPanel.result.text = "Computer won";
                    resultsBoostRatingButtonLabel.text = $"{localizationService.Get(LocalizationKey.RESULTS_RECOVER_RATING_BUTTON)} +{rewardsSettingsModel.ratingBoostReward}";
                }
            }

            resultsGameImage.SetNativeSize();
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

            //resultsAdTVImage.gameObject.SetActive(!removeAds);
            //resultsCollectRewardButton.gameObject.SetActive(!removeAds);
            //resultsCollectRewardButtonLabel.gameObject.SetActive(!removeAds);

            if (removeAds)
            {
                resultsDialog.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1050.0f);
            }

            int rewardCoins = rewardsSettingsModel.getRewardCoins(AdType.Interstitial, powerupUsage, playerWins);

            // Reward
            adRewardType = playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN_AD : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN_AD;
            collectRewardType = playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN;
            resultRewardCoins = rewardCoins;

            dailogueBg.enabled = false;
            earnRewardsSection.SetActive(!playerModel.HasSubscription());
            dailogueBg.enabled = true;

            var barFillPercentage = playerModel.rewardCurrentPoints / playerModel.rewardPointsRequired;
            rewardBar.sizeDelta = new Vector2(rewardBarOriginalWidth * barFillPercentage, rewardBar.sizeDelta.y);
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
            vo.challengeId = "";
            vo.playerWins = playerWins;
            playerModel.adContext = AnalyticsContext.rewarded;
            analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
            showRewardedAdSignal.Dispatch(vo);
        }

        private void OnResultsBoostRatingButtonClicked()
        {
            audioService.PlayStandardClick();
            resultsBoostRatingTooltip?.SetActive(true);
            //ResultAdsVO vo = new ResultAdsVO();
            //vo.adsType = AdType.RewardedVideo;
            //vo.rewardType = GSBackendKeys.ClaimReward.TYPE_BOOST_RATING;
            //vo.challengeId = "";
            //vo.playerWins = playerWins;
            //playerModel.adContext = AnalyticsContext.rewarded;
            //analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
            //showRewardedAdSignal.Dispatch(vo);
        }

        public void OnResultsSkipRewardButtonClicked()
        {
            audioService.PlayStandardClick();
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.Interstitial;
            vo.rewardType = GSBackendKeys.ClaimReward.NONE;
            vo.challengeId = "";
            vo.playerWins = playerWins;
            playerModel.adContext = AnalyticsContext.interstitial_endgame;
            if (!playerModel.HasSubscription())
            {
                analyticsService.Event(AnalyticsEventId.ad_user_requested, playerModel.adContext);
            }
            showAdSignal.Dispatch(vo, false);
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
            //toggleBannerSignal.Dispatch(true);
        }

        private void SetupRatingBoostButton()
        {
            var c = resultsBoostRatingAdTVImage.color;
            resultsBoostRatingTooltip?.SetActive(false);
            resultsBoostRatingButtonLabel.color = Colors.ColorAlpha(Colors.BLACK_DIM, Colors.DISABLED_TEXT_ALPHA);
            c = resultsBoostRatingAdTVImage.color;
            c.a = Colors.DISABLED_TEXT_ALPHA;
            resultsBoostRatingAdTVImage.color = c;
            resultsBoostRatingButton.GetComponent<Image>().color = c;
        }
    }
}
