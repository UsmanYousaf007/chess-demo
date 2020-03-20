using System.Collections.Generic;
using System.Linq;

namespace HUFEXT.PackageManager.Editor.Utils
{
    /*internal static class VersionComparer
    {
        public enum Result
        {
            PackageIsNewer,
            PackageIsOlder,
            PackagesAreEqual,
            CompareError
        }
        
        public static bool IsPackageNewerThan( string first, string second )
        {
            return Compare( first, second ) == Result.PackageIsNewer;
        }

        public static bool ArePackagesEqual( string first, string second )
        {
            return Compare( first, second ) == Result.PackagesAreEqual;
        }

        public static Result Compare( string versionA, string versionB )
        {
            var a = ParseVersionString( versionA );
            var b = ParseVersionString( versionB );

            if ( a == null )
            {
                return Result.CompareError;
            }

            if ( b == null )
            {
                return Result.PackageIsNewer;
            }
            
            return CompareVersions( a, b );
        }

        private static List<int> ParseVersionString( string version )
        {
            return !string.IsNullOrEmpty( version ) ? 
                       version.Split( '.' ).Select( int.Parse ).ToList() 
                       : null;
        }

        private static Result CompareVersions( IReadOnlyList<int> a, IReadOnlyList<int> b )
        {
            if ( a.Count != b.Count )
            {
                return Result.CompareError;
            }
            
            for ( var i = 0; i < a.Count; ++i )
            {
                if ( a[i] > b[i] )
                {
                    return Result.PackageIsNewer;
                }

                if ( a[i] < b[i] )
                {
                    return Result.PackageIsOlder;
                }
            }

            return Result.PackagesAreEqual;
        }
    }*/
}
