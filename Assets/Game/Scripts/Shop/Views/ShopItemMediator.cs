using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ShopItemMediator : Mediator
    {
        //View injection
        [Inject] public ShopItemView view { get; set; }

        //Dispatch Signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.buyButtonSignal.AddListener(OnPurchaseSignal);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }

        private void OnPurchaseSignal(string shortCode)
        {
            purchaseStoreItemSignal.Dispatch(shortCode, true);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            view.SetOwnedStatus();

            if (view.isActiveAndEnabled && (view.shortCode.Equals(item.key) || (view.canGoOnSale && view.saleShortCode.Equals(item.key))))
            {
                if (view.checkOwned)
                {
                    iTween.PunchScale(view.owned.gameObject, iTween.Hash("amount", new Vector3(0.3f, 0.3f, 0f), "time", 3f));
                }

                if (!view.isSpot)
                {
                    var context = item.displayName.Replace(' ', '_').ToLower();
                    analyticsService.Event(AnalyticsEventId.shop_purchase, AnalyticsParameter.context, context);

                    if (view.isBundle)
                    {
                        analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "shop", $"{context}_gems");
                        analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, (int)item.currency4Cost, "shop", $"{context}_coins");
                    }

                    if (item.currency3Payout > 0)
                    {
                        analyticsService.ResourceEvent(GAResourceFlowType.Source, "gems", item.currency3Payout, "shop", context);
                    }
                }
            }
        }

        [ListensTo(typeof(ResetSubscirptionStatusSignal))]
        public void OnResetSubcriptionStatus()
        {
            view.SetOwnedStatus();
        }

        [ListensTo(typeof(ActivePromotionSaleSingal))]
        public void OnShowSale(string key)
        {
            view.SetupSale(key);
        }
    }
}
