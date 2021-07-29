using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

[System.CLSCompliant(false)]
public class PromotionBundleMediator : Mediator
{
    // View injection
    [Inject] public PromotionBundleView view { get; set; }

    // Services
    [Inject] public IPromotionsService promotionsService { get; set; }
    [Inject] public IAnalyticsService analyticsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
    [Inject] public BuyDynamicBundleClickedSignal buyDynamicBundleClickedSignal { get; set; }

    //Models
    [Inject] public IPreferencesModel preferencesModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    public override void OnRegister()
    {
        view.InitOnce();
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

    private void OnPurchase()
    {
        buyDynamicBundleClickedSignal.Dispatch();
        purchaseStoreItemSignal.Dispatch(view.key, true);
    }

    [ListensTo(typeof(UpdatePromotionBundleSignal))]
    public void OnUpdateBundle()
    {
        if (view.key == playerModel.dynamicBundleToDisplay)
        {
            view.Show();
        }
        else
        {
            view.Hide();
        }
    }
}
