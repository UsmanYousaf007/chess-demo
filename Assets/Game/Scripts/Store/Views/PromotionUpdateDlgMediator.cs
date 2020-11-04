using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

public class PromotionUpdateDlgMediator : Mediator
{
    // View injection
    [Inject] public PromotionUpdateDlgView view { get; set; }

    // Services
    [Inject] public IAnalyticsService analyticsService { get; set; }
    [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

    //Models
    [Inject] public IAppInfoModel appInfoModel { get; set; }
    [Inject] public IPreferencesModel preferencesModel { get; set; }

    private IPromise<AdsResult> promise;
    private string screenContext;

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDailogue);
        view.purchaseSignal.AddListener(OnPurchase);
    }

    [ListensTo(typeof(ShowPromotionUpdateDlgSignal))]
    public void OnShowView(PromotionVO promotionVO)
    {
        view.SetView(promotionVO);
        view.Show();
    }

    [ListensTo(typeof(ClosePromotionUpdateDlgSignal))]
    public void OnCloseDaiglogueSignal()
    {
        OnCloseDailogue();
    }

    private void OnCloseDailogue()
    {
        view.Hide();
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

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        if (view.IsVisible())
        {
            OnCloseDailogue();
        }
    }
}
