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

namespace TurboLabz.CPU
{
    public partial class GameView
    {
		[Header("Results Dialog")]
        public Button playbackOverlay;

        public GameObject resultsDialog;
        public Text resultsGameResultLabel;
        public Text resultsGameResultReasonLabel;
        public Text resultsFriendlyLabel;

        public Text resultsRatingTitleLabel;
        public Text resultsRatingValueLabel;
        public Text resultsRatingChangeLabel;

        public Button resultsCollectRewardButton;
        public Text resultsCollectRewardButtonLabel;
        public Text resultsCollectRewardHeadingLabel;
        public Button resultsCloseButton;
        public Text resultsCloseButtonLabel;

        public Image resultsAdTVImage;
        public Text resultsRewardCoinsLabel;
        public Image resultsVictoryRewardImage;
        public Image resultsDefeatRewardImage;
        public Text resultsEarnedLabel;

        public Button resultsSkipRewardButton;
        public Text resultsSkipRewardButtonLabel;

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

        private bool playerWins;
        private bool isDraw;
        private string adRewardType;
        private string collectRewardType;
        private float animDelay;
        private bool menuOpensResultsDlg;
        private int resultRewardCoins;

        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }

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
            resultsCloseButton.onClick.AddListener(OnResultsClosed);
            resultsSkipRewardButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);

            // Text Labels
            resultsCollectRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON);
            resultsCloseButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);
            resultsRatingTitleLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsSkipRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_SKIP_REWARD_BUTTON);

            resultsDialogHalfHeight = resultsDialog.GetComponent<RectTransform>().rect.height / 2f;
        }

        public void CleanupResults()
        {
            resultsCollectRewardButton.onClick.RemoveAllListeners();
            resultsCloseButton.onClick.RemoveAllListeners();
            resultsSkipRewardButton.onClick.RemoveAllListeners();
        }

        private void EnableRewarededVideoButton(bool enable)
        {
            if (enable)
            {
                resultsCollectRewardButton.interactable = true;
                resultsCollectRewardButtonLabel.color = Colors.ColorAlpha(Colors.YELLOW, Colors.ENABLED_TEXT_ALPHA);
                resultsCollectRewardHeadingLabel.color = Colors.ColorAlpha(Colors.WHITE, Colors.ENABLED_TEXT_ALPHA);
                Color c = resultsAdTVImage.color;
                c.a = Colors.FULL_ALPHA;
                resultsAdTVImage.color = c;

                analyticsService.Event(AnalyticsEventId.ads_rewared_available, AnalyticsContext.computer_match);

            }
            else
            {
                resultsCollectRewardButton.interactable = false;
                resultsCollectRewardButtonLabel.color = Colors.ColorAlpha(Colors.YELLOW, Colors.DISABLED_TEXT_ALPHA);
                resultsCollectRewardHeadingLabel.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                Color c = resultsAdTVImage.color;
                c.a = Colors.DISABLED_TEXT_ALPHA;
                resultsAdTVImage.color = c;

                analyticsService.Event(AnalyticsEventId.ads_rewared_failed, AnalyticsContext.computer_match);

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

            bool isRewardedButton = adsService.IsRewardedVideoAvailable();
            EnableRewarededVideoButton(isRewardedButton);

            viewBoardResultPanel.gameObject.SetActive(false);
        }

        public void HideResultsDialog()
        {
            resultsDialog.SetActive(false);
        }

        private void UpdateResultRatingSection(bool isRanked, int currentEloScore, int eloScoreDelta)
        {
            resultsFriendlyLabel.gameObject.SetActive(false);
            resultsRatingTitleLabel.gameObject.SetActive(true);
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
                resultsRatingChangeLabel.color = Colors.GREEN;
            }
            else if (eloScoreDelta < 0)
            {
                resultsRatingChangeLabel.text = "(" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.RED;
            }
        }

        private void UpdateGameEndReasonSection(GameEndReason gameEndReason)
        {
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
                resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DRAW);
                resultsGameResultLabel.color = Colors.YELLOW;
                viewBoardResultPanel.result.text = "Drawn";
            }
            else
            {
                if (playerWins)
                {
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN);
                    resultsGameResultLabel.color = Colors.GREEN;
                    viewBoardResultPanel.result.text = string.Format("{0} won", playerInfoPanel.GetComponentInChildren<ProfileView>().profileName.text);
                }
                else
                {
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE);
                    resultsGameResultLabel.color = Colors.RED;
                    viewBoardResultPanel.result.text = "Computer won";
                }
            }
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

            resultsVictoryRewardImage.gameObject.SetActive(playerWins);
            resultsDefeatRewardImage.gameObject.SetActive(!playerWins);

            resultsAdTVImage.gameObject.SetActive(!removeAds);
            resultsCollectRewardButton.gameObject.SetActive(!removeAds);
            resultsCollectRewardButtonLabel.gameObject.SetActive(!removeAds);
            resultsCollectRewardHeadingLabel.gameObject.SetActive(!removeAds);

            if (removeAds)
            {
                resultsDialog.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1050.0f);
            }

            int rewardCoins = rewardsSettingsModel.getRewardCoins(AdType.Interstitial, powerupUsage, playerWins);

            // Reward
            resultsRewardCoinsLabel.text = rewardCoins + " Coins"; 
            if (playerWins)
            {
                resultsEarnedLabel.text = localizationService.Get(LocalizationKey.RESULTS_REWARD);
            }
            else
            {
                resultsEarnedLabel.text = localizationService.Get(LocalizationKey.RESULTS_EARNED);
            }

            adRewardType = playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN_AD : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN_AD;
            collectRewardType = playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN;
            resultRewardCoins = rewardCoins;
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
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.RewardedVideo;
            vo.rewardType = adRewardType;
            vo.challengeId = "";
            vo.playerWins = playerWins;
            showAdSignal.Dispatch(vo);

           // showAdSignal.Dispatch(AdType.RewardedVideo, adRewardType);
            backToLobbySignal.Dispatch();

            analyticsService.Event(AnalyticsEventId.ads_collect_reward, AnalyticsContext.computer_match);         
            analyticsService.Event(AnalyticsEventId.ads_rewared_show, AnalyticsContext.computer_match);
            
        }

        public void OnResultsSkipRewardButtonClicked()
        {
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.Interstitial;
            vo.rewardType = collectRewardType;
            vo.challengeId = "";
            vo.playerWins = playerWins;
            showAdSignal.Dispatch(vo);

            //showAdSignal.Dispatch(AdType.Interstitial, collectRewardType);
            backToLobbySignal.Dispatch();

            analyticsService.Event(AnalyticsEventId.ads_skip_reward, AnalyticsContext.computer_match);
        }

        private void OnResultsClosed()
        {
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
    }
}
