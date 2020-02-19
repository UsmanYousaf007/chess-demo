namespace HUFEXT.PackageManager.Editor.Utils
{
    internal static partial class Registry
    {
        private const string PACKAGE_MANAGER_PATH = "Assets/HUFEXT/PackageManager/";

        internal static class Cache
        {
            internal const string CACHE_DIRECTORY = "Assets/HUF/.cache";
            internal const string TOKEN_FILE_PATH = "Assets/HUF/auth-token.huf";
        }
        
        internal static class Urls
        {
            internal const string CONTACT_SUPPORT_URL = "https://contactus.huuugegames.com/?tag=huf&userid=";
        }
        
        internal static class Keys
        {
            internal const string CACHE_ENTRIES_KEY = "HUF.PackageManager.Cache.Entries";
            internal const string PACKAGES_CACHE_LIST_KEY = "HUF.PackageManager.Cache.Packages";
            internal const string NEXT_FETCH_DATE = "HUF.PackageManager.Packages.NextFetchDate";
            internal const string PACKAGE_MANAGER_STATUS = "HUF.PackageManager.Status";
            internal const string PACKAGE_MANAGER_DIRTY_FLAG = "HUF.PackageManager.DirtyFlag";
            internal const string PACKAGE_MANAGER_FORCE_CLOSE = "HUF.PackageManager.ForceClose";
            internal const string PACKAGE_MANAGER_UPDATES = "HUF.PackageManager.PackagesToUpdate";
            internal const string PACKAGE_INSTALLER_LOCK = "HUF.PackageInstaller.Lock";
        }

        internal static class MenuItems
        {
            internal const string POLICY = "HUF/Package Manager/Debug/Show Policy";
            internal const string PACKAGE_MANAGER = "HUF/Package Manager/Open Package Manager";
        }

        internal static class Resources
        {
            private const string ICON_PREFIX = PACKAGE_MANAGER_PATH + "Editor/Resources/Icons/";
            internal const string LICENSE_TEXT = PACKAGE_MANAGER_PATH + "LICENSE.md";
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
    }
}
