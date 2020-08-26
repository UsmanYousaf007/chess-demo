using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

public class SkinItemMediator : Mediator
{
    // View injection
    [Inject] public SkinItemView view { get; set; }

    //Dispatch Signals
    [Inject] public SetSkinSignal setSkinSignal { get; set; }
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }

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

    private void OnUnlockItem(VirtualGoodsTransactionVO vo)
    {
        virtualGoodsTransactionSignal.Dispatch(vo);
    }

    private void OnNotEnoughCurrency()
    {
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

    [ListensTo(typeof(VirtualGoodBoughtSignal))]
    public void OnItemUnlocked(string itemShortCode)
    {
        if (itemShortCode.Equals(view.Key))
        {
            view.PlayAnimation();
        }
    }
}
