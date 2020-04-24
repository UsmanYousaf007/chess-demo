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
using HUFEXT.CrossPromo.API;
using strange.extensions.promise.api;
using HUFEXT.CrossPromo.Implementation;

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

        public Button resultsCollectRewardButton;
        public Text resultsCollectRewardButtonLabel;
        public Button resultsViewBoardButton;
        public Text resultsViewBoardButtonLabel;
        public Image resultsAdTVImage;

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
        private bool menuOpensResultsDlg;
        private int resultRewardCoins;

        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

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
            resultsViewBoardButton.onClick.AddListener(OnResultsClosed);
            resultsSkipRewardButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);
            showCrossPromoButton.onClick.AddListener(OnCrossPromoButtonClicked);

            // Text Labels
            resultsCollectRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON);
            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsSkipRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_SKIP_REWARD_BUTTON);
            earnRewardsText.text = localizationService.Get(LocalizationKey.RESULTS_EARNED);
            resultsViewBoardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);

            resultsDialogHalfHeight = resultsDialog.GetComponent<RectTransform>().rect.height / 2f;
            rewardBarOriginalWidth = rewardBar.sizeDelta.x;
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

                analyticsService.Event(AnalyticsEventId.ads_rewarded_available, AnalyticsContext.computer_match);

            }
            else
            {
                resultsCollectRewardButton.interactable = false;
                resultsCollectRewardButtonLabel.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                Color c = resultsAdTVImage.color;
                c.a = Colors.DISABLED_TEXT_ALPHA;
                resultsAdTVImage.color = c;

                analyticsService.Event(AnalyticsEventId.ads_rewarded_failed, AnalyticsContext.computer_match);

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

            viewBoardResultPanel.gameObject.SetActive(false);

            showCrossPromoButton.gameObject.SetActive(HCrossPromo.service.hasContent);

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
            EnableRewarededVideoButton(true);
            viewBoardResultPanel.reason.text = "";
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
                        viewBoardResultPanel.reason.text = string.Format("{0} resigned", playerInfoPanel.GetComponentInChildren<ProfileView>().profileName.text);
                        EnableRewarededVideoButton(preferencesModel.resignCount <= adsSettingsModel.resignCap);
                    }
                    else
                    {
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_OPPONENT);
                        viewBoardResultPanel.reason.text = "Computer resigned";

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
            }
            else
            {
                if (playerWins)
                {
                    resultsGameImage.sprite = winSprite;
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN);
                    resultsGameResultLabel.color = Colors.GREEN_DIM;
                    viewBoardResultPanel.result.text = string.Format("{0} won", playerInfoPanel.GetComponentInChildren<ProfileView>().profileName.text);
                }
                else
                {
                    resultsGameImage.sprite = defeatSprite;
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE);
                    resultsGameResultLabel.color = Colors.RED_DIM;
                    viewBoardResultPanel.result.text = "Computer won";
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

            UpdateResultRatingSection(false, 1, 0);
            UpdateGameEndReasonSection(gameEndReason);
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

            resultsAdTVImage.gameObject.SetActive(!removeAds);
            resultsCollectRewardButton.gameObject.SetActive(!removeAds);
            resultsCollectRewardButtonLabel.gameObject.SetActive(!removeAds);

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
            showRewardedAdSignal.Dispatch(vo);

           // showAdSignal.Dispatch(AdType.RewardedVideo, adRewardType);
            backToLobbySignal.Dispatch();

            analyticsService.Event(AnalyticsEventId.ads_collect_reward, AnalyticsContext.computer_match);         
            analyticsService.Event(AnalyticsEventId.ads_rewarded_show, AnalyticsContext.computer_match);
            
        }

        public void OnResultsSkipRewardButtonClicked()
        {
            audioService.PlayStandardClick();
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.Interstitial;
            vo.rewardType = GSBackendKeys.ClaimReward.NONE;
            vo.challengeId = "";
            vo.playerWins = playerWins;
            showAdSignal.Dispatch(vo);

            //showAdSignal.Dispatch(AdType.Interstitial, collectRewardType);
            backToLobbySignal.Dispatch();

            analyticsService.Event(AnalyticsEventId.ads_skip_reward, AnalyticsContext.computer_match);
        }

        private void OnResultsClosed()
        {
            audioService.PlayStandardClick();
            HideResultsDialog();
            //playbackOverlay.gameObject.SetActive(true);
            menuOpensResultsDlg = true;
            EnableMenuButton();
            DisableModalBlocker();
            viewBoardResultPanel.gameObject.SetActive(true);
        }

        private void OnPlaybackOverlayClicked()
        {
            playbackOverlay.gameObject.SetActive(false);
            ShowResultsDialog();
        }

        private void OnCrossPromoButtonClicked()
        {
            toggleBannerSignal.Dispatch(false);
            hAnalyticsService.LogEvent(AnalyticsEventId.cross_promo_clicked.ToString());

            IPromise promise = HCrossPromo.OpenPanel();
            if (promise != null)
            {
                promise.Then(ToggleBannerSignalFunc);
            }
        }

        private void ToggleBannerSignalFunc()
        {
            toggleBannerSignal.Dispatch(true);
        }
    }
}
