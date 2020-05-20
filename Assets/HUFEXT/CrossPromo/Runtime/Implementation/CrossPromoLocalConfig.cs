using HUF.Utils.Runtime.Configs.API;
using HUFEXT.CrossPromo.Runtime.Implementation.View.BannerTile;
using HUFEXT.CrossPromo.Runtime.Implementation.View.BottomPanel;
using HUFEXT.CrossPromo.Runtime.Implementation.View.BulletPoint;
using HUFEXT.CrossPromo.Runtime.Implementation.View.CrossPromoTile;
using UnityEngine;

namespace HUFEXT.CrossPromo.Runtime.Implementation
{
    [CreateAssetMenu(fileName = "CrossPromoLocalConfig", menuName = "HUFEXT/CrossPromo/CrossPromoLocalConfig")]
    public class CrossPromoLocalConfig : AbstractConfig
    {
        [SerializeField] CrossPromoView crossPromoRootPrefab = default;
        [SerializeField] TileContainer tileContainerPrefab = default;
        [SerializeField] BannerTileContainer bannerTileContainerPrefab = default;
        [SerializeField] BulletPointView bulletPointView = default;
        [SerializeField] bool useDefaultAppOrientation = default;
        [SerializeField] ScreenOrientation defaultAppOrientation = default;
        [SerializeField] Vector2 baseResolution = new Vector2(1080.0f, 1920.0f);
        [SerializeField] int crossPromoCanvasSortingOrder = default;
        [SerializeField] BottomPanelContainer customBottomPanelContainer = default;
        [SerializeField] string[] iOSEnabledCustomURLSchemes = default;
        [SerializeField] string[] iOSGameCustomURLSchemes = default;

        public CrossPromoView CrossPromoRootPrefab => crossPromoRootPrefab;
        public TileContainer TileContainerPrefab => tileContainerPrefab;
        public BannerTileContainer BannerTileContainerPrefab => bannerTileContainerPrefab;
        public BulletPointView BulletPointView => bulletPointView;
        public bool UseDefaultAppOrientation => useDefaultAppOrientation;
        public ScreenOrientation DefaultAppOrientation => defaultAppOrientation;
        public Vector2 BaseResolution => baseResolution;
        public int CrossPromoCanvasSortingOrder => crossPromoCanvasSortingOrder;
        public BottomPanelContainer CustomBottomPanelContainer => customBottomPanelContainer;
        public string[] EnabledCustomURLSchemesiOS => iOSEnabledCustomURLSchemes;
        public string[] GameCustomURLSchemesiOS => iOSGameCustomURLSchemes;
    }
}