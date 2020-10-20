/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework
{
    public class ShowInventoryRewardedVideoCommand : Command
    {
        //Parameters
        [Inject] public InventoryVideoVO vo { get; set; }

        //Dispatch Signals
        [Inject] public InventoryVideoResultSignal inventoryVideoResultSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }

        //Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        //Listeners
        [Inject] public VirtualGoodBoughtSignal virtualGoodBoughtSignal { get; set; }

        public override void Execute()
        {
            var itemPointsKey = vo.isPopup ? $"{vo.itemPointsKey}Popup" : vo.itemPointsKey;
            var adContext = CollectionsUtil.GetContextFromString(itemPointsKey);
            var adPlacementId = CollectionsUtil.GetAdPlacementsIdFromString(itemPointsKey);

            playerModel.adContext = adContext;
            analyticsService.Event(AnalyticsEventId.ad_user_requested, adContext);

            if (!adsService.IsRewardedVideoAvailable(adPlacementId))
            {
                inventoryVideoResultSignal.Dispatch(InventoryVideoResult.NOT_AVAILABLE, vo.itemKey);
                return;
            }

            Retain();
            adsService.ShowRewardedVideo(adPlacementId).Then(OnVideoShown);
        }

        private void OnVideoShown(AdsResult result)
        {
            if (result != AdsResult.FINISHED)
            {
                Release();
                return;
            }

            var currentPoints = playerModel.GetInventoryItemCount(vo.itemPointsKey);

            if (currentPoints + 1 == settingsModel.GetInventorySpecialItemsRewardedVideoCost(vo.itemKey))
            {
                var transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.buyItemShortCode = vo.itemKey;
                transactionVO.buyQuantity = 1;

                if (currentPoints > 0)
                {
                    transactionVO.consumeItemShortCode = vo.itemPointsKey;
                    transactionVO.consumeQuantity = playerModel.GetInventoryItemCount(vo.itemPointsKey);
                }

                virtualGoodBoughtSignal.AddOnce(OnItemUnlocked);
                virtualGoodsTransactionSignal.Dispatch(transactionVO);
            }
            else
            {
                var transactionVO = new VirtualGoodsTransactionVO();
                transactionVO.buyItemShortCode = vo.itemPointsKey;
                transactionVO.buyQuantity = 1;
                virtualGoodBoughtSignal.AddOnce(OnPointAdded);
                virtualGoodsTransactionSignal.Dispatch(transactionVO);
            }

            var itemKey = vo.isPopup ? $"{vo.itemKey}Popup" : vo.itemKey;
            analyticsService.Event(AnalyticsEventId.inventory_rewarded_video_watched, CollectionsUtil.GetContextFromString(itemKey));

            preferencesModel.intervalBetweenPregameAds = DateTime.Now;
        }

        private void OnItemUnlocked(string item)
        {
            inventoryVideoResultSignal.Dispatch(InventoryVideoResult.ITEM_UNLOCKED, item);
            Release();
        }

        private void OnPointAdded(string item)
        {
            inventoryVideoResultSignal.Dispatch(InventoryVideoResult.SUCCESS, vo.itemKey);
            Release();
        }
    }
}
