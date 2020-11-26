/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantFramework
{
    public class DailyRewardsService : IDailyRewardsService
    {
        public bool dailyRewardShown { get; set; }
        private Dictionary<string, InboxMessage> rewards = new Dictionary<string, InboxMessage>();

        // Listen to signals
        [Inject] public PromotionCycleOverSignal promotionCycleOverSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IInboxModel inboxModel { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowFadeBlockerSignal showFadeBlockerSignal { get; set; }
        [Inject] public DailyRewardsCycleOverSignal dailyRewardsOverSignal { get; set; }
        [Inject] public LoadRewardDlgViewSignal loadRewardDlgViewSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }


        [PostConstruct]
        public void PostConstruct()
        {
            promotionCycleOverSignal.AddListener(Reset);
        }

        private void Reset()
        {
            dailyRewardShown = false;
        }

        public void LoadDailyRewards()
        {
            SetupRewards();
            LoadDailyReward();
        }

        public void LoadDailyReward()
        {
            if (!dailyRewardShown)
            {
                SelectAndDispatchReward();
            }
            else
            {
                OnRewardsCycleOver();
            }
        }

        public void ShowDailyReward(string key, Signal onCloseSignal)
        {
            if (TimeUtil.unixTimestampMilliseconds >= inboxModel.items[key].startTime)
            {
                loadRewardDlgViewSignal.Dispatch(key, onCloseSignal);
            }
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
            ShowDailyReward(keyList[0], loadLobbySignal);
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

        public void OnRewardDlgClosed()
        {
            TLUtils.LogUtil.Log("DailyRewardService::OnRewardDlgClosed()");
            loadLobbySignal.Dispatch();
        }

        private void OnRewardsCycleOver()
        {
            dailyRewardShown = false;
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
        }
    }
}
