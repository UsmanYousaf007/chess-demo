using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;

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
        public GameObject playerProfile;
        public Transform movePlayerProfileToPivot;

        private GameObject spawnedBanner;
        private Vector3 scrollViewOrignalPosition;
        private StoreItem storeItem;
        private float scrollViewportOrginalBottom;
        private PromotionVO currentPromotion;
        private IAPBanner iapBanner;
        private Vector3 playerProfileOriginalPosition;

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
                    playerProfile.transform.localPosition = movePlayerProfileToPivot.localPosition;

                    iapBanner = spawnedBanner.GetComponent<IAPBanner>();
                    if (iapBanner != null)
                    {
                        storeItem = metaDataModel.store.items[iapBanner.key];

                        if (storeItem != null)
                        {
                            //iapBanner.price.text = storeItem.remoteProductPrice == null ?
                                //localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE) :
                                //storeItem.remoteProductPrice;
                            spawnedBanner.GetComponent<Button>().onClick.AddListener(() => vo.onClick(storeItem.key));

                            //if (iapBanner.payout != null && storeItem.bundledItems.ContainsKey(GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG))
                            //{
                            //    iapBanner.payout.text = metaDataModel.store.items[GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG].displayName;
                            //}
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
                playerProfile.transform.localPosition = playerProfileOriginalPosition;
            }
        }

        public void SetPriceOfIAPBanner(bool isAvailable)
        {
            if (isAvailable && iapBanner != null)
            {
                //iapBanner.price.text = storeItem.remoteProductPrice;
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
            if (iapBanner != null)
            {
                //for closing promotion pass key 'none'
                ShowPromotion(new PromotionVO
                {
                    cycleIndex = 0,
                    key = "none",
                    condition = null,
                    onClick = null
                });
            }
        }
    }
}