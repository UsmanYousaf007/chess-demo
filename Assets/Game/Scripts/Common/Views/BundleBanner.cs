using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

public class BundleBanner : MonoBehaviour
{ 
    public string key;
    public Text priceText;
    public Text gemsPayout;
    public Text coinsPayout;
    public Text discountText;
    public GameObject loading;
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupBundle(StoreItem bundleStoreItem, StoreItem gemStoreItem, StoreItem coinsStoreItem)
    {
        var isStoreAvailable = !string.IsNullOrEmpty(bundleStoreItem.remoteProductPrice);
        var discount = CalculateDiscount(bundleStoreItem, gemStoreItem, coinsStoreItem);

        //discountText.text = $"{(int)(discount * 100)}";
        priceText.text = bundleStoreItem.remoteProductPrice;
        gemsPayout.text = bundleStoreItem.currency3Cost.ToString("N0");
        coinsPayout.text = FormatUtil.AbbreviateNumber(bundleStoreItem.currency4Cost, false);
        loading.SetActive(!isStoreAvailable);
        button.interactable = isStoreAvailable;
        priceText.enabled = isStoreAvailable;
    }

    private float CalculateDiscount(StoreItem bundleStoreItem, StoreItem gemStoreItem, StoreItem coinsStoreItem)
    {
        var costPerGem = gemStoreItem.productPrice / gemStoreItem.currency3Payout;
        var priceForGems = bundleStoreItem.currency3Cost * costPerGem;
        var coinsPerGem = coinsStoreItem.currency4Payout / coinsStoreItem.currency3Payout;
        var priceForCoinsInGems = bundleStoreItem.currency4Cost / coinsPerGem;
        var priceForCoins = priceForCoinsInGems * costPerGem;
        var totalBundlePrice = priceForGems + priceForCoins;
        var overallDiscount = (float)(1 - (bundleStoreItem.productPrice / totalBundlePrice));

        return overallDiscount;
    }
}
