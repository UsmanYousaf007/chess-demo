using System;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;

public class SkinItemView : View
{
    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationServicec { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

    public Signal setSkinSignal = new Signal();

    public string key;

    public Image thumbnail;
    public Image icon;
    public Text displayName;
    public Image unlock;
    //public Text owned;
    public Image tick;
    public Button button;

    private StoreItem item;
    private StoreThumbsContainer thumbsContainer;
    private StoreIconsContainer iconsContainer;
    private bool isPremium;

    public void Init()
    {
        thumbsContainer = StoreThumbsContainer.Load();
        iconsContainer = StoreIconsContainer.Load();
        button.onClick.AddListener(OnButtonClicked);
        UpdateView();
    }

    public void UpdateView()
    {
        if (metaDataModel.store == null ||
           !metaDataModel.store.items.ContainsKey(key))
        {
            return;
        }

        item = metaDataModel.store.items[key];
        isPremium = playerModel.HasSubscription() || item.skinIndex < playerModel.rewardSkinIndex;

        SetOwnedState();

        thumbnail.sprite = thumbsContainer.GetSprite(key);
        icon.sprite = iconsContainer.GetSprite(key);
        displayName.text = item.displayName;
        //owned.text = localizationServicec.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);
    }

    private void OnButtonClicked()
    {
        if (isPremium)
        {
            setSkinSignal.Dispatch();
        }
        else
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
        }
    }

    public void SetOwnedState()
    {
        unlock.gameObject.SetActive(!isPremium);
        //owned.gameObject.SetActive(isPremium);
        tick.gameObject.SetActive(playerModel.activeSkinId == key);
    }
}
