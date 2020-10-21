using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

public class SubscriptionDlgView : View
{
    public string key;
    public Text title;
    public GameObject offerPrefab;
    public Transform offersContainer;
    public Button closeButton;
    public Text termsOfUseText;
    public Button termsOfUseButton;
    public Text restorePurchaseText;
    public Button restorePurchaseButton;
    public Text purchaseText;
    public Button purchaseButton;
    public GameObject uiBlocker;
    public GameObject processingUi;
    public VerticalLayoutGroup offerBg;

    public Button privacyPolicyButton;
    public Text privacyPolicyText;

    //Fetching data
    public GameObject thinking;
    public Image[] radioButtons;

    public SubscriptionTierView[] subscriptionTiers;

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public ISettingsModel settingsModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal restorePurchasesSignal = new Signal();
    public Signal purchaseSignal = new Signal();
    public Signal showTermsSignal = new Signal();

    private StoreIconsContainer iconsContainer;

    public void InitOnce()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        termsOfUseButton.onClick.AddListener(OnTermsOfUseClicked);
        restorePurchaseButton.onClick.AddListener(OnRestorePurchaseClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyClicked);
        iconsContainer = StoreIconsContainer.Load();
    }

    public void Init()
    {
        title.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TITLE);
        //restorePurchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE);
        //termsOfUseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TERMS_OF_USE);
        purchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PURCHASE_BUTTON);

        var storeItem = storeSettingsModel.items[key];

        if (storeItem == null)
            return;

        SetupDefaultSubscriptionTier();

        // Fill only once
        if (offersContainer.childCount == 0)
        {
            var offers = storeItem.description.Split(',');
            foreach (var offer in offers)
            {
                var offerObj = Instantiate(offerPrefab, offersContainer, false) as GameObject;
                var text = offer.Trim();
                offerObj.GetComponent<SubscriptionOffer>().Init(iconsContainer.GetSprite(GSBackendKeys.ShopItem.GetOfferItemKey(text)), text);
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        audioService.PlayStandardClick();
        closeDailogueSignal.Dispatch();
    }

    private void OnPrivacyPolicyClicked()
    {
        audioService.PlayStandardClick();
        Application.OpenURL(metaDataModel.appInfo.privacyPolicyURL);
    }

    private void OnTermsOfUseClicked()
    {
        audioService.PlayStandardClick();
        showTermsSignal.Dispatch();
        Application.OpenURL(metaDataModel.appInfo.termsOfUseURL);
    }

    private void OnRestorePurchaseClicked()
    {
        audioService.PlayStandardClick();
        restorePurchasesSignal.Dispatch();
    }

    private void OnPurchaseButtonClicked()
    {
        audioService.PlayStandardClick();
        purchaseSignal.Dispatch();
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
        thinking.SetActive(!isAvailable);
        foreach(var btn in radioButtons)
        {
            var tempColor = btn.color;
            if(isAvailable)
                tempColor.a = 1f;
            else
                tempColor.a = 0.3f;
            btn.color = tempColor;
        }
        //purchaseText.color = isAvailable ? Colors.WHITE : Colors.DISABLED_WHITE;
    }

    private void SetupDefaultSubscriptionTier()
    {
        foreach (var t in subscriptionTiers)
        {
            t.isSelected = t.key.Equals(settingsModel.defaultSubscriptionKey);
            key = settingsModel.defaultSubscriptionKey;

            if (t.isSelected)
            {
                t.transform.SetAsFirstSibling();
            }
        }

    }
}


