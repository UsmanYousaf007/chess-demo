using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using System.Collections;

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

                    var IAPBanner = spawnedBanner.GetComponent<IAPBanner>();
                    if (IAPBanner != null)
                    {
                        storeItem = metaDataModel.store.items[IAPBanner.key];

                        if (storeItem != null)
                        {
                            IAPBanner.price.text = localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE);
                            StartCoroutine(WaitForPriceToLoad(IAPBanner.price));
                            spawnedBanner.GetComponent<Button>().onClick.AddListener(() => vo.onClick(storeItem.key));

                            if (IAPBanner.payout != null && storeItem.bundledItems.ContainsKey(GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG))
                            {
                                IAPBanner.payout.text = metaDataModel.store.items[GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG].displayName;
                            }
                        }
                        else
                        {
                            LogUtil.Log(string.Format("Banner Promotion: store item against key '{0}' not found", IAPBanner.key), "red");
                        }
                    }
                    else
                    {
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

        private IEnumerator WaitForPriceToLoad(Text price)
        {
            yield return new WaitForSeconds(1.0f);

            if (price != null)
            {
                if (storeItem.remoteProductPrice == null)
                {
                    StartCoroutine(WaitForPriceToLoad(price));
                }

                if (storeItem.remoteProductPrice != null && price.text.Equals(localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE)))
                {
                    price.text = storeItem.remoteProductPrice;
                }
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
    }
}