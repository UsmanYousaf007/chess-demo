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
        view.Init();
        view.setSkinSignal.AddListener(OnSetSkin);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        view.OnVirtualGoodsAvailable();
    }

    public void OnSetSkin()
    {
        setSkinSignal.Dispatch(view.key);
    }

    [ListensTo(typeof(SkinUpdatedSignal))]
    public void OnSkinChanged()
    {
        view.SetOwnedState();
    }
}
