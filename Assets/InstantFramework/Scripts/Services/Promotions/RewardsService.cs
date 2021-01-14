/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;

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

        // Models
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadRewardDlgViewSignal loadRewardDlgViewSignal { get; set; }
        [Inject] public UpdateSpotCoinsWatchAdDlgSignal updateSpotCoinsWatchAdDlgSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IRateAppService rateAppService { get; set; }

        private bool isPromotionShownOnStart;
        private Stack<string> rewards;

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
            rewards = new Stack<string>();
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

            var onCloseSignal = new Signal();
            onCloseSignal.AddListener(LoadDailyReward);
            loadRewardDlgViewSignal.Dispatch(rewards.Pop(), onCloseSignal);
        }

        private void SetupRewards()
        {
            rewards.Clear();

            foreach (var msg in inboxModel.items)
            {
                if (TimeUtil.unixTimestampMilliseconds >= msg.Value.startTime)
                {
                    rewards.Push(msg.Key);
                }
            }
        }

        private void OnRewardsOver()
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
            if (playerModel.coins < metaDataModel.settingsModel.bettingIncrements[0])
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_COIN_PURCHASE);
                updateSpotCoinsWatchAdDlgSignal.Dispatch(0, metaDataModel.store.items["CoinPack1"], AdPlacements.Rewarded_coins_popup);
            }
        }
    }
}
