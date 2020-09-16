using System;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using UnityEngine;
using System.Collections;

[CLSCompliant(false)]
public class SkinItemView : View
{
    //Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationServicec { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    public Signal<string> setSkinSignal = new Signal<string>();
    public Signal<VirtualGoodsTransactionVO> unlockItemSignal = new Signal<VirtualGoodsTransactionVO>();
    public Signal notEnoughCurrencyToUnlockSignal = new Signal();
    public string Key { get { return key; } }

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

    private void OnEnable()
    {
        base.OnEnable();
        unlockedAnimation.gameObject.SetActive(false);
    }

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
    }

    public void PlayAnimation()
    {
        unlockedAnimation.gameObject.SetActive(true);
        audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
        StartCoroutine(StopAnimation());
    }

    private void OnUnlockClicked()
    {
        audioService.PlayStandardClick();

        var vo = new VirtualGoodsTransactionVO();
        vo.buyItemShortCode = key;
        vo.buyQuantity = 1;

        if (haveEnoughItemsToUnlock)
        {
            vo.consumeItemShortCode = unlockItemKey;
            vo.consumeQuantity = 1;
            unlockItemSignal.Dispatch(vo);
        }
        else if (haveEnoughGemsToUnlock)
        {
            vo.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
            vo.consumeQuantity = unlockItem.currency3Cost;
            unlockItemSignal.Dispatch(vo);
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
