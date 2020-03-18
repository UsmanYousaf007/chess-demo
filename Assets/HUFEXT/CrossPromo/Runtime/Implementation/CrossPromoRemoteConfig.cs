using System.Collections.Generic;
using HUF.Utils.Configs.API;
using HUFEXT.CrossPromo.Implementation.Model;
using UnityEngine;

namespace HUFEXT.CrossPromo.Implementation
{
    [CreateAssetMenu(fileName = "CrossPromoRemoteConfig", menuName = "HUFEXT/CrossPromo/CrossPromoRemoteConfig")]
    public class CrossPromoRemoteConfig : FeatureConfigBase
    {
        [SerializeField] List<TileModel> topPanelCrossPromoGameModels = default;
        [SerializeField] List<TileModel> crossPromoPanelGameModels = default;
        [SerializeField] string bottomPanelLogoSpritePath = default;
        [SerializeField] float bannerTileRotateFrequency = default;
        [SerializeField] float bannerTileRotationDuration = default;
        [SerializeField] Color notInstalledStateButtonColor = default;
        [SerializeField] Color installedStateButtonColor = default;
        [SerializeField] Color bulletPointImageColor = default;
        [SerializeField] float topBannerHorizontalSwipeThreshold = default;
        [SerializeField] float clickDetectionThreshold = default;
        [SerializeField] float imagesInteractionFadeValue = default;
        [SerializeField] float imagesInteractionFadeTime = default;
        [SerializeField] float bulletPointFadeTime = default;
        
        public List<TileModel> TopPanelCrossPromoGameModels => topPanelCrossPromoGameModels;
        public List<TileModel> CrossPromoPanelGameModels => crossPromoPanelGameModels;
        public string BottomPanelLogoSpritePath => bottomPanelLogoSpritePath;
        public float BannerTileRotateFrequency => bannerTileRotateFrequency;
        public float BannerTileRotationDuration => bannerTileRotationDuration;
        public Color NotInstalledStateButtonColor => notInstalledStateButtonColor;
        public Color InstalledStateButtonColor => installedStateButtonColor;
        public Color BulletPointImageColor => bulletPointImageColor;
        public float TopBannerHorizontalSwipeThreshold => topBannerHorizontalSwipeThreshold;
        public float ClickDetectionThreshold => clickDetectionThreshold;
        public float ImagesInteractionFadeValue => imagesInteractionFadeValue;
        public float ImagesInteractionFadeTime => imagesInteractionFadeTime;
        public float BulletPointFadeTime => bulletPointFadeTime;
    }
}