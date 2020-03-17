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
    }

    public string key;
    public Button bg;
    public Text title;
    public Text actualPrice;
    public Text price;
    public Text billed;
    public Text savings;
    public Text bestValue;
    public RectTransform bestValueObject;
    public bool showBestValue;
    public bool isSelected;
    public bool isMonthly;
    public float savingsValue;
    public TierConfig selectedConfig;
    public TierConfig defaultConfig;

    //Signals
    public Signal selectTierClicked = new Signal();

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    public void InitOnce()
    {
        bg.onClick.AddListener(OnClickSelectButton);
        bestValueObject.gameObject.SetActive(showBestValue);
        SelectTier(isSelected);
    }

    public void Init()
    {
        var item = metaDataModel.store.items[key];

        if (item == null)
        {
            return;
        }

        title.text = item.displayName;

        if (isMonthly)
        {
            actualPrice.text = $"{item.remoteProductCurrencyCode} {item.productPrice + (item.productPrice * (decimal)savingsValue)}";
            price.text = $"{item.remoteProductCurrencyCode} {item.productPrice}/mo";
            billed.text = $"Billed {item.remoteProductCurrencyCode} {item.productPrice} monthly";
            savings.text = $"Save {savingsValue * 100}%";
        }
        else
        {
            var monthlyItem = metaDataModel.store.items["Subscription"];
            actualPrice.text = $"{monthlyItem.remoteProductCurrencyCode} {monthlyItem.productPrice}";
            var monthlyPrice = item.productPrice / 12;
            price.text = $"{item.remoteProductCurrencyCode} {(int) monthlyPrice}/mo";
            billed.text = $"Billed {item.remoteProductCurrencyCode} {item.productPrice} annually";
            savingsValue = (float)(monthlyPrice / monthlyItem.productPrice);
            savings.text = $"Save {(int)(savingsValue * 100)}%";
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
        actualPrice.color = billed.color = config.textColor;
        bg.image.sprite = config.bg;
        this.transform.localScale = config.scale;
        bestValueObject.localPosition = config.bestValuePosition;
    }
}
