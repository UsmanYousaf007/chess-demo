using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;

[System.CLSCompliantAttribute(false)]
public class PromotionBundleView : View
{

    [System.Serializable]
    public class ShopPayout
    {
        public Image icon;
        public Text count;
    }

    public string key;
    public Button purchaseButton;
    public Text overallPercentageVal;
    public GameObject loading;
    public Text purchaseText;

    [Header("Items")]
    public Text gemsPercentageVal;
    public ShopPayout currencyPayout;
    public ShopPayout currency2Payout;

    //Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal purchaseSignal = new Signal();

    protected StoreItem bundleStoreItem;
    protected decimal totalBundlePrice;

    public virtual void InitOnce()
    {
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
    }

    public  virtual void Init()
    {
        bundleStoreItem = storeSettingsModel.items[key];

        if (bundleStoreItem == null)
            return;

        CalculateDiscount();
        if (bundleStoreItem.currency3Cost > 0)
        {
            currencyPayout.count.text = bundleStoreItem.currency3Cost.ToString("N0"); ;
        }

        if (bundleStoreItem.currency4Cost > 0)
        {
            currency2Payout.count.text = FormatUtil.AbbreviateNumber(bundleStoreItem.currency4Cost, false);
        }

        purchaseText.text = bundleStoreItem.remoteProductPrice;
    }

    protected void CalculateDiscount()
    {
        //Discount on gems
        var storeItem = storeSettingsModel.items["GemPack150"];
        var costPerGem = storeItem.productPrice / storeItem.currency3Payout;
        var priceForGems = bundleStoreItem.currency3Cost * costPerGem;
        var discountOnGems = 1 - (bundleStoreItem.productPrice / priceForGems);
        gemsPercentageVal.text = $"{(int)(discountOnGems * 100)}%";

        //Overall discount
        var coinsStoreItem = storeSettingsModel.items["CoinPack1"];
        var coinsPerGem = coinsStoreItem.currency4Payout / coinsStoreItem.currency3Payout;
        var priceForCoinsInGems = bundleStoreItem.currency4Cost / coinsPerGem;
        var priceForCoins = priceForCoinsInGems * costPerGem;
        totalBundlePrice = priceForGems + priceForCoins;
        var overallDiscount = 1 - (bundleStoreItem.productPrice / totalBundlePrice);
        overallPercentageVal.text = $"{(int)(overallDiscount * 100)}";
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    public void SetupPurchaseButton(bool isAvailable)
    {
        purchaseButton.interactable = isAvailable;
        purchaseText.enabled = isAvailable;
        loading.SetActive(!isAvailable);
    }

    protected void OnPurchaseButtonClicked()
    {
        audioService.PlayStandardClick();
        purchaseSignal.Dispatch();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}