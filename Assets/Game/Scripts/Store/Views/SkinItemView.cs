using System;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using UnityEngine;
using System.Collections;

public class SkinItemView : View
{
    //Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationServicec { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    public Signal<string> setSkinSignal = new Signal<string>();
    public Signal<string, string, int> unlockItemSignal = new Signal<string, string, int>();
    public Signal notEnoughCurrencyToUnlockSignal = new Signal();

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
    public ParticleSystem unlockedAnimation;

    private string key;
    private StoreItem item;
    private StoreItem unlockItem;
    private static StoreIconsContainer iconsContainer;
    private static StoreThumbsContainer thumbsContainer;
    private bool isUnlocked;
    private bool haveEnoughItemsToUnlock;
    private bool haveEnoughGemsToUnlock;
    private int playUnlockAnimation = 0;

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
        unlockBtn.onClick.AddListener(OnUnlockClicked);
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
        unlockItem = storeSettingsModel.items[unlockItemKey];
        unlockBtn.gameObject.SetActive(!isUnlocked);
        owned.gameObject.SetActive(isUnlocked);
        tick.gameObject.SetActive(playerModel.activeSkinId == key);
        haveEnoughItemsToUnlock = playerModel.GetInventoryItemCount(unlockItemKey) > 0;
        haveEnoughGemsToUnlock = playerModel.gems >= unlockItem.currency3Cost;
        requiredGems.text = unlockItem.currency3Cost.ToString();
        notEnoughUnlockItems.gameObject.SetActive(!haveEnoughItemsToUnlock);
        notEnoughUnlockItems.sprite = haveEnoughGemsToUnlock ? enoughGems : notEnoughGems;
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        if (gameObject.activeInHierarchy && isUnlocked && playUnlockAnimation == 1)
        {
            unlockedAnimation.gameObject.SetActive(true);
            playUnlockAnimation = 2;
            StartCoroutine(StopAnimation());
        }
    }

    private void OnUnlockClicked()
    {
        audioService.PlayStandardClick();

        if (haveEnoughItemsToUnlock)
        {
            playUnlockAnimation = 1;
            unlockItemSignal.Dispatch(key, unlockItemKey, 1);
        }
        else if (haveEnoughGemsToUnlock)
        {
            playUnlockAnimation = 1;
            unlockItemSignal.Dispatch(key, GSBackendKeys.PlayerDetails.GEMS, unlockItem.currency3Cost);
        }
        else
        {
            notEnoughCurrencyToUnlockSignal.Dispatch();
        }
    }

    IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(2.0f);
        unlockedAnimation.gameObject.SetActive(false);
    }
}
