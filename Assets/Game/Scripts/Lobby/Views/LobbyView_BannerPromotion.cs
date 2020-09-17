﻿using strange.extensions.mediation.impl;
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
        public Vector3 scrollViewOrignalPosition;
        private StoreItem storeItem;
        public float scrollViewportOrginalBottom;
        private PromotionVO currentPromotion;
        private IAPBanner iapBanner;
        private Vector3 playerProfileOriginalPosition;

        public static bool isCoachTrainingShown;
        public static bool isStrengthTrainingShown;

        [Inject] public LoadPromotionSingal loadPromotionSingal { get; set; }

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
                    spawnedBanner.GetComponent<Button>().onClick.AddListener(() => vo.onClick());

                    var updateBanner = spawnedBanner.GetComponent<UpdateBanner>();
                    if (updateBanner != null)
                    {
                        updateBanner.updateReleaseMessage.text = settingsModel.updateReleaseBannerMessage;
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