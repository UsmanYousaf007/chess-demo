using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

public class SubscriptionDlgView : View
{
    public string key;
    public Text title;
    public GameObject offerPrefab;
    public Transform offersContainer;
    public Button closeButton;
    public Text disclaimer;
    public Text privacyPolicyText;
    public Button privacyPolicyButton;
    public Text termsOfUseText;
    public Button termsOfUseButton;
    public Text restorePurchaseText;
    public Button restorePurchaseButton;
    public Text priceText;
    public Text purchaseText;
    public Button purchaseButton;
    public GameObject uiBlocker;
    public GameObject processingUi;

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal restorePurchasesSignal = new Signal();
    public Signal purchaseSignal = new Signal();

    public void Init()
    {
        title.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TITLE);
        restorePurchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE);
        disclaimer.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_DISCLAIMER);
        privacyPolicyText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PRIVACY_POLICY);
        termsOfUseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TERMS_OF_USE);
        purchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PURCHASE_BUTTON);

        closeButton.onClick.AddListener(OnCloseButtonClicked);
        privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyClicked);
        termsOfUseButton.onClick.AddListener(OnTermsOfUseClicked);
        restorePurchaseButton.onClick.AddListener(OnRestorePurchaseClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);

        var storeItem = metaDataModel.store.items[key];

        if (storeItem == null)
            return;

        priceText.text = string.Format("then {0} per month", storeItem.remoteProductPrice);
        var offers = storeItem.description.Split(',');
        foreach (var offer in offers)
        {
            var offerObj = Instantiate(offerPrefab, offersContainer, false) as GameObject;
            offerObj.GetComponentInChildren<Text>().text = offer;
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
        closeDailogueSignal.Dispatch();
    }

    private void OnPrivacyPolicyClicked()
    {
        Application.OpenURL(metaDataModel.appInfo.privacyPolicyURL);
    }

    private void OnTermsOfUseClicked()
    {
        Application.OpenURL(metaDataModel.appInfo.termsOfUseURL);
    }

    private void OnRestorePurchaseClicked()
    {
        restorePurchasesSignal.Dispatch();
    }

    private void OnPurchaseButtonClicked()
    {
        purchaseSignal.Dispatch();
    }

    public void ShowProcessing(bool show, bool showProcessingUi)
    {
        processingUi.SetActive(showProcessingUi);
        uiBlocker.SetActive(show);
    }
}


