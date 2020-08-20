﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;

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

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        //Listeners
        [Inject] public VirtualGoodBoughtSignal virtualGoodBoughtSignal { get; set; }

        public override void Execute()
        {
            if (!adsService.IsRewardedVideoAvailable())
            {
                inventoryVideoResultSignal.Dispatch(InventoryVideoResult.NOT_AVAILABLE, vo.itemKey);
                return;
            }

            Retain();
            adsService.ShowRewardedVideo().Then(OnVideoShown);
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
