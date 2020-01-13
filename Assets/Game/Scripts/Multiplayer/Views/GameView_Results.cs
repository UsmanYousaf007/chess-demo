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

        public Signal refreshLobbySignal = new Signal();

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
        private string playerName;
        private string opponentName;
        private string challengeId;
        
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }


        public void InitResults()
        {
            // Button listeners
            resultsCollectRewardButton.onClick.AddListener(OnResultsCollectRewardButtonClicked);
            declinedLobbyButton.onClick.AddListener(OnResultsDeclinedButtonClicked);
            resultsCloseButton.onClick.AddListener(OnResultsClosed);
            resultsSkipRewardButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);

            // Text Labels
            resultsCollectRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON);
            resultsCloseButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);
            resultsRatingTitleLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsSkipRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_SKIP_REWARD_BUTTON);

            declinedHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DECLINED);
            declinedReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DECLINED);
            declinedLobbyButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_RESULTS_BACK);
		
            resultsDialogHalfHeight = resultsDialog.GetComponent<RectTransform>().rect.height / 2f;
            declinedDialogHalfHeight = declinedDialog.GetComponent<RectTransform>().rect.height / 2f;
        }

        public void CleanupResults()
        {
            resultsCollectRewardButton.onClick.RemoveAllListeners();
            declinedLobbyButton.onClick.RemoveAllListeners();
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

                if(isLongPlay)
                {
                    analyticsService.Event(AnalyticsEventId.ads_rewared_available, AnalyticsContext.long_match);
                }
                else
                {
                    analyticsService.Event(AnalyticsEventId.ads_rewared_available, AnalyticsContext.quick_match);
                }
                
            }
            else
            {
                resultsCollectRewardButton.interactable = false;
                resultsCollectRewardButtonLabel.color = Colors.ColorAlpha(Colors.YELLOW, Colors.DISABLED_TEXT_ALPHA);
                resultsCollectRewardHeadingLabel.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                Color c = resultsAdTVImage.color;
                c.a = Colors.DISABLED_TEXT_ALPHA;
                resultsAdTVImage.color = c;

                if (isLongPlay)
                {
                    analyticsService.Event(AnalyticsEventId.ads_rewared_failed, AnalyticsContext.long_match);
                }
                else
                {
                    analyticsService.Event(AnalyticsEventId.ads_rewared_failed, AnalyticsContext.quick_match);
                }
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

            bool isRewardedButton = adsService.IsRewardedVideoAvailable();
            EnableRewarededVideoButton(isRewardedButton);

            viewBoardResultPanel.gameObject.SetActive(false);
        }

        public void HideResultsDialog()
        {
            resultsDialog.SetActive(false);
            declinedDialog.SetActive(false);
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
            resultsRatingChangeLabel.gameObject.SetActive(false);

            resultsRatingValueLabel.text = currentEloScore.ToString();

            if (eloScoreDelta > 0)
            {
                resultsRatingChangeLabel.text = "(+" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.GREEN;
                resultsRatingChangeLabel.gameObject.SetActive(true);
            }
            else if (eloScoreDelta < 0)
            {
                resultsRatingChangeLabel.text = "(" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.RED;
                resultsRatingChangeLabel.gameObject.SetActive(true);
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
                        viewBoardResultPanel.reason.text = string.Format("{0} resigned", playerName);
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
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED);
                        viewBoardResultPanel.reason.text = string.Format("{0} left", playerName);
                    }
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
                    viewBoardResultPanel.result.text = string.Format("{0} won", playerName);
                }
                else
                {
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE);
                    resultsGameResultLabel.color = Colors.RED;
                    viewBoardResultPanel.result.text = string.Format("{0} won", opponentName);
                }
            }
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

            UpdateResultRatingSection(vo.isRanked, vo.currentEloScore, vo.eloScoreDelta);
            UpdateGameEndReasonSection(vo.reason);
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

            resultsAdTVImage.gameObject.SetActive(!vo.removeAds);
            resultsCollectRewardButton.gameObject.SetActive(!vo.removeAds);
            resultsCollectRewardButtonLabel.gameObject.SetActive(!vo.removeAds);
            resultsCollectRewardHeadingLabel.gameObject.SetActive(!vo.removeAds);

            if (vo.removeAds)
            {
                resultsDialog.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1050.0f);
            }

            int rewardCoins = rewardsSettingsModel.getRewardCoins(AdType.Interstitial, vo.powerupUsedCount, playerWins);

            // Reward
            resultsRewardCoinsLabel.text = rewardCoins + " Coins";
            adRewardType = vo.playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN_AD : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN_AD;
            collectRewardType = vo.playerWins ? GSBackendKeys.ClaimReward.TYPE_MATCH_WIN : GSBackendKeys.ClaimReward.TYPE_MATCH_RUNNERUP_WIN;
            challengeId = vo.challengeId;

            if (playerWins)
            {
                resultsEarnedLabel.text = localizationService.Get(LocalizationKey.RESULTS_REWARD);
            }
            else
            {
                resultsEarnedLabel.text = localizationService.Get(LocalizationKey.RESULTS_EARNED);
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
            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.RewardedVideo;
            vo.rewardType = adRewardType;
            vo.challengeId = challengeId;
            vo.playerWins = playerWins;
            showAdSignal.Dispatch(vo);

            //showAdSignal.Dispatch(AdType.RewardedVideo, adRewardType);

            if (isLongPlay)
            {
                backToLobbySignal.Dispatch();
                analyticsService.Event(AnalyticsEventId.ads_collect_reward, AnalyticsContext.long_match);

                analyticsService.Event(AnalyticsEventId.ads_rewared_show, AnalyticsContext.long_match);

            }
            else
            {
                backToLobbySignal.Dispatch();
                analyticsService.Event(AnalyticsEventId.ads_collect_reward, AnalyticsContext.quick_match);

                analyticsService.Event(AnalyticsEventId.ads_rewared_show, AnalyticsContext.quick_match);

            }
        }

        private void OnResultsDeclinedButtonClicked()
        {
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
            uiBlocker.SetActive(false);
            resultsDialogClosedSignal.Dispatch();
            viewBoardResultPanel.gameObject.SetActive(true);
        }

        public void OnResultsSkipRewardButtonClicked()
        {
            backToLobbySignal.Dispatch();
            refreshLobbySignal.Dispatch();

            ResultAdsVO vo = new ResultAdsVO();
            vo.adsType = AdType.Interstitial;
            vo.rewardType = collectRewardType;
            vo.challengeId = challengeId;
            vo.playerWins = playerWins;
            showAdSignal.Dispatch(vo);

            if (isLongPlay)
            { 
                analyticsService.Event(AnalyticsEventId.ads_skip_reward, AnalyticsContext.long_match);
            }
            else
            {
                analyticsService.Event(AnalyticsEventId.ads_skip_reward, AnalyticsContext.quick_match);
            }
        }
    }
}
