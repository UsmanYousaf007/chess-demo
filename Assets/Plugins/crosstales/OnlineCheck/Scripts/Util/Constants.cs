namespace Crosstales.OnlineCheck.Util
{
    /// <summary>Collected constants of very general utility for the asset.</summary>
    public abstract class Constants : Common.Util.BaseConstants
    {

        #region Constant variables

        /// <summary>Name of the asset.</summary>
        public const string ASSET_NAME = "Online Check PRO";

        /// <summary>Short name of the asset.</summary>
        public const string ASSET_NAME_SHORT = "OC PRO";
        
        /// <summary>Version of the asset.</summary>
        public const string ASSET_VERSION = "2019.1.4";

        /// <summary>Build number of the asset.</summary>
        public const int ASSET_BUILD = 20190512;

        /// <summary>Create date of the asset (YYYY, MM, DD).</summary>
        public static readonly System.DateTime ASSET_CREATED = new System.DateTime(2017, 5, 4);

        /// <summary>Change date of the asset (YYYY, MM, DD).</summary>
        public static readonly System.DateTime ASSET_CHANGED = new System.DateTime(2019, 5, 12);

        /// <summary>URL of the PRO asset in UAS.</summary>
        public const string ASSET_PRO_URL = "https://assetstore.unity.com/packages/slug/74688?aid=1011lNGT";

        /// <summary>URL of the 2019 asset in UAS.</summary>
        public const string ASSET_2019_URL = "https://www.assetstore.unity3d.com/#!/content/74688?aid=1011lNGT"; //TODO TBD!

        /// <summary>URL for update-checks of the asset</summary>
        public const string ASSET_UPDATE_CHECK_URL = "https://www.crosstales.com/media/assets/onlinecheck_versions.txt";
        //public const string ASSET_UPDATE_CHECK_URL = "https://www.crosstales.com/media/assets/test/onlinecheck_versions_test.txt";

        /// <summary>Contact to the owner of the asset.</summary>
        public const string ASSET_CONTACT = "onlinecheck@crosstales.com";

        /// <summary>URL of the asset manual.</summary>
        public const string ASSET_MANUAL_URL = "https://www.crosstales.com/media/data/assets/OnlineCheck/OnlineCheck-doc.pdf";

        /// <summary>URL of the asset API.</summary>
        public const string ASSET_API_URL = "https://crosstales.com/media/data/assets/OnlineCheck/api";

        /// <summary>URL of the asset forum.</summary>
        public const string ASSET_FORUM_URL = "https://goo.gl/9XH5Ke";

        /// <summary>URL of the asset in crosstales.</summary>
        public const string ASSET_WEB_URL = "https://www.crosstales.com/en/portfolio/OnlineCheck/";

        /// <summary>URL of the promotion video of the asset (Youtube).</summary>
        public const string ASSET_VIDEO_PROMO = "https://youtu.be/pPvKE-eyxV4?list=PLgtonIOr6Tb41XTMeeZ836tjHlKgOO84S";

        /// <summary>URL of the tutorial video of the asset (Youtube).</summary>
        public const string ASSET_VIDEO_TUTORIAL = "https://youtu.be/bNdafUNcs68?list=PLgtonIOr6Tb41XTMeeZ836tjHlKgOO84S";

        // Keys for the configuration of the asset
        public const string KEY_PREFIX = "ONLINECHECK_CFG_";
        //public const string KEY_ASSET_PATH = KEY_PREFIX + "ASSET_PATH";
        public const string KEY_DEBUG = KEY_PREFIX + "DEBUG";
        public const string KEY_DONT_DESTROY_ON_LOAD = KEY_PREFIX + "DONT_DESTROY_ON_LOAD";
        public const string KEY_ENSURE_NAME = KEY_PREFIX + "ENSURE_NAME";

        // Default values
        public const bool DEFAULT_ENSURE_NAME = true;

        /// <summary>OnlineCheck prefab scene name.</summary>
        public const string ONLINECHECK_SCENE_OBJECT_NAME = "OnlineCheck";

        /// <summary>Proxy prefab scene name.</summary>
        public const string PROXY_SCENE_OBJECT_NAME = "Proxy";

        #endregion
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)