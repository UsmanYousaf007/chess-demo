﻿using System;
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

    public Signal<string> setSkinSignal = new Signal<string>();

    private string key;

    public Image thumbnail;
    public Image icon;
    public Text displayName;
    public Image unlock;
    public Image tick;
    public Button button;

    private StoreItem item;
    private StoreThumbsContainer thumbsContainer;
    private StoreIconsContainer iconsContainer;
    private bool isPremium;

    public void Init(string key)
    {
        this.key = key;
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
        isPremium = playerModel.HasSubscription() || playerModel.OwnsVGood(key);

        SetOwnedState();

        thumbnail.sprite = thumbsContainer.GetSprite(key);
        icon.sprite = iconsContainer.GetSprite(key);
        displayName.text = item.displayName;
    }

    private void OnButtonClicked()
    {
        if (isPremium)
        {
            setSkinSignal.Dispatch(key);
        }
        else
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
        }
    }

    public void SetOwnedState()
    {
        unlock.gameObject.SetActive(!isPremium);
        tick.gameObject.SetActive(playerModel.activeSkinId == key);
    }
}
