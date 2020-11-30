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

        // Models
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadRewardDlgViewSignal loadRewardDlgViewSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        private bool isPromotionShownOnStart;
        private Stack<string> rewards;

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Init);
            promotionCycleOverSignal.AddListener(SetPomotionFlag);
            inboxAddMessagesSignal.AddListener(LoadDailyRewards);
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
            if (isPromotionShownOnStart && (navigatorModel.currentViewId == NavigatorViewId.LOBBY ||
                                            navigatorModel.currentViewId == NavigatorViewId.RATE_APP_DLG))
            {
                SetupRewards();
                LoadDailyReward();
            }
        }

        private void LoadDailyReward()
        {
            if (rewards.Count == 0)
            {
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
    }
}
