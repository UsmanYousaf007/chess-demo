#if UNITY_EDITOR
namespace Crosstales.OnlineCheck.EditorUtil
{
    /// <summary>Collected editor constants of very general utility for the asset.</summary>
    public static class EditorConstants
    {

        #region Constant variables

        // Keys for the configuration of the asset
        public const string KEY_UPDATE_CHECK = Util.Constants.KEY_PREFIX + "UPDATE_CHECK";
        //public const string KEY_REMINDER_CHECK = Util.Constants.KEY_PREFIX + "REMINDER_CHECK";
        public const string KEY_TRACER = Util.Constants.KEY_PREFIX + "TRACER";
        public const string KEY_PREFAB_AUTOLOAD = Util.Constants.KEY_PREFIX + "PREFAB_AUTOLOAD";

        public const string KEY_HIERARCHY_ICON = Util.Constants.KEY_PREFIX + "HIERARCHY_ICON";

        public const string KEY_UPDATE_DATE = Util.Constants.KEY_PREFIX + "UPDATE_DATE";

        public const string KEY_REMINDER_DATE = Util.Constants.KEY_PREFIX + "REMINDER_DATE";
        public const string KEY_REMINDER_COUNT = Util.Constants.KEY_PREFIX + "REMINDER_COUNT";

        public const string KEY_LAUNCH = Util.Constants.KEY_PREFIX + "LAUNCH";

        public const string KEY_TRACER_DATE = Util.Constants.KEY_PREFIX + "TRACER_DATE";

        // Default values
        public const string DEFAULT_ASSET_PATH = "/Plugins/crosstales/OnlineCheck/";
        public const bool DEFAULT_UPDATE_CHECK = false;
        //public const bool DEFAULT_UPDATE_OPEN_UAS = false;
        //public const bool DEFAULT_REMINDER_CHECK = true;
        public const bool DEFAULT_TRACER = true;
        public const bool DEFAULT_PREFAB_AUTOLOAD = false;
        public const bool DEFAULT_HIERARCHY_ICON = false;

        #endregion


        #region Changeable variables

        /// <summary>Sub-path to the prefabs.</summary>
        public static string PREFAB_SUBPATH = "Prefabs/";

        #endregion


        #region Properties

        /// <summary>Returns the URL of the asset in UAS.</summary>
        /// <returns>The URL of the asset in UAS.</returns>
        public static string ASSET_URL
        {
            get
            {

                return Util.Constants.ASSET_PRO_URL;
            }
        }

        /// <summary>Returns the ID of the asset in UAS.</summary>
        /// <returns>The ID of the asset in UAS.</returns>
        public static string ASSET_ID
        {
            get
            {
                return "74688";
            }
        }

        /// <summary>Returns the UID of the asset.</summary>
        /// <returns>The UID of the asset.</returns>
        public static System.Guid ASSET_UID
        {
            get
            {
                return new System.Guid("8d9c788b-065f-410e-ba67-f44e997d1a9c");
            }
        }

        #endregion

    }
}
#endif
// © 2017-2019 crosstales LLC (https://www.crosstales.com)