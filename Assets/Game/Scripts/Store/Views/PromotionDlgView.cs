using HUF.Analytics.API;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

public class PromotionDlgView : View
{
    public string key;
    public Text title;
    public GameObject offerPrefab;
    public Transform offersContainer;
    public Button closeButton;
    public Text priceText;
    public Text purchaseText;
    public Button purchaseButton;
    public GameObject uiBlocker;
    public GameObject processingUi;

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal purchaseSignal = new Signal();

    public void InitOnce()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
    }

    public void Init()
    {
        title.text = localizationService.Get(LocalizationKey.PROMOTON_DLG_TITLE);
        
        purchaseText.text = localizationService.Get(LocalizationKey.PROMOTION_DLG_PURCHASE);

        var storeItem = metaDataModel.store.items[key];

        if (storeItem == null)
            return;


        string subscriptionInfo = localizationService.Get(LocalizationKey.PROMOTION_DLG_PRICE);
        string price = storeItem.remoteProductPrice;

        string subscriptionPriceString = subscriptionInfo.Replace("(price)", price);
        priceText.text = subscriptionPriceString;

        //priceText.text = string.Format("then {0} per month", storeItem.remoteProductPrice);

        if (offersContainer.childCount == 0)
        {
            var offers = storeItem.description.Split(',');
            foreach (var offer in offers)
            {
                var offerObj = Instantiate(offerPrefab, offersContainer, false) as GameObject;
                offerObj.GetComponentInChildren<Text>().text = offer;
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        HAnalytics.LogEvent(AnalyticsEvent.Create("internal_ad_displayed").ST1("monetization").ST2("internal_fullscreen").ST3("internal"));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        audioService.PlayStandardClick();
        HAnalytics.LogEvent(AnalyticsEvent.Create("internal_ad_closed").ST1("monetization").ST2("internal_fullscreen").ST3("internal"));
        closeDailogueSignal.Dispatch();
    }

    private void OnPurchaseButtonClicked()
    {
        audioService.PlayStandardClick();
        HAnalytics.LogEvent(AnalyticsEvent.Create("internal_ad_clicked").ST1("monetization").ST2("internal_fullscreen").ST3("internal"));
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
}


