using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

[System.CLSCompliantAttribute(false)]
public class PromotionWelcomeBundleDlgView : View
{

    [System.Serializable]
    public class ShopPayout
    {
        public Image icon;
        public Text count;
    }

    public string key;
    public Text title;
    public Button closeButton;
    public Text purchaseText;
    public Button purchaseButton;
    public GameObject loading;

    [Header("Items")]
    public ShopPayout currencyPayout;
    public ShopPayout currency2Payout;
    public ShopPayout[] payouts;

    //Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal purchaseSignal = new Signal();

    private StoreIconsContainer iconsContainer;

    public void InitOnce()
    {
        UIDlgManager.Setup(gameObject);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        iconsContainer = StoreIconsContainer.Load();
    }

    public void Init()
    {
        var storeItem = storeSettingsModel.items[key];

        if (storeItem == null)
            return;

        title.text = storeItem.displayName;
        purchaseText.text = storeItem.remoteProductPrice;

        //if (storeItem.bundledItems != null)
        //{
        if (storeItem.currency3Cost > 0)
        {
            //currencyPayout.icon.sprite = iconsContainer.GetSprite("Gem");
            currencyPayout.count.text = storeItem.currency3Cost.ToString();
        }

        if (storeItem.currency4Cost > 0)
        {
            //currencyPayout.icon.sprite = iconsContainer.GetSprite("Gem");
            currency2Payout.count.text = storeItem.currency4Cost.ToString();
        }

        //var i = 0;
        //foreach (var item in storeItem.bundledItems)
        //{
        //    if (i < payouts.Length)
        //    {
        //        //payouts[i].icon.sprite = iconsContainer.GetSprite(item.Key);
        //        payouts[i].count.text = $"{storeSettingsModel.items[item.Key].displayName} x{item.Value}";
        //        i++;
        //    }
        //}
        //}
    }

    public void Show()
    {
        UIDlgManager.Show(gameObject);
    }

    public void Hide()
    {
        UIDlgManager.Hide(gameObject);
    }

    private void OnCloseButtonClicked()
    {
        audioService.PlayStandardClick();
        closeDailogueSignal.Dispatch();
    }

    public bool IsVisible()
    {
        return isActiveAndEnabled;
    }

    public void SetupPurchaseButton(bool isAvailable)
    {
        purchaseButton.interactable = isAvailable;
        purchaseText.enabled = isAvailable;
        loading.SetActive(!isAvailable);
    }

    private void OnPurchaseButtonClicked()
    {
        audioService.PlayStandardClick();
        purchaseSignal.Dispatch();
    }
}


