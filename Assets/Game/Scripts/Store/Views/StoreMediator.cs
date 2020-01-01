/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public partial class StoreMediator : Mediator
    {
        // View injection
        [Inject] public StoreView view { get; set; }

        // Dispatch signals
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateStoreBuyDlgSignal updateStoreBuyDlgSignal { get; set; }
        [Inject] public UpdateStoreNotEnoughBucksDlgSignal updateStoreNotEnoughBucksDlgSignal { get; set; }
        [Inject] public SetSkinSignal setSkinSignal { get; set; }
        [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }


        public override void OnRegister()
        {
            view.Init();
            view.InitInfo();
            view.storeItemClickedSignal.AddListener(OnStoreItemClicked);

            OnRegisterBuy();
            OnRegisterNotEnoughBucks();
            OnRegisterPurchasedDlg();
        }

        public override void OnRemove()
        {
            view.storeItemClickedSignal.RemoveAllListeners();

            OnRemoveBuy();
            OnRemoveNotEnoughBucks();
            OnRemovePurchasedDlg();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.STORE)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.shop);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.STORE)
            {
                if (view.HasSkinChanged())
                {
                    // Dispatch save skin signal
                    savePlayerInventorySignal.Dispatch();
                }

                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateStoreSignal))]
        public void OnUpdateStore(StoreVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(ShowStoreTabSignal))]
        public void OnShowTab(StoreView.StoreTabs tab)
        {
            if (view.IsVisible())
            {
                view.ShowTab(tab);
            }
        }

        [ListensTo(typeof(UpdatePurchasedBundleStoreItemSignal))]
        public void OnUpdatePurchasedBundleStoreItem(StoreVO vo, StoreItem item)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnPurchaseResult(StoreItem item, PurchaseResult result)
        {
            if (!view.IsVisible())
            {
                return;
            }

            if (result == PurchaseResult.ALREADY_OWNED)
            {
                if (item.kind == GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
                {
                    view.currentSkinItemId = item.key;
                    view.UpdateItemThumbnail(item.key);
                    setSkinSignal.Dispatch(item.key);
                }
            }
            else if (result == PurchaseResult.NOT_ENOUGH_BUCKS)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_NOT_ENOUGH_DLG);
            }
            else if (result == PurchaseResult.PERMISSION_TO_PURCHASE)
            {
                updateStoreBuyDlgSignal.Dispatch(item);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_BUY_DLG);
            }
            else if (result == PurchaseResult.PURCHASE_SUCCESS)
            {
                view.UpdatePurchasedDlg(item);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PURCHASED_DLG);

                if (item.kind == GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
                {
                    view.currentSkinItemId = item.key;
                    view.UpdateItemThumbnail(item.key);
                    setSkinSignal.Dispatch(item.key);
                }

                analyticsService.Event(AnalyticsEventId.store_purchase_complete, AnalyticsParameter.item_id, item.key);
            }
        }

        private void OnStoreItemClicked(StoreItem item)
        {
            // Purchase item after confirmation. No confirmation for remote store items
            purchaseStoreItemSignal.Dispatch(item.key, true);
        }

        private void OnBackButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }

        [ListensTo(typeof(ShowProcessingSignal))]
        public void OnShowProcessingUI(bool show, bool showProcessingUi)
        {
            view.ShowProcessing(show, showProcessingUi);
        }
    }
}
