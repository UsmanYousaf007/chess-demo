using System;
using System.Collections;
using System.Collections.Generic;
using HUF.Utils.Configs.API;
using HUFEXT.CrossPromo.API;
using HUFEXT.CrossPromo.Implementation.Model;
using HUFEXT.CrossPromo.Implementation.View.BannerTile;
using UnityEngine;

namespace HUFEXT.CrossPromo.Implementation.View.TopPanel
{
    public class TopPanelContainer : MonoBehaviour
    {
        [SerializeField] RectTransform self = default;
        [SerializeField] BannersLayout bannersLayout = default;
        [SerializeField] TopPanelCloseButton closeButton = default;

        float currentBannerWidth;
        float timeElapsed;
        float bannerSwitchTime;
        float bannerRotationTime;
        bool hasRotationStarted;

        int BannersCount => bannersLayout.BannersCount;
        int BulletPointIndex => currentBanner - 1;
        int currentBanner;

        void OnEnable()
        {
            var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();
            bannerSwitchTime = config.BannerTileRotateFrequency;
            bannerRotationTime = config.BannerTileRotationDuration;
            closeButton.SetAction(HandleCloseButtonClicked);
            bannersLayout.OnBannerRotate += HandleBannerRotate;
            bannersLayout.ResetPosition();
            bannersLayout.ResetBulletPoints();
            hasRotationStarted = false;
            currentBanner = 1;
            timeElapsed = 0.0f;
        }

        void OnDisable()
        {
            bannersLayout.OnBannerRotate -= HandleBannerRotate;
        }

        void HandleBannerRotate(RotationDirection rotationDirection)
        {
            StartCoroutine(RotateBanner(rotationDirection));
        }

        void HandleCloseButtonClicked()
        {
            HCrossPromo.ClosePanel();
        }

        void Update()
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > bannerSwitchTime)
            {
                StartCoroutine(RotateBanner(RotationDirection.Left));
            }
        }

        IEnumerator RotateBanner(RotationDirection rotationDirection)
        {
            timeElapsed = 0.0f;
            
            if (!hasRotationStarted)
            {
                hasRotationStarted = true;
                var currentTime = 0.0f;
                var rt = bannersLayout.GetComponent<RectTransform>();
                var startPosition = rt.anchoredPosition;
                var isLeft = rotationDirection == RotationDirection.Left;
                var targetDisplacement = isLeft ? -currentBannerWidth : currentBannerWidth;

                var isAtTheEndOfFilm = ((rotationDirection == RotationDirection.Right && currentBanner == 1) ||
                                        (rotationDirection == RotationDirection.Left && currentBanner == BannersCount));
                if (isAtTheEndOfFilm)
                {
                    currentBanner = isLeft ? 0 : BannersCount + 1;
                    targetDisplacement = -targetDisplacement * (BannersCount - 1);
                }

                currentBanner += (isLeft ? 1 : -1);
                bannersLayout.SetBulletPointActive(BulletPointIndex);

                while (currentTime < bannerRotationTime)
                {
                    currentTime += Time.deltaTime;
                    var dtStep = currentTime / bannerRotationTime;
                    var step = Mathf.Lerp(0.0f, targetDisplacement, dtStep);
                    rt.anchoredPosition = startPosition + new Vector2(step, 0.0f);
                    yield return null;
                }

                hasRotationStarted = false;
            }
        }

        public void AddNewBanner(TileModel model)
        {
            bannersLayout.AddOrUpdateBanner(model, self);
        }

        public void UpdateOrder()
        {
            bannersLayout.UpdateOrder();
        }

        public void RemoveObsoleteBanners(List<TileModel> newBanners)
        {
            bannersLayout.RemoveObsoleteBanners(newBanners);
        }

        public void UpdateBulletPointsColor(Color color)
        {
            bannersLayout.UpdateBulletPointsColor(color);
        }

        public void UpdateTexts()
        {
            bannersLayout.UpdateTexts();
        }

        void OnRectTransformDimensionsChange()
        {
            currentBannerWidth = self.sizeDelta.x;
        }
    }
}