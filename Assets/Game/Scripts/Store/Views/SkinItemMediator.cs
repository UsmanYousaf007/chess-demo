using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

public class SkinItemMediator : Mediator
{
    // View injection
    [Inject] public SkinItemView view { get; set; }

    //Dispatch Signals
    [Inject] public SetSkinSignal setSkinSignal { get; set; }
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }
    [Inject] public LoadSpotInventorySignal loadSpotInventorySignal { get; set; }

    //Services
    [Inject] public IAnalyticsService analyticsService { get; set; }

    private VirtualGoodsTransactionVO transactionVO;

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
        transactionVO = vo;
        virtualGoodsTransactionSignal.Dispatch(vo);
    }

    private void OnNotEnoughCurrency(VirtualGoodsTransactionVO vo)
    {
        transactionVO = vo;
        //SpotPurchaseMediator.customContext = "themes";
        //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        var spotInventoryParams = new LoadSpotInventoryParams();
        spotInventoryParams.itemShortCode = view.unlockItemKey;
        spotInventoryParams.itemToUnclockShortCode = view.Key;
        loadSpotInventorySignal.Dispatch(spotInventoryParams);
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
            analyticsService.ResourceEvent(GAResourceFlowType.Sink, CollectionsUtil.GetContextFromString(transactionVO.consumeItemShortCode).ToString(), transactionVO.consumeQuantity, "theme_unlocked", itemShortCode);
        }
    }

    [ListensTo(typeof(SpotInventoryPurchaseCompletedSignal))]
    public void OnSpotInventoryPurchaseCompleted(string key, string purchaseType)
    {
        if (key.Equals(view.Key))
        {
            virtualGoodsTransactionSignal.Dispatch(transactionVO);
        }
    }
}
