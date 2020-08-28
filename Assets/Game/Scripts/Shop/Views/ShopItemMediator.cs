using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
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

            if (view.isActiveAndEnabled && view.shortCode.Equals(item.key))
            {
                if (view.checkOwned)
                {
                    iTween.PunchScale(view.owned.gameObject, iTween.Hash("amount", new Vector3(0.3f, 0.3f, 0f), "time", 3f));
                }

                if (!view.isSpot)
                {
                    var context = item.displayName.Replace(' ', '_').ToLower();
                    analyticsService.Event(AnalyticsEventId.shop_purchase, AnalyticsParameter.context, context);

                    if (view.isBundle && item.bundledItems != null)
                    {
                        foreach (var bItem in item.bundledItems)
                        {
                            analyticsService.ResourceEvent(GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(bItem.Key).ToString(), bItem.Value, "shop", context);
                        }
                    }

                    if (item.currency3Payout > 0)
                    {
                        analyticsService.ResourceEvent(GAResourceFlowType.Source, "gems", item.currency3Payout, "shop", context);
                    }
                }
            }
        }
    }
}
