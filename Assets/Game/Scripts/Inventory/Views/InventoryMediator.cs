using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class InventoryMediator : Mediator
    {
        //View Injection
        [Inject] public InventoryView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        //DispatchSignals
        [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.applyThemeSignal.AddListener(OnApplyTheme);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.INVENTORY)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.INVENTORY)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }

        private void OnApplyTheme()
        {
            if (view.HasSkinChanged())
            {
                savePlayerInventorySignal.Dispatch("");
                view.originalSkinId = view.playerModel.activeSkinId;
                hAnalyticsService.LogEvent("selection", "menu", "", "theme_change");
            }
        }
    }
}
