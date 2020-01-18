using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

public class PromotionDlgMediator : Mediator
{
    // View injection
    [Inject] public PromotionDlgView view { get; set; }

    // Services
    [Inject] public IAnalyticsService analyticsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

    public override void OnRegister()
    {
        view.closeDailogueSignal.AddListener(OnCloseDailogue);
        view.restorePurchasesSignal.AddListener(OnRestorePurchases);
        view.purchaseSignal.AddListener(OnPurchase);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        if (isAvailable)
        {
            view.Init();
        }
    }

    [ListensTo(typeof(ShowPromotionDlgSignal))]
    public void OnShowView()
    {
        view.Show();
    }

    private void OnCloseDailogue()
    {
        view.Hide();
    }

    private void OnRestorePurchases()
    {
        restorePurchasesSignal.Dispatch();
    }

    private void OnPurchase()
    {
        purchaseStoreItemSignal.Dispatch(view.key, true);
    }

    [ListensTo(typeof(ShowProcessingSignal))]
    public void OnShowProcessingUI(bool show, bool showProcessingUi)
    {
        view.ShowProcessing(show, showProcessingUi);
    }
}
