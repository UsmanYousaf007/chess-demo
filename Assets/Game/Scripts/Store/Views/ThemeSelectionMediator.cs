using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

public class ThemeSelectionMediator : Mediator
{
    // View injection
    [Inject] public ThemeSelectionView view { get; set; }

    // Services
    [Inject] public IAnalyticsService analyticsService { get; set; }
    [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }

    public override void OnRegister()
    {
        view.InitOnce();
        view.applyThemeSignal.AddListener(OnApplyTheme);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        if (!isAvailable)
        {
            view.Init();
        }
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.THEME_SELECTION)
        {
            view.Show();
            analyticsService.ScreenVisit(AnalyticsScreen.theme_selection_dlg);
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.THEME_SELECTION)
        {
            view.Hide();
        }
    }

    private void OnCloseDailogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
    }

    private void OnApplyTheme()
    {
        if (view.HasSkinChanged())
        {
            savePlayerInventorySignal.Dispatch();
            hAnalyticsService.LogEvent("selection", "menu", "", "theme_change");
        }

        OnCloseDailogue();
    }
}
