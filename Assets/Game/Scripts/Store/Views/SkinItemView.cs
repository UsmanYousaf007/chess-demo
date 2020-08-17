using System;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using UnityEngine;

public class SkinItemView : View
{
    //Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationServicec { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

    public Signal<string> setSkinSignal = new Signal<string>();

    private string key;

    public Image thumbnail;
    public Image icon;
    public Text displayName;
    public Button unlockBtn;
    public Text unlockText;
    public Image notEnoughUnlockItems;
    public Text requiredGems;
    public string unlockItemKey;
    public Sprite enoughGems;
    public Sprite notEnoughGems;
    public Image tick;
    public Button button;
    public Text owned;

    private StoreItem item;
    private static StoreIconsContainer iconsContainer;
    private static StoreThumbsContainer thumbsContainer;
    private bool isUnlocked;
    private bool haveEnoughItemsToUnlock;
    private bool haveEnoughGemsToUnlock;

    public void Init(string key)
    {
        this.key = key;

        if (iconsContainer == null)
        {
            iconsContainer = StoreIconsContainer.Load();
        }

        if (thumbsContainer == null)
        {
            thumbsContainer = StoreThumbsContainer.Load();
        }

        button.onClick.AddListener(OnButtonClicked);
        unlockText.text = localizationServicec.Get(LocalizationKey.INVENTORY_ITEM_UNLOCK);
        owned.text = localizationServicec.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);
        UpdateView();
    }

    public void UpdateView()
    {
        if (storeSettingsModel == null ||
           !storeSettingsModel.items.ContainsKey(key))
        {
            return;
        }

        item = storeSettingsModel.items[key];
        isUnlocked = playerModel.HasSubscription() || playerModel.OwnsVGood(key) || playerModel.OwnsVGood(GSBackendKeys.ShopItem.ALL_THEMES_PACK);

        SetOwnedState();

        thumbnail.sprite = thumbsContainer.GetSprite(key);
        icon.sprite = iconsContainer.GetSprite(key);
        displayName.text = item.displayName;
    }

    private void OnButtonClicked()
    {
        audioService.PlayStandardClick();
        if (isUnlocked)
        {
            setSkinSignal.Dispatch(key);
        }
        //else
        //{
        //    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
        //}
    }

    public void SetOwnedState()
    {
        var unlockItem = storeSettingsModel.items[unlockItemKey];
        unlockBtn.gameObject.SetActive(!isUnlocked);
        owned.gameObject.SetActive(isUnlocked);
        tick.gameObject.SetActive(playerModel.activeSkinId == key);
        haveEnoughItemsToUnlock = playerModel.GetInventoryItemCount(unlockItemKey) > 0;
        haveEnoughGemsToUnlock = playerModel.gems >= unlockItem.currency3Cost;
        requiredGems.text = unlockItem.currency3Cost.ToString();
        notEnoughUnlockItems.gameObject.SetActive(!haveEnoughItemsToUnlock);
        notEnoughUnlockItems.sprite = haveEnoughGemsToUnlock ? enoughGems : notEnoughGems;
    }
}
