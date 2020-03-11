using System.Collections.Generic;
using System.Linq;
using HUFEXT.CrossPromo.Implementation.Model;
using HUFEXT.CrossPromo.Implementation.View.BannerTile;
using HUFEXT.CrossPromo.Implementation.View.BulletPoint;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.CrossPromo.Implementation.View.TopPanel
{
    [RequireComponent(typeof(RectTransform))]
    public class BannersLayout : MonoBehaviour
    {
        [SerializeField] RectTransform self = default;
        [SerializeField] RectTransform bulletPointsContainer = default;

        readonly Dictionary<string, TileModel> banners = new Dictionary<string, TileModel>();
        readonly List<BannerViewModel> bannerViews = new List<BannerViewModel>();
        readonly List<BulletPointViewModel> bulletPointViews = new List<BulletPointViewModel>();

        public event UnityAction<RotationDirection> OnBannerRotate;
        public int BannersCount => bannerViews.Count;

        public void AddOrUpdateBanner(TileModel model, RectTransform sizeArchetype)
        {
            if (banners.ContainsKey(model.Uuid))
            {
                banners[model.Uuid] = model;
            }
            else
            {
                var newBanner = new BannerViewModel(model, self, sizeArchetype);
                var newBulletPoint = new BulletPointViewModel(bulletPointsContainer);
                newBanner.OnBannerRotate += HandleBannerRotate;
                banners.Add(model.Uuid, model);
                bannerViews.Add(newBanner);

                if (bulletPointViews.Count == 0)
                {
                    newBulletPoint.SetActive(false);
                }

                bulletPointViews.Add(newBulletPoint);

                if (bulletPointViews.Count == 2)
                {
                    bulletPointViews[0].SetActive(true);
                }
            }
        }

        void HandleBannerRotate(RotationDirection rotationDirection)
        {
            OnBannerRotate?.Invoke(rotationDirection);
        }

        public void UpdateOrder()
        {
            var sortedModels = banners
                .Select(a => a.Value)
                .OrderBy(a => a.Priority)
                .ToList();

            for (int i = 0; i < bannerViews.Count; i++)
            {
                bannerViews[i].UpdateView(sortedModels[i]);
            }
        }

        public void RemoveObsoleteBanners(List<TileModel> newBanners)
        {
            foreach (var bannerKey in banners.Keys.ToList())
            {
                var found = newBanners.Any(newTile => newTile.Uuid == bannerKey);
                if (!found)
                {
                    banners.Remove(bannerKey);
                }
            }

            var tilesCount = banners.Count;
            for (int i = tilesCount; i < bannerViews.Count; i++)
            {
                bannerViews[i].OnBannerRotate -= HandleBannerRotate;
                bannerViews[i].DestroyView();
                bulletPointViews[i].DestroyView();
            }

            bannerViews.RemoveRange(tilesCount, bannerViews.Count - tilesCount);
            bulletPointViews.RemoveRange(tilesCount, bulletPointViews.Count - tilesCount);
        }

        public void UpdateBulletPointsColor(Color color)
        {
            foreach (var bulletPoint in bulletPointViews)
            {
                bulletPoint.SetColor(color);
            }
        }

        public void SetBulletPointActive(int bulletPointIndex)
        {
            foreach (var bulletPoint in bulletPointViews)
            {
                bulletPoint.Deactivate();
            }

            if ( bulletPointViews.Count > 0 )
                bulletPointViews[bulletPointIndex].Activate();
        }

        public void ResetPosition()
        {
            self.anchoredPosition = Vector2.zero;
        }

        public void ResetBulletPoints()
        {
            if (bulletPointViews.Count > 0)
            {
                bulletPointViews[0].Activate(true);
            }

            for (int i = 1; i < bulletPointViews.Count; i++)
            {
                bulletPointViews[i].Deactivate(true);
            }
        }

        public void UpdateTexts()
        {
            foreach (var bannerView in bannerViews)
            {
                bannerView.UpdateTexts();
            }
        }
    }
}