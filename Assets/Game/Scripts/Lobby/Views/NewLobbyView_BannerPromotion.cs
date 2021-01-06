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

                    if (saleBanner != null)
                    {
                        var isSaleActive = promotionsService.IsSaleActive(saleBanner.saleKey);
                        var regularItem = metaDataModel.store.items[saleBanner.key];
                        var saleItem = metaDataModel.store.items[saleBanner.saleKey];
                        saleBanner.SetupSale(isSaleActive, regularItem, saleItem);
                    }
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
            if (isAvailable && saleBanner != null)
            {
                var isSaleActive = promotionsService.IsSaleActive(saleBanner.saleKey);
                var regularItem = metaDataModel.store.items[saleBanner.key];
                var saleItem = metaDataModel.store.items[saleBanner.saleKey];
                saleBanner.SetupSale(isSaleActive, regularItem, saleItem);
            }
        }

        public void RemovePromotion(string key)
        {
            if (LobbyPromotionKeys.Contains(currentPromotion.key) && !currentPromotion.condition())
            {
                if (IsVisible())
                {
                    analyticsService.Event(AnalyticsEventId.banner_purchased, currentPromotion.analyticsContext);
                }

                loadPromotionSingal.Dispatch();
            }
        }
    }
}