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
        private Dictionary<string, InboxMessage> rewards = new Dictionary<string, InboxMessage>();

        // Listen to signals
        [Inject] public PromotionCycleOverSignal promotionCycleOverSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowFadeBlockerSignal showFadeBlockerSignal { get; set; }
        [Inject] public DailyRewardsCycleOverSignal dailyRewardsOverSignal { get; set; }
        [Inject] public LoadRewardDlgViewSignal loadRewardDlgViewSignal { get; set; }
        [Inject] public InboxAddMessagesSignal inboxAddMessagesSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        private bool dailyRewardShown;
        private bool isPromotionShownOnStart;
        private bool callLock;

        [PostConstruct]
        public void PostConstruct()
        {
            Init();
            promotionCycleOverSignal.AddListener(SetPomotionFlag);
            inboxAddMessagesSignal.AddListener(LoadDailyRewards);
        }

        private void Init()
        {
            isPromotionShownOnStart = false;
            dailyRewardShown = false;
        }

        private void SetPomotionFlag()
        {
            isPromotionShownOnStart = true;
            LoadDailyRewards();
        }

        public void LoadDailyRewards()
        {
            if (navigatorModel.currentViewId == NavigatorViewId.LOBBY && isPromotionShownOnStart)
            {
                SetupRewards();
                LoadDailyReward();
            }
        }

        private void LoadDailyReward()
        {
            if (callLock)
                return;

            callLock = true;

            if (!dailyRewardShown)
            {
                SelectAndDispatchReward();
            }
            else
            {
                OnRewardsCycleOver();
            }
        }

        private void ShowDailyReward(string key, Signal onCloseSignal)
        {
            loadRewardDlgViewSignal.Dispatch(key, onCloseSignal);
        }

        private void SelectAndDispatchReward()
        {
            Signal onCloseSignal = new Signal();
            onCloseSignal.AddListener(LoadDailyReward);

            if (rewards.Count == 0)
            {
                dailyRewardShown = true;
                LoadDailyReward();
                return;
            }

            List<string> keyList = new List<string>(rewards.Keys);
            ShowDailyReward(keyList[0], onCloseSignal);
            rewards.Remove(keyList[0]);
        }

        private void SetupRewards()
        {
            foreach (KeyValuePair<string, InboxMessage> obj in inboxModel.items)
            {
                if (rewards.ContainsKey(obj.Key))
                {
                    rewards.Remove(obj.Key);
                }

                InboxMessage msg = obj.Value;
                if (TimeUtil.unixTimestampMilliseconds >= msg.startTime)
                {
                    rewards.Add(obj.Key, obj.Value);
                }
            }
        }

        private void OnRewardsCycleOver()
        {
            callLock = false;
            dailyRewardShown = false;
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
        }
    }
}
