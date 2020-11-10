using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

public class PromotionChessCourseBundleDlgMediator : Mediator
{
    // View injection
    [Inject] public PromotionChessCourseBundleDlgView view { get; set; }

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
        view.closeDailogueSignal.AddListener(OnCloseDialogue);
        view.purchaseSignal.AddListener(OnPurchase);
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_CHESS_COURSE_DLG)
        {
            view.Show();
        }
    }

    [ListensTo(typeof(ClosePromotionUpdateDlgSignal))]
    public void OnCloseDialogueSignal()
    {
        OnCloseDialogue();
    }

    private void OnCloseDialogue()
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
            OnCloseDialogue();
        }
    }
}
