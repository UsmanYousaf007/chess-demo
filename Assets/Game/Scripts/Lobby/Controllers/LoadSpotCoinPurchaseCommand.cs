using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class LoadSpotCoinPurchaseCommand : Command
    {
        // Command Params
        [Inject] public long betValue { get; set; }

        //Models
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateSpotCoinsPurchaseDlgSignal updateSpotCoinsPurchaseDlgSignal { get; set; }
        [Inject] public UpdateSpotCoinsWatchAdDlgSignal updateSpotCoinsWatchAdDlgSignal { get; set; }

        public override void Execute()
        {
            if (betValue <= settingsModel.bettingIncrements[0])
            {
                updateSpotCoinsWatchAdDlgSignal.Dispatch(betValue, storeSettingsModel.items["CoinPack1"], AdPlacements.Rewarded_coins_purchase);
            }
            else
            {
                var coinPacks = storeSettingsModel.lists.ContainsKey(GSBackendKeys.ShopItem.COINS_SHOP_TAG) ?
                    storeSettingsModel.lists[GSBackendKeys.ShopItem.COINS_SHOP_TAG] : null;

                if (coinPacks != null)
                {
                    coinPacks.Sort((x, y) => x.currency4Payout.CompareTo(y.currency4Payout));

                    var availablePacks = new List<string>();

                    foreach (var pack in coinPacks)
                    {
                        if (pack.currency4Payout >= betValue - playerModel.coins)
                        {
                            availablePacks.Add(pack.key);
                        }
                    }

                    var dynamicContent = new DynamicSpotPurchaseBundle();
                    dynamicContent.leftPackShortCode = availablePacks[0];
                    dynamicContent.rightPackShortCode = availablePacks[1];
                    dynamicContent.dynamicBundleShortCode = playerModel.dynamicBundleToDisplay;

                    var bundles = storeSettingsModel.lists.ContainsKey(GSBackendKeys.ShopItem.SPECIALPACK_SHOP_TAG) ?
                        storeSettingsModel.lists[GSBackendKeys.ShopItem.SPECIALPACK_SHOP_TAG] : null;

                    if (bundles != null)
                    {
                        bundles.Sort((x, y) => x.currency4Cost.CompareTo(y.currency4Cost));

                        foreach (var bundle in bundles)
                        {
                            if (bundle.currency4Cost >= betValue - playerModel.coins)
                            {
                                if (bundle.currency1Cost > storeSettingsModel.items[dynamicContent.dynamicBundleShortCode].currency1Cost)
                                {
                                    dynamicContent.dynamicBundleShortCode = bundle.key;
                                }

                                break;
                            }
                        }
                    }

                    updateSpotCoinsPurchaseDlgSignal.Dispatch(betValue, availablePacks, dynamicContent);
                }
            }

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_COIN_PURCHASE);
        }
    }
}
