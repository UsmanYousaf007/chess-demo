using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

public class SubscriptionTierMediator : Mediator
{
    // View injection
    [Inject] public SubscriptionTierView view { get; set; }

    // Dispatch Signals
    [Inject] public SelectTierSignal selectTierSignal { get; set; }

    //Services
    [Inject] public IAudioService audioService { get; set; }

    public override void OnRegister()
    {
        view.InitOnce();
        view.selectTierClicked.AddListener(OnSelectTier);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        if (isAvailable)
        {
            view.Init();
        }
    }

    private void OnSelectTier()
    {
        audioService.PlayStandardClick();
        selectTierSignal.Dispatch(view.key);
    }

    [ListensTo(typeof(SelectTierSignal))]
    public void OnTierSelected(string key)
    {
        view.SelectTier(key.Equals(view.key));
    }
}
