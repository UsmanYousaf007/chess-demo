using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

public class SkinItemMediator : Mediator
{
    // View injection
    [Inject] public SkinItemView view { get; set; }

    //Dispatch Signals
    [Inject] public SetSkinSignal setSkinSignal { get; set; }
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

    //Services
    [Inject] public IAnalyticsService analyticsService { get; set; }

    //Models
    [Inject] public IPreferencesModel preferencesModel { get; set; }

    public override void OnRegister()
    {
        view.setSkinSignal.AddListener(OnSetSkin);
        view.unlockItemSignal.AddListener(OnUnlockItem);
        view.notEnoughCurrencyToUnlockSignal.AddListener(OnNotEnoughCurrency);
    }

    private void OnSetSkin(string key)
    {
        setSkinSignal.Dispatch(key);
    }

    private void OnUnlockItem(string shortCode)
    {
        purchaseStoreItemSignal.Dispatch(shortCode, true);
    }

    private void OnNotEnoughCurrency()
    {
        SpotPurchaseMediator.analyticsContext = "theme";
        navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        view.UpdateView();
    }

    [ListensTo(typeof(RewardUnlockedSignal))]
    public void OnRewardClaimed(string key, int quantity)
    {
        view.UpdateView();
    }

    [ListensTo(typeof(SkinUpdatedSignal))]
    public void OnSkinChanged()
    {
        view.SetOwnedState();
    }

    [ListensTo(typeof(UpdatePlayerInventorySignal))]
    public void OnInventoryUpdated(PlayerInventoryVO inventory)
    {
        view.UpdateView();
    }

    [ListensTo(typeof(PurchaseStoreItemResultSignal))]
    public void OnItemPurchased(StoreItem item, PurchaseResult result)
    {
        if (result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(view.Key))
        {
            view.PlayAnimation();
            analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.theme);
            analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "theme_unlocked", item.displayName.ToLower().Replace(" ","_"));
        }
    }
}
