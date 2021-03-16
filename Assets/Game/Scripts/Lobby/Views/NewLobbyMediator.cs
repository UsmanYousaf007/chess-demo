/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

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
            view.RemovePromotion(item.key);

            if (item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG))
            {
                view.LogSubscriptionBannerPurchasedAnalytics(item.key);
            }
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnCoinsPurchase(PlayerInventoryVO inventoryVO)
        {
            view.RemovePromotion(LobbyPromotionKeys.COINS_BANNER);
        }
    }
}
