/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using System.Linq;
using System.Collections;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class RewardsService : IRewardsService
    {
        // Listen to signals
        [Inject] public PromotionCycleOverSignal promotionCycleOverSignal { get; set; }
        [Inject] public InboxAddMessagesSignal inboxAddMessagesSignal { get; set; }
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public RateAppDlgClosedSignal rateAppDlgClosedSignal { get; set; }
        [Inject] public InboxEmptySignal inboxEmptySignal { get; set; }
        [Inject] public RankPromotedDlgClosedSignal rankPromotedDlgClosedSignal { get; set; }
        [Inject] public SpotCoinsPurchaseDlgClosedSignal spotCoinsPurchaseDlgClosedSignal { get; set; }

        // Models
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadRewardDlgViewSignal loadRewardDlgViewSignal { get; set; }
        [Inject] public UpdateSpotCoinsWatchAdDlgSignal updateSpotCoinsWatchAdDlgSignal { get; set; }
        [Inject] public LobbySequenceEndedSignal lobbySequenceEndedSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IRateAppService rateAppService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }

        private bool isPromotionShownOnStart;
        private Dictionary<string, string> rewards;
        private bool outOfCoinsPopupShown;
        private bool isDailyRewardDlgShown;
        private string[] rewardsPriority = { "RewardDailyLeague", "RewardLeaguePromotion", "RewardTournamentEnd" };

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Init);
            promotionCycleOverSignal.AddListener(SetPomotionFlag);
            inboxAddMessagesSignal.AddListener(LoadDailyRewards);
            inboxEmptySignal.AddListener(LoadDailyRewards);
        }

        private void Init()
        {
            isPromotionShownOnStart = false;
            outOfCoinsPopupShown = false;
            isDailyRewardDlgShown = false;
            rewards = new Dictionary<string, string>();
        }

        private void SetPomotionFlag()
        {
            isPromotionShownOnStart = true;
            LoadDailyRewards();
        }

        private void LoadDailyRewards()
        {
            if (isPromotionShownOnStart && navigatorModel.currentViewId == NavigatorViewId.LOBBY)
            {
                SetupRewards();
                LoadDailyReward();
            }
        }

        private void LoadDailyReward()
        {
            if (rewards.Count == 0)
            {
                OnRewardsOver();
                return;
            }

            var rewardKey = rewards.FirstOrDefault().Key;

            foreach (var reward in rewardsPriority)
            {
                if (rewards.ContainsValue(reward))
                {
                    if (reward.Equals("RewardDailyLeague"))
                    {
                        if (!isDailyRewardDlgShown)
                        {
                            isDailyRewardDlgShown = true;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    rewardKey = GetRewardKeyByValue(reward);
                    break;
                }
            }

            rewards.Remove(rewardKey);
            var onCloseSignal = new Signal();
            onCloseSignal.AddListener(LoadDailyReward);
            loadRewardDlgViewSignal.Dispatch(rewardKey, onCloseSignal);
        }

        private string GetRewardKeyByValue(string value)
        {
            return (from r in rewards
                    where r.Value.Equals(value)
                    select r).FirstOrDefault().Key;
        }

        private void SetupRewards()
        {
            rewards.Clear();

            foreach (var msg in inboxModel.items)
            {
                if (backendService.serverClock.currentTimestamp >= msg.Value.startTime)
                {
                    rewards.Add(msg.Key, msg.Value.type);
                }
            }
        }

        private void OnRewardsOver()
        {
            if (metaDataModel.ShowChampionshipNewRankDialog)
            {
                metaDataModel.ShowChampionshipNewRankDialog = false;
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CHAMPIONSHIP_NEW_RANK_DLG);
                rankPromotedDlgClosedSignal.AddOnce(ShowRateUsPopupAsync);
            }
            else
            {
                ShowRateUsPopup();
            }
        }

        private void ShowRateUsPopupAsync()
        {
            routineRunner.StartCoroutine(ShowRateUsPopupWithDelay());
        }

        IEnumerator ShowRateUsPopupWithDelay()
        {
            yield return new WaitForEndOfFrame();
            ShowRateUsPopup();
        }

        private void ShowRateUsPopup()
        {
            if (rateAppService.CanShowRateDialogue())
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_RATE_APP_DLG);
                rateAppDlgClosedSignal.AddOnce(ShowOutOfCoinsPopup);
            }
            else
            {
                ShowOutOfCoinsPopup();
            }
        }

        private void ShowOutOfCoinsPopup()
        {
            if (playerModel.coins < metaDataModel.settingsModel.bettingIncrements[0] && !outOfCoinsPopupShown)
            {
                outOfCoinsPopupShown = true;
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_COIN_PURCHASE);
                updateSpotCoinsWatchAdDlgSignal.Dispatch(0, metaDataModel.store.items["CoinPack1"], AdPlacements.Rewarded_coins_popup);
                spotCoinsPurchaseDlgClosedSignal.AddOnce(() => lobbySequenceEndedSignal.Dispatch());
            }
            else
            {
                lobbySequenceEndedSignal.Dispatch();
            }
        }
    }
}
