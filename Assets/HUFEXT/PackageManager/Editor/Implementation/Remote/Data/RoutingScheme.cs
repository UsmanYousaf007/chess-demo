using HUFEXT.PackageManager.Editor.Implementation.Huuuge.Utils;

namespace HUFEXT.PackageManager.Editor.Implementation.Remote.Data
{
    public static class RoutingScheme
    {
        public static string URL => "https://huf-packages.wt-prod.com";
        public static int TimeoutInSeconds => 300;

        public static class Scope
        {
            public static string Tag => "{scope}";
            public static string Public => "public";
            public static string Private => "private";
        }

        public static class Channel
        {
            public static string Tag => "{channel}";
            public static string Stable => "stable";
            public static string Preview => "preview";
        }

        public static class Package
        {
            public static string Tag => "{package}";
        }

        public static class Version
        {
            public static string Tag => "{version}";
        }

        public static class API
        {
            private static string CommonPrefix => URL + "/v1/scopes/{scope}/channels/{channel}";
            
            public static string GlobalConfig => URL + "/v1/config";
            public static string AllowedScopes => URL + "/v1/scopes";
            public static string Packages => CommonPrefix + "/packages";
            public static string Configs => Packages + "/configs";
            public static string PackageConfig => CommonPrefix + "/packages/{package}/config";
            public static string PackageVersions => CommonPrefix + "/packages/{package}/versions";
            public static string PackageManifest => PackageVersions + "/{version}/manifest";
            public static string UnityPackageLink => PackageVersions + "/{version}/download-link";
            public static string LatestPackagesList => URL + "/v1/scopes/channels/{channel}/packages/latest-versions";
        }
        
        public static RouteBuilder CreateRoute( string path )
        {
            return new RouteBuilder( path );
        }
    }
}
