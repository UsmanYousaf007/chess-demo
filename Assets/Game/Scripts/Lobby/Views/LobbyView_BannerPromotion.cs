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

        public void ShowPromotion(PromotionVO vo)
        {
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

                    var IAPBanner = spawnedBanner.GetComponent<IAPBanner>();
                    if (IAPBanner != null)
                    {
                        storeItem = metaDataModel.store.items[IAPBanner.key];

                        if (storeItem != null)
                        {
                            IAPBanner.price.text = localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE);
                            StartCoroutine(WaitForPriceToLoad(IAPBanner.price));
                            spawnedBanner.GetComponent<Button>().onClick.AddListener(() => vo.onClick(storeItem.key));
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

        public void ShowCoachTrainingDailogue()
        {
            coachTrainingDailogue.SetActive(true);
        }

        public void ShowStrengthTrainingDailogue()
        {
            strengthTrainingDailogue.SetActive(true);
        }
    }
}