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
    public Text title;
    public Text actualPrice;
    public Image actualPriceStrikeThrough;
    public Text price;
    public Text billed;
    public GameObject billedSeperator;
    public Text savings;
    public Text bestValue;
    public RectTransform bestValueObject;
    public bool showBestValue;
    public bool isSelected;
    public bool isMonthly;
    public float savingsValue;
    public TierConfig selectedConfig;
    public TierConfig defaultConfig;
    public RectTransform root;
    public HorizontalLayoutGroup bottomContainer;

    [Header("Not Available")]
    public RectTransform package;
    public GameObject notAvailable;
    public Image[] processing;

    //Signals
    public Signal selectTierClicked = new Signal();

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }

    public void InitOnce()
    {
        bg.onClick.AddListener(OnClickSelectButton);
        bestValueObject.gameObject.SetActive(showBestValue);
        SelectTier(isSelected);
    }

    public void Init(bool isStoreAvailable)
    {
        package.gameObject.SetActive(isStoreAvailable);
        notAvailable.SetActive(!isStoreAvailable);

        if (!isStoreAvailable)
        {
            return;
        }

        var item = metaDataModel.store.items[key];

        if (item == null)
        {
            return;
        }

        title.text = item.displayName;

        if (isMonthly)
        {
            item.originalPrice = Math.Round(item.productPrice / (decimal)(1 - savingsValue), 2);
            actualPrice.text = $"{item.remoteProductCurrencyCode} {item.originalPrice}";
            price.text = $"{item.remoteProductCurrencyCode} {item.productPrice}/mo";
            //billed.text = $"Billed {item.remoteProductCurrencyCode} {item.productPrice} monthly";
            billed.gameObject.SetActive(false);
            billedSeperator.gameObject.SetActive(false);
            savings.text = $"Save {savingsValue * 100}%";
            item.discountedRatio = savingsValue;
        }
        else
        {
            var monthlyItem = metaDataModel.store.items["Subscription"];
            var monthlyPrice = item.productPrice / 12;
            savingsValue = 1 - (float)(monthlyPrice / monthlyItem.productPrice);
            item.originalPrice = Math.Round(item.productPrice / (decimal)(1 - savingsValue), 2);
            actualPrice.text = $"{item.remoteProductCurrencyCode} {item.originalPrice}";
            price.text = $"{item.remoteProductCurrencyCode} {item.productPrice}/yr";
            billed.text = $"Just {item.remoteProductCurrencyCode} {Math.Round(monthlyPrice, 2)}/mo"; ;
            savings.text = $"Save {(int)(savingsValue * 100)}%";
            var showSavings = savingsValue > 0;
            actualPrice.gameObject.SetActive(showSavings);
            savings.gameObject.SetActive(showSavings);
            item.discountedRatio = savingsValue;
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
        title.color = price.color = savings.color = config.headingsColor;
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
        }
    }
}
