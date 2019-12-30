using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using System.Collections;
using HUF.Analytics.API;

namespace TurboLabz.InstantFramework
{
    public partial class LobbyView : View
    {
        [Header("Lobby Promotions")]
        public Transform loadPromotionAt;
        public Transform moveScrollViewTo;
        public Transform promotionContainer;
        public float setScorllViewportBottomTo;
        public RectTransform scrollViewport;
        public GameObject coachTrainingDailogue;
        public GameObject strengthTrainingDailogue;

        private GameObject spawnedBanner;
        private Vector3 scrollViewOrignalPosition;
        private StoreItem storeItem;
        private float scrollViewportOrginalBottom;
        private PromotionVO currentPromotion;
        private IAPBanner iapBanner;

        public static bool isCoachTrainingShown;
        public static bool isStrengthTrainingShown;

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
                    spawnedBanner = Instantiate(prefabToInstantiate, loadPromotionAt.position, Quaternion.identity, promotionContainer) as GameObject;
                    scrollRect.transform.localPosition = moveScrollViewTo.localPosition;
                    scrollViewport.offsetMin = new Vector2(scrollViewport.offsetMin.x, setScorllViewportBottomTo);
                    scrollRect.verticalNormalizedPosition = 1;

                    iapBanner = spawnedBanner.GetComponent<IAPBanner>();
                    if (iapBanner != null)
                    {
                        storeItem = metaDataModel.store.items[iapBanner.key];

                        if (storeItem != null)
                        {
                            iapBanner.price.text = storeItem.remoteProductPrice == null ?
                                localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE) :
                                storeItem.remoteProductPrice;
                            spawnedBanner.GetComponent<Button>().onClick.AddListener(() => vo.onClick(storeItem.key));

                            if (iapBanner.payout != null && storeItem.bundledItems.ContainsKey(GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG))
                            {
                                iapBanner.payout.text = metaDataModel.store.items[GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG].displayName;
                            }
                        }
                        else
                        {
                            LogUtil.Log(string.Format("Banner Promotion: store item against key '{0}' not found", iapBanner.key), "red");
                        }
                    }
                    else
                    {
                        var updateBanner = spawnedBanner.GetComponent<UpdateBanner>();
                        if (updateBanner != null)
                        {
                            updateBanner.updateReleaseMessage.text = settingsModel.updateReleaseBannerMessage;
                        }

                        spawnedBanner.GetComponent<Button>().onClick.AddListener(() => vo.onClick());
                    }
                }
                else
                {
                    LogUtil.Log(string.Format("Banner Promotion: resource against key '{0}' not found", vo.key), "red");
                }
            }
            else
            {
                scrollRect.transform.localPosition = scrollViewOrignalPosition;
                scrollViewport.offsetMin = new Vector2(scrollViewport.offsetMin.x, scrollViewportOrginalBottom);
            }
        }

        public void SetPriceOfIAPBanner(bool isAvailable)
        {
            if (isAvailable && iapBanner != null)
            {
                iapBanner.price.text = storeItem.remoteProductPrice;
            }
        }

        public void ShowCoachTrainingDailogue()
        {
            coachTrainingDailogue.SetActive(true);
            isCoachTrainingShown = true;
        }

        public void ShowStrengthTrainingDailogue()
        {
            strengthTrainingDailogue.SetActive(true);
            isStrengthTrainingShown = true;
        }

        public void RemovePromotion(string key)
        {
            if (currentPromotion.key.Equals(key))
            {
                //for closing promotion pass key 'none'
                ShowPromotion(new PromotionVO
                {
                    cycleIndex = 0,
                    key = "none",
                    condition = null,
                    onClick = null,
                    analyticsImpId = 0
                });
            }
        }

        public void ReportAnalytic(string key, AnalyticsEventId eventId)
        {
            if (currentPromotion.key.Equals(key))
            {
                analyticsService.Event(eventId);
            }
        }

        public void ReportHAnalytic(string key, string result)
        {
            if (iapBanner != null  && iapBanner.key.Equals(key) && storeItem != null)
            {
                var analyticsEvent = AnalyticsMonetizationEvent.Create(result, storeItem.currency1Cost)
                .ST1("iap_purchase")
                .ST2(storeItem.displayName.Replace("Ad Free", "special").Replace(" ", "_").ToLower())
                .ST3("lobby_banner")
                .Value(storeItem.currency1Cost);
                HAnalytics.LogMonetizationEvent((AnalyticsMonetizationEvent)analyticsEvent);
            }
        }
    }
}