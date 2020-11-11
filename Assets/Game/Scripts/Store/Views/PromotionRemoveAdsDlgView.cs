using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

[System.CLSCompliantAttribute(false)]
public class PromotionRemoveAdsDlgView : View
{
    public string shortCode;
    public string saleShortCode;
    public Text title;
    public Button closeButton;
    public Text purchaseText;
    public Button purchaseButton;
    public GameObject uiBlocker;
    public GameObject processingUi;

    public GameObject saleObj;
    public Text goAdsFreeText;
    public Text ribbonText;
    public Text orignalPrice;

    public bool isOnSale;
    private StoreItem storeItem;
    private StoreItem saleItem;

    //Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal<string> purchaseSignal = new Signal<string>();

    private StoreIconsContainer iconsContainer;

    public void InitOnce()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        iconsContainer = StoreIconsContainer.Load();
    }

    public void Init()
    {
        var storeItem = storeSettingsModel.items[shortCode];

        if (storeItem == null)
            return;

        title.text = storeItem.displayName;
        purchaseText.text = $"{storeItem.remoteProductCurrencyCode} {storeItem.productPrice} only";

        SetupSalePrice();
    }

    public void UpdateView()
    {
        var key = shortCode;

        if (isOnSale)
            key = saleShortCode;

        var storeItem = storeSettingsModel.items[key];

        if (storeItem == null)
            return;
        title.text = storeItem.displayName;

        if(isOnSale)
        {
            saleObj.SetActive(true);
            goAdsFreeText.enabled = false;
            closeButton.transform.localPosition = new Vector3(closeButton.transform.localPosition.x, -221f, closeButton.transform.localPosition.z);
            title.transform.localPosition = new Vector3(closeButton.transform.localPosition.x, -14f, closeButton.transform.localPosition.z);
            purchaseText.text = $"{saleItem.remoteProductCurrencyCode} {saleItem.remoteProductPrice}";

        }
        else
        {
            saleObj.SetActive(false);
            goAdsFreeText.enabled = true;
            closeButton.transform.localPosition = new Vector3(closeButton.transform.localPosition.x, -103f, closeButton.transform.localPosition.z);
            title.transform.localPosition = new Vector3(closeButton.transform.localPosition.x, 0, closeButton.transform.localPosition.z);
            purchaseText.text = $"{storeItem.remoteProductCurrencyCode} {storeItem.productPrice} only";
        }
    }

    public void Show()
    {
        UpdateView();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetupSalePrice()
    {
        saleItem = storeSettingsModel.items[saleShortCode];

        if (saleItem == null)
        {
            return;
        }

        var discount = 1 - (saleItem.productPrice / storeItem.productPrice);
        orignalPrice.text = $"Limited Time Only! {storeItem.remoteProductCurrencyCode}{storeItem.remoteProductPrice}";
        ribbonText.text = $"{(int)discount * 100}% OFF";
    }

    public void SetupSale(string saleKey)
    {
        if (saleKey.Equals(saleShortCode))
            return;

        isOnSale = true;
        UpdateView();
    }

    private void OnCloseButtonClicked()
    {
        audioService.PlayStandardClick();
        closeDailogueSignal.Dispatch();
    }

    private void OnPurchaseButtonClicked()
    {
        audioService.PlayStandardClick();
        purchaseSignal.Dispatch(isOnSale ? saleShortCode : shortCode);
    }

    public void ShowProcessing(bool show, bool showProcessingUi)
    {
        processingUi.SetActive(showProcessingUi);
        uiBlocker.SetActive(show);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    public void SetupPurchaseButton(bool isAvailable)
    {
        purchaseButton.interactable = isAvailable;
        purchaseText.color = isAvailable ? Colors.WHITE : Colors.DISABLED_WHITE;
    }
}


