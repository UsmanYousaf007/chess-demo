using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

public class SaleBanner : MonoBehaviour
{
    public string key;
    public string saleKey;
    public GameObject[] saleItems;
    public GameObject nonSaleItem;
    public Text originalPrice;
    public Text newPrice;
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

    public void SetupSale(bool saleActive, StoreItem originalItem, StoreItem saleItem)
    {
        foreach (var item in saleItems)
        {
            item.SetActive(saleActive);
        }

        nonSaleItem.SetActive(!saleActive);

        var isStoreAvailable = !string.IsNullOrEmpty(originalItem.remoteProductPrice);

        loading.SetActive(!isStoreAvailable);
        button.interactable = isStoreAvailable;
        originalPrice.gameObject.SetActive(isStoreAvailable && saleActive);
        newPrice.gameObject.SetActive(isStoreAvailable && saleActive);
        discountText.gameObject.SetActive(isStoreAvailable);

        if (isStoreAvailable)
        {
            originalPrice.text = originalItem.remoteProductPrice;
            newPrice.text = saleItem.remoteProductPrice;

            var discount = 1 - (float)(saleItem.productPrice / originalItem.productPrice);
            discountText.text = $"OFF\n{(int)(discount * 100)}%";
        }
    }
}
