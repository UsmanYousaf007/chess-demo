using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class NewLobbyView : View
    {
        [Header("Lobby Promotions")]
        public RectTransform promotionContainer;

        private GameObject spawnedBanner;
        private PromotionVO currentPromotion;
        private SaleBanner saleBanner;
        private BundleBanner bundleBanner;

        [Inject] public LoadPromotionSingal loadPromotionSingal { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }

        public void ShowPromotion(PromotionVO vo)
        {
            currentPromotion = vo;

            if (spawnedBanner != null)
            {
                Destroy(spawnedBanner);
            }


            if (LobbyPromotionKeys.Contains(vo.key))
            {
                var prefabToInstantiate = Resources.Load(vo.key);

                if (prefabToInstantiate != null)
                {
                    spawnedBanner = Instantiate(prefabToInstantiate, promotionContainer) as GameObject;
                    spawnedBanner.transform.SetAsFirstSibling();
                    spawnedBanner.GetComponent<Button>().onClick.AddListener(() => vo.onClick());
                    saleBanner = spawnedBanner.GetComponent<SaleBanner>();
                    bundleBanner = spawnedBanner.GetComponent<BundleBanner>();

                    SetupSaleBanner();
                    SetupBundleBanner();
                }
                else
                {
                    LogUtil.Log(string.Format("Banner Promotion: resource against key '{0}' not found", vo.key), "red");
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(promotionContainer);
        }

        public void SetPriceOfIAPBanner(bool isAvailable)
        {
            if (isAvailable)
            {
                SetupSaleBanner();
                SetupBundleBanner();
            }
        }

        public void RemovePromotion()
        {
            if (LobbyPromotionKeys.Contains(currentPromotion.key) && !currentPromotion.condition())
            {
                loadPromotionSingal.Dispatch();
            }
        }

        public void LogSubscriptionBannerPurchasedAnalytics(string key)
        {
            if (LobbyPromotionKeys.Contains(currentPromotion.key) && !currentPromotion.condition())
            {
                var context = key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG) ? AnalyticsContext.monthly_sub :
                    key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_ANNUAL_SHOP_TAG) ? AnalyticsContext.annual_sub : AnalyticsContext.annual_mega_sale;
                analyticsService.Event(AnalyticsEventId.banner_purchased, context);
            }
        }

        public void LogBannerPurchasedAnalytics()
        {
            analyticsService.Event(AnalyticsEventId.banner_purchased, currentPromotion.analyticsContext);
        }

        private void SetupSaleBanner()
        {
            if (saleBanner != null)
            {
                var isSaleActive = promotionsService.IsSaleActive(saleBanner.saleKey);
                var regularItem = metaDataModel.store.items[saleBanner.key];
                var saleItem = metaDataModel.store.items[saleBanner.saleKey];
                saleBanner.SetupSale(isSaleActive, regularItem, saleItem);
            }
        }

        private void SetupBundleBanner()
        {
            if (bundleBanner != null)
            {
                var gemsItem = metaDataModel.store.items["GemPack150"];
                var coinsItem = metaDataModel.store.items["CoinPack1"];
                var bundleItem = metaDataModel.store.items[bundleBanner.key];
                bundleBanner.SetupBundle(bundleItem, gemsItem, coinsItem);
            }
        }
    }
}