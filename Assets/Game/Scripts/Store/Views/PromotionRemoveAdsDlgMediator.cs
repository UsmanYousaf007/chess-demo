using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

[System.CLSCompliant(false)]
public class PromotionRemoveAdsDlgMediator : Mediator
{
    // View injection
    [Inject] public PromotionRemoveAdsDlgView view { get; set; }

    // Services
    [Inject] public IPromotionsService promotionsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

    //Models
    [Inject] public IAppInfoModel appInfoModel { get; set; }
    [Inject] public IPreferencesModel preferencesModel { get; set; }

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDialogue);
        view.purchaseSignal.AddListener(OnPurchase);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        if (isAvailable)
        {
            view.Init();
        }
        view.SetupPurchaseButton(isAvailable);
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_DLG)
        {
            view.isOnSale = false;
            view.Show();
        }
        else if (viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG)
        {
            view.isOnSale = true;
            view.Show();
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_DLG || viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG)
        {
            view.Hide();
        }
    }

    private void OnCloseDialogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        promotionsService.LoadPromotion();
    }

    private void OnPurchase(string shortCode)
    {
        purchaseStoreItemSignal.Dispatch(shortCode, true);
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
