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
        public GameObject coachTrainingDailogue;
        public GameObject strengthTrainingDailogue;

        private GameObject spawnedBanner;
        private Vector3 scrollViewOrignalPosition;
        private StoreItem storeItem;

        //Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        //Signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public LoadSpotPurchaseSignal loadSpotPurchaseSignal { get; set; }

        public void ShowPromotion(string key)
        {
            if (spawnedBanner != null)
            {
                Destroy(spawnedBanner);
            }
            
            if (LobbyPromotionKeys.Contains(key))
            {
                var prefabToInstantiate = Resources.Load(key);

                if (prefabToInstantiate != null)
                {
                    spawnedBanner = Instantiate(prefabToInstantiate, loadPromotionAt.position, Quaternion.identity, promotionContainer) as GameObject;
                    scrollRect.transform.localPosition = moveScrollViewTo.localPosition;
                    spawnedBanner.GetComponent<Button>().onClick.AddListener(() => OnClickPromotion(key));

                    var IAPBanner = spawnedBanner.GetComponent<IAPBanner>();
                    if (IAPBanner != null)
                    {
                        storeItem = metaDataModel.store.items[IAPBanner.key];

                        if (storeItem != null)
                        {
                            IAPBanner.price.text = localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE);
                            StartCoroutine(WaitForPriceToLoad(IAPBanner.price));
                        }
                        else
                        {
                            LogUtil.Log(string.Format("Banner Promotion: store item against key '{0}' not found", IAPBanner.key), "red");
                        }
                    }
                }
                else
                {
                    LogUtil.Log(string.Format("Banner Promotion: resource against key '{0}' not found", key), "red");
                }
            }
            else
            {
                scrollRect.transform.localPosition = scrollViewOrignalPosition;
            }
        }

        private void OnClickPromotion(string key)
        {
            audioService.PlayStandardClick();
            ShowPromotion("none");
            switch (key)
            {
                case LobbyPromotionKeys.COACH_BANNER:
                    coachTrainingDailogue.SetActive(true);
                    break;

                case LobbyPromotionKeys.STRENGTH_BANNER:
                    strengthTrainingDailogue.SetActive(true);
                    break;

                case LobbyPromotionKeys.ULTIMATE_BANNER:
                    purchaseStoreItemSignal.Dispatch(storeItem.key, true);
                    break;

                case LobbyPromotionKeys.ADS_BANNER:
                    purchaseStoreItemSignal.Dispatch(storeItem.key, true);
                    break;

                case LobbyPromotionKeys.STRENGTH_PURCHASE:
                    loadSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.MOVEMETER);
                    break;

                case LobbyPromotionKeys.COACH_PURCHASE:
                    loadSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.COACH);
                    break;
            }
        }

        private IEnumerator WaitForPriceToLoad(Text price)
        {
            yield return new WaitForSeconds(1.0f);

            if (storeItem.remoteProductPrice == null)
            {
                StartCoroutine(WaitForPriceToLoad(price));
            }

            price.text = storeItem.remoteProductPrice;
        }
    }
}