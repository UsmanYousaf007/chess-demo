using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

public class SkinItemMediator : Mediator
{
    // View injection
    [Inject] public SkinItemView view { get; set; }

    //Dispatch Signals
    [Inject] public SetSkinSignal setSkinSignal { get; set; }

    public override void OnRegister()
    {
        view.setSkinSignal.AddListener(OnSetSkin);
    }

    public void OnSetSkin()
    {
        setSkinSignal.Dispatch(view.key);
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
}
