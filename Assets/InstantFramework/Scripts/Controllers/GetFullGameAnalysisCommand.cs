using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using GameSparks.Core;
using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class GetFullGameAnalysisCommand : Command
    {
        //Models
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Dispatch Signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public FullAnalysisBoughtSignal fullAnalysisBoughtSignal { get; set; }

        //Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Listeners
        [Inject] public PurchaseStoreItemResultSignal purchaseStoreItemResultSignal { get; set; }

        public override void Execute()
        {
            var shortCode = GSBackendKeys.ShopItem.FULL_GAME_ANALYSIS;

            if (playerModel.GetInventoryItemCount(shortCode) < rewardsSettingsModel.freeFullGameAnalysis)
            {
                var jsonData = new GSRequestData().AddString("rewardType", GSBackendKeys.ClaimReward.TYPE_FREE_FULL_GAME_ANALYSIS);
                backendService.ClaimReward(jsonData).Then(OnFreeRewardClaimed);
            }
            else
            {
                purchaseStoreItemSignal.Dispatch(shortCode, true);
                purchaseStoreItemResultSignal.AddOnce(OnItemPurchased);
            }
        }

        private void OnFreeRewardClaimed(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                fullAnalysisBoughtSignal.Dispatch();
                analyticsService.Event(AnalyticsEventId.free_game_analysis_used);
            }
        }

        private void OnItemPurchased(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(GSBackendKeys.ShopItem.FULL_GAME_ANALYSIS))
            {
                fullAnalysisBoughtSignal.Dispatch();
                analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.game_analysis);
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "booster_used", AnalyticsContext.game_analysis.ToString());
            }
        }
    }
}
