/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class ConsumeVirtualGoodCommand : Command
    {
        // Command Params
        [Inject] public string key { get; set; }
        [Inject] public int quantity { get; set; }

        // Dispatch Signals
        [Inject] public UpdatePlayerConsumablesSignal updatePlayerConsumableDisplaySignal { get; set; }

        // Models

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        string itemKey;
        int itemQuantity;

        public override void Execute()
        {
            itemKey = key;
            itemQuantity = quantity;

            Retain();
            backendService.ConsumeVirtualGood(quantity, key).Then(OnConsumeStoreItem);
        }

        private void OnConsumeStoreItem(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                updatePlayerConsumableDisplaySignal.Dispatch();
                analyticsService.VirtualGoodConsumed(itemKey, itemQuantity);
            }

            Release();
        }
    }
}