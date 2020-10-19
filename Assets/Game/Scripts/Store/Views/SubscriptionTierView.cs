using strange.extensions.mediation.impl;
using UnityEngine.UI;
using UnityEngine;
using System;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;

public class SubscriptionTierView : View
{
    [Serializable]
    public class TierConfig
    {
        public Sprite bg;
        public Color headingsColor;
        public Color textColor;
        public Vector3 scale;
        public Vector3 bestValuePosition;
        public Sprite processing;
        public float bottomContainerSpacing;
    }

    public string key;
    public Button bg;

    public bool isSelected;
    public bool isMonthly;
    public float savingsValue;
    public TierConfig selectedConfig;
    public TierConfig defaultConfig;
    public Text singleText;
    public Text infoText;
    public Image toggleBall;

    //TO-DO Remove code - SubDlg with icons
    /*public Text title;
    public Text actualPrice;
    public Image actualPriceStrikeThrough;
    public Text price;
    public Text billed;
    public GameObject billedSeperator;
    public Text savings;
    public Text bestValue;
    public bool showBestValue;
    public RectTransform bestValueObject;
    public RectTransform root;
    public HorizontalLayoutGroup bottomContainer;

    [Header("Not Available")]
    public RectTransform package;
    public GameObject notAvailable;
    public Image[] processing;*/

    //Signals
    public Signal selectTierClicked = new Signal();

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }

    public void InitOnce()
    {
        bg.onClick.AddListener(OnClickSelectButton);
        //TO-DO Remove code - SubDlg with icons
        //bestValueObject.gameObject.SetActive(showBestValue);
        //SelectTier(isSelected);
    }

    public void Init(bool isStoreAvailable)
    {
        //TO-DO Remove code - SubDlg with icons
        //package.gameObject.SetActive(isStoreAvailable);
        //notAvailable.SetActive(!isStoreAvailable);

        if (!isStoreAvailable)
        {
            return;
        }

        var item = metaDataModel.store.items[key];

        if (item == null)
        {
            return;
        }

        SelectTier(isSelected);

        //TO-DO Remove code - SubDlg with icons
        //title.text = item.displayName;

        if (isMonthly)
        {
            item.originalPrice = Math.Round(item.productPrice / (decimal)(1 - savingsValue), 2);
            item.discountedRatio = savingsValue;

            //TO-DO Remove code - SubDlg with icons
            /*actualPrice.text = $"{item.remoteProductCurrencyCode} {item.originalPrice}";
            price.text = $"{item.remoteProductCurrencyCode} {item.productPrice}/mo";
            billed.text = $"Billed {item.remoteProductCurrencyCode} {item.productPrice} monthly";
            //billed.gameObject.SetActive(false);
            billedSeperator.gameObject.SetActive(false);
            savings.text = $"Save {savingsValue * 100}%";*/

            //singleText.text = $"Upgrade with {savingsValue * 100}% off, for {item.remoteProductCurrencyCode} {item.productPrice}, billed monthly";
            singleText.text = $"<b>{item.remoteProductCurrencyCode} {item.productPrice} / Month</b>";
            infoText.enabled = false;
        }
        else
        {
            var monthlyItem = metaDataModel.store.items["Subscription"];
            var monthlyPrice = item.productPrice / 12;
            savingsValue = 1 - (float)(monthlyPrice / monthlyItem.productPrice);
            item.originalPrice = Math.Round(item.productPrice / (decimal)(1 - savingsValue), 2);
            item.discountedRatio = savingsValue;

            //TO-DO Remove code - SubDlg with icons
            /*actualPrice.text = $"{item.remoteProductCurrencyCode} {item.originalPrice}";
            price.text = $"{item.remoteProductCurrencyCode} {item.productPrice}/yr";
            billed.text = $"Just {item.remoteProductCurrencyCode} {Math.Round(monthlyPrice, 2)}/mo"; ;
            savings.text = $"Save {(int)(savingsValue * 100)}%";
            var showSavings = savingsValue > 0;
            actualPrice.gameObject.SetActive(showSavings);
            savings.gameObject.SetActive(showSavings);*/

            //singleText.text = $"Upgrade with {(int)(savingsValue * 100)}% off, for {item.remoteProductCurrencyCode} {Math.Round(monthlyPrice, 2)}/mo, billed annually";
            singleText.text = $"<b>{item.remoteProductCurrencyCode} {item.productPrice} / Year</b>";
            infoText.text = $"(12 months at {item.remoteProductCurrencyCode} {Math.Round(monthlyPrice, 2)} / mo. Save {(int)(savingsValue * 100)}%)";
            infoText.enabled = true;
        }
    }

    private void OnClickSelectButton()
    {
        selectTierClicked.Dispatch();
    }

    public void SelectTier(bool isSelected)
    {
        this.isSelected = isSelected;
        var config = isSelected ? selectedConfig : defaultConfig;

        toggleBall.enabled = isSelected;
        singleText.color = config.textColor;

        //TO-DO Remove code - SubDlg with icons
        /*title.color = price.color = savings.color = config.headingsColor;
        actualPrice.color = billed.color = actualPriceStrikeThrough.color = config.textColor;
        bg.image.sprite = config.bg;
        bg.image.SetNativeSize();
        root.sizeDelta = bg.image.rectTransform.sizeDelta;
        bestValueObject.localPosition = config.bestValuePosition;
        title.transform.localScale = price.transform.localScale = savings.transform.localScale = actualPrice.transform.localScale = billed.transform.localScale = config.scale;
        bottomContainer.spacing = config.bottomContainerSpacing;

        foreach (var bar in processing)
        {
            bar.sprite = config.processing;
        }*/

    }
}
