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
    public Signal<string> unlockItemSignal = new Signal<string>();
    public Signal notEnoughCurrencyToUnlockSignal = new Signal();
    public string Key { get { return key; } }
    public int Price { get { return item.currency3Cost; } }

    public Image thumbnail;
    public Image icon;
    public Text displayName;
    public Button unlockBtn;
    public Text price;
    public Image tick;
    public Button button;
    public Text owned;
    public ParticleSystem unlockedAnimation;

    private string key;
    private StoreItem item;
    private static StoreIconsContainer iconsContainer;
    private static StoreThumbsContainer thumbsContainer;
    private bool isUnlocked;
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
        unlockBtn.gameObject.SetActive(!isUnlocked);
        owned.gameObject.SetActive(isUnlocked);
        tick.gameObject.SetActive(playerModel.activeSkinId == key);
        haveEnoughGemsToUnlock = playerModel.gems >= item.currency3Cost;
        price.text = item.currency3Cost.ToString();
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

        if (haveEnoughGemsToUnlock)
        {
            unlockItemSignal.Dispatch(key);
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
