using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;
using System;
using TurboLabz.InstantFramework;
using strange.extensions.signal.impl;

public class ManageSubscriptionView : View
{
    [Serializable]
    public class Section
    {
        public Text title;
        public GameObject popular;
        public Text popularText;
        public Text savingsText;
        public Text price;
        public Text billed;
    }

    public Section topSection;
    public Section middleSection;
    public GameObject offerPrefab;
    public Transform offersContainer;
    public Text benefitsText;
    public Button switchButton;
    public Text switchButtonText;
    public Text optionsText;
    public Button manageSubscriptionButton;
    public Text manageSubscriptionText;
    public Button backButton;
    public Text backText;

    //Services
    [Inject] public IAudioService audioService { get; set; }
    [Inject] public ILocalizationService localizationService { get; set; }

    //Signals
    public Signal backSignal = new Signal ();
    public Signal switchPlanSignal = new Signal();

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    public void Init()
    {
        backButton.onClick.AddListener(OnBackClicked);
        manageSubscriptionButton.onClick.AddListener(OnManageSubscriptionClicked);
        switchButton.onClick.AddListener(OnSwitchPlanClicked);

        backText.text = localizationService.Get(LocalizationKey.BACK_TEXT);
        manageSubscriptionText.text = localizationService.Get(LocalizationKey.SUB_MANAGE);
        optionsText.text = localizationService.Get(LocalizationKey.SUB_OPTIONS);
        benefitsText.text = localizationService.Get(LocalizationKey.SUB_BENEFITS);
        topSection.popularText.text = localizationService.Get(LocalizationKey.SUB_POPULAR);
        middleSection.popularText.text = localizationService.Get(LocalizationKey.SUB_POPULAR);
    }

    public void Show()
    {
        Setup();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Setup()
    {
        var monthlySubscription = metaDataModel.store.items[GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG];
        var annualSubscription = metaDataModel.store.items[GSBackendKeys.ShopItem.SUBSCRIPTION_ANNUAL_SHOP_TAG];
        var isMonthlyActive = playerModel.subscriptionType.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG);
        var monthlyPriceForAnnual = annualSubscription.productPrice / 12;
        var annualSavings = (float)(monthlyPriceForAnnual / monthlySubscription.productPrice);

        if (isMonthlyActive)
        {
            topSection.title.text = localizationService.Get(LocalizationKey.SUB_MONTHLY);
            topSection.popular.SetActive(false);
            topSection.billed.gameObject.SetActive(false);
            topSection.price.text = $"{monthlySubscription.remoteProductCurrencyCode} {monthlySubscription.productPrice}/mo";

            middleSection.title.text = localizationService.Get(LocalizationKey.SUB_SWITCH_ANNUAL);
            middleSection.popular.SetActive(true);
            middleSection.billed.gameObject.SetActive(true);
            middleSection.billed.text = $"{annualSubscription.remoteProductCurrencyCode} {annualSubscription.productPrice} Annually";
            middleSection.price.text = $"{annualSubscription.remoteProductCurrencyCode} {(int)monthlyPriceForAnnual}/mo";
            middleSection.savingsText.text = $"{(int)(annualSavings * 100)}%\nSavings";

            switchButtonText.text = localizationService.Get(LocalizationKey.SUB_SWITCH_ANNUAL_BTN);
        }
        else
        {
            topSection.title.text = localizationService.Get(LocalizationKey.SUB_ANNUAL);
            topSection.popular.SetActive(true);
            topSection.billed.gameObject.SetActive(true);
            topSection.price.text = $"{annualSubscription.remoteProductCurrencyCode} {(int)monthlyPriceForAnnual}/mo";
            topSection.billed.text = $"{annualSubscription.remoteProductCurrencyCode} {annualSubscription.productPrice} Annually";
            topSection.savingsText.text = $"{(int)(annualSavings * 100)}%\nSavings";

            middleSection.title.text = localizationService.Get(LocalizationKey.SUB_SWITCH_MONTHLY);
            middleSection.popular.SetActive(false);
            middleSection.billed.gameObject.SetActive(false);
            middleSection.price.text = $"{monthlySubscription.remoteProductCurrencyCode} {monthlySubscription.productPrice}/mo";

            switchButtonText.text = localizationService.Get(LocalizationKey.SUB_SWITCH_MONTHLY_BTN);
        }

        // Fill only once
        if (offersContainer.childCount == 0)
        {
            var offers = monthlySubscription.description.Split(',');
            foreach (var offer in offers)
            {
                var offerObj = Instantiate(offerPrefab, offersContainer, false) as GameObject;
                offerObj.GetComponentInChildren<Text>().text = offer.Trim();
            }
        }
    }

    private void OnSwitchPlanClicked()
    {
        audioService.PlayStandardClick();
#if UNITY_ANDROID
        switchPlanSignal.Dispatch();
#elif UNITY_IOS
        Application.OpenURL(metaDataModel.settingsModel.manageSubscriptionURL);
#endif
    }

    private void OnManageSubscriptionClicked()
    {
        audioService.PlayStandardClick();
        Application.OpenURL(metaDataModel.settingsModel.manageSubscriptionURL);
    }

    private void OnBackClicked()
    {
        audioService.PlayStandardClick();
        backSignal.Dispatch();
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }
}
