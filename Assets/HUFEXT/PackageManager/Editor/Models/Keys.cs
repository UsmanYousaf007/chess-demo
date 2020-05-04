namespace HUFEXT.PackageManager.Editor.Models
{
    internal static class Keys
    {
        internal const string PACKAGE_MANAGER_PATH = "Assets/HUFEXT/PackageManager/";
        internal const int AUTO_FETCH_DELAY = 1 * 60 * 60;
        
        internal const string CACHE_DIRECTORY = "Assets/HUF/.cache";
        internal const string FILE_PACKAGE_LOCK = "Assets/HUF/package-lock.huf";

        internal const string GOOGLE_SCOPED_REGISTRY_KEY = "https://unityregistry-pa.googleapis.com";
        internal const string HELPSHIFT_URL_KEY = "https://contactus.huuugegames.com/?tag=huf&userid=";

        internal const string CACHE_AUTH_TOKEN_KEY = "HUF.PackageManager.Token";
        
        // Key used for store registered in cache items list.
        internal const string CACHE_DATA_KEY = "HUF.PackageManager.Cache.Data";
        
        // Key used for store current settings of Package Manager view like sorting options, settings flags etc.
        internal const string CACHE_PACKAGE_MANAGER_STATE_KEY = "HUF.PackageManager.Cache.State";
        
        // Key used for store current list of packages (local and remote).
        internal const string CACHE_PACKAGE_REGISTRY_KEY = "HUF.PackageManager.Cache.PackageRegistry";
        internal const string CACHE_PACKAGE_LOCAL_REGISTRY_KEY = "HUF.PackageManager.Cache.LocalPackageRegistry";
        internal const string CACHE_PACKAGE_REMOTE_REGISTRY_KEY = "HUF.PackageManager.Cache.LastRemotePackageRegistry";
        internal const string CACHE_PACKAGE_INSTALL_IN_PROGRESS = "HUF.PackageManager.Cache.InstallInProgress";
        internal const string CACHE_PACKAGES_UPDATE_IN_PROGRESS = "HUF.PackageManager.Cache.UpdateInProgress";
        internal const string CACHE_PACKAGES_CURRENT_CHANNEL = "HUF.PackageManager.Cache.Channel";
        
        // Key used for indicate that package import was completed (cleared when import begin and set when is completed).
        internal const string CACHE_LAST_IMPORTED_PACKAGE_NAME_KEY = "HUF.PackageManager.Cache.LastImportedPackage";
        
        // Key used for indicate when last packages fetch was performed.
        // Is set when Package Registry is merging packages.
        internal const string CACHE_LAST_FETCH_TIME_KEY = "HUF.PackageManager.Cache.LastFetchTime";

        //internal const string PACKAGE_MANAGER_CURRENT_CHANNEL = "HUF.PackageManager.Channel";
        internal const string PACKAGE_MANAGER_DEV_ENVIRONMENT = "HUF.PackageManager.Debug.Environment";
        internal const string PACKAGE_MANAGER_DEBUG_LOGS = "HUF.PackageManager.Debug";
        internal const string PACKAGE_MANAGER_DIRTY_FLAG = "HUF.PackageManager.DirtyFlag";
        internal const string PACKAGE_MANAGER_FORCE_CLOSE = "HUF.PackageManager.ForceClose";
        internal const string PACKAGE_MANAGER_LAST_IMPORT_FAILED = "HUF.PackageManager.LastImportFailed";
        internal const string PACKAGE_MANAGER_LAST_FETCH_KEY = "HUF.PackageManager.LastFetch";
        internal const string PACKAGE_MANAGER_NEXT_AUTO_FETCH = "HUF.PackageManager.NextAutoFetch";

        internal const string MENU_ITEM_OPEN_PACKAGE_MANAGER = "HUF/Package Manager";

        internal static class Filesystem
        {
            internal const string ASSETS_DIR = "Assets";
            internal const string CONFIG_EXTENSION = "config.json";
            internal const string MANIFEST_EXTENSION = "package.json";
            internal const string META_EXTENSION = ".meta";
            internal const string UNITY_PACKAGE_EXTENSION = ".unitypackage";
            internal const string UNITY_PACKAGES_MANIFEST_FILE = "Packages/manifest.json";
        }
        
        internal static class Views
        {
            internal static class Policy
            {
                internal const string LICENSE_PATH = PACKAGE_MANAGER_PATH + "LICENSE.md";
                internal const string TITLE = "Huuuge Unity Framework - Accept license and import our package manager";
                internal const string LICENSE = "Please read and accept our license:";
                internal const string CHECKBOX = " I accept the terms and conditions";
                internal const string BUTTON = "Proceed";
                internal const string VALIDATE = "Validating...";
                internal const string ERROR_TITLE = "Error...";
                internal const string ERROR_DESC = "You must accept our license and enter valid credentials.";
                internal const string ERROR_BUTTON = "OK";
            }

            internal static class Update
            {
                internal const string TITLE = "Huuuge Unity Framework - Update Packages";
                internal const string SELECT = "Select packages to update:";
            }
        }
        
        internal static class Resources
        {
            private const string ICON_PREFIX = PACKAGE_MANAGER_PATH + "Editor/Resources/Icons/";
            internal const string HUF_LOGO = PACKAGE_MANAGER_PATH + "Editor/Resources/Common/huf_logo.png";

            internal static class Icons
            {
                internal const string WINDOW = ICON_PREFIX + "huf_icon.png";
                internal const string INSTALLED = ICON_PREFIX + "huf_pm_installed.png";
                internal const string NOT_INSTALLED = ICON_PREFIX + "huf_pm_not_installed.png";
                internal const string UPDATE = ICON_PREFIX + "huf_pm_upgrade.png";
                internal const string ERROR = ICON_PREFIX + "huf_pm_error.png";
                internal const string FORCE_UPDATE = ICON_PREFIX + "huf_pm_force_upgrade.png";
                internal const string MIGRATION = ICON_PREFIX + "huf_pm_migration.png";
                internal const string CONFLICT = ICON_PREFIX + "huf_pm_conflict.png";
            }
        }

        internal static class Routing
        {
            public const int TIMEOUT = 999;
            public const string STABLE_CHANNEL = "stable";
            public const string PREVIEW_CHANNEL = "preview";
            public const string EXPERIMENTAL_CHANNEL = "experimental";
        
            public static class Tag
            {
                public const string SCOPE = "{scope}";
                public const string CHANNEL = "{channel}";
                public const string PACKAGE = "{package}";
                public const string VERSION = "{version}";
            }
        
            public static class API
            {
                const string URL = "https://huf-packages.wt-prod.com";
                public const string GLOBAL_CONFIG = URL + "/v1/config";
                public const string SCOPES = URL + "/v1/scopes";
                public const string CONFIGS = URL + "/v1/scopes/{scope}/channels/{channel}/packages/configs";
                public const string PACKAGE_CONFIG = URL + "/v1/scopes/{scope}/channels/{channel}/packages/{package}/config";
                public const string PACKAGE_VERSIONS = URL + "/v1/scopes/{scope}/channels/{channel}/packages/{package}/versions";
                public const string PACKAGE_MANIFEST = URL + "/v1/scopes/{scope}/channels/{channel}/packages/{package}/versions/{version}/manifest";
                public const string PACKAGE_LINK = URL + "/v1/scopes/{scope}/channels/{channel}/packages/{package}/versions/{version}/download-link";
                public const string LATEST_PACKAGES = URL + "/v1/scopes/channels/{channel}/packages/latest-versions";
            }
        }
    }
}