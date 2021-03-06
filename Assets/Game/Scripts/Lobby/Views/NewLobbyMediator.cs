/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class NewLobbyMediator : Mediator
    {
        // View injection
        [Inject] public NewLobbyView view { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        //Dispatch Signals
        [Inject] public LoadStatsSignal loadStatsSignal { get; set; }
        [Inject] public StartLobbyChampionshipTimerSignal startLobbyChampionshipTimerSignal { get; set; }

        //Models
        [Inject] public INavigatorModel navigatorModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Show();
                startLobbyChampionshipTimerSignal.Dispatch();
                analyticsService.ScreenVisit(AnalyticsScreen.lobby, facebookService.isLoggedIn());
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(PlayerProfilePicTappedSignal))]
        public void OnPlayerProfileButtonTapped()
        {
            if (gameObject.activeSelf)
            {
                loadStatsSignal.Dispatch();
            }
        }

        [ListensTo(typeof(ShowPromotionSignal))]
        public void OnShowPromotion(PromotionVO vo)
        {
            view.ShowPromotion(vo);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.SetPriceOfIAPBanner(isAvailable);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnRemoveLobbyPromotion(StoreItem item)
        {
            if (navigatorModel.currentViewId == NavigatorViewId.LOBBY && item.kind.Equals(GSBackendKeys.ShopItem.SPECIALPACK_SHOP_TAG))
            {
                var context = item.displayName.Replace(' ', '_').ToLower();
                analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "promotion", $"banner_{context}_gems");
                analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, (int)item.currency4Cost, "promotion", $"banner_{context}_coins");
                view.LogBundleBannerPurchasedAnalytics();
            }
            else if (view.IsVisible())
            {
                if (item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG))
                {
                    view.LogSubscriptionBannerPurchasedAnalytics(item.key);
                }
                else
                {
                    view.LogBannerPurchasedAnalytics();
                }
            }

            view.RemovePromotion();
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnCoinsPurchase(PlayerInventoryVO inventoryVO)
        {
            view.LogBannerPurchasedAnalytics();
            view.RemovePromotion();
        }
    }
}
