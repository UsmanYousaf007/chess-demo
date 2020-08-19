/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Core;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class VirtualGoodsTransactionCommand : Command
    {
        // Command Params
        [Inject] public VirtualGoodsTransactionVO vo { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        // Dispatch Signals
        [Inject] public UpdatePlayerInventorySignal updatePlayerInventorySignal { get; set; }
        [Inject] public VirtualGoodBoughtSignal virtualGoodBoughtSignal { get; set; }

        public override void Execute()
        {
            var jsonData = new GSRequestData();

            if (!string.IsNullOrEmpty(vo.buyItemShortCode))
            {
                jsonData.AddString("buyShortCode", vo.buyItemShortCode);
                jsonData.AddNumber("buyQuantity", vo.buyQuantity);
            }

            if (!string.IsNullOrEmpty(vo.consumeItemShortCode))
            {
                jsonData.AddString("consumeShortCode", vo.consumeItemShortCode);
                jsonData.AddNumber("consumeQuantity", vo.consumeQuantity);
            }

            if (jsonData.ContainsKey("buyShortCode") || jsonData.ContainsKey("consumeShortCode"))
            {
                Retain();
                backendService.VirtualGoodsTransaction(jsonData).Then(OnVirtualGoodsTransaction);
            }
        }

        private void OnVirtualGoodsTransaction(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());

                if (!string.IsNullOrEmpty(vo.buyItemShortCode))
                {
                    virtualGoodBoughtSignal.Dispatch(vo.buyItemShortCode);
                }
            }

            Release();
        }
    }
}
