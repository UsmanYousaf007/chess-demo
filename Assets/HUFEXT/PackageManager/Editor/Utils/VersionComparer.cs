using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Utils
{
    internal static class VersionComparer
    {
        static readonly Dictionary<string, int> tags = new Dictionary<string, int>()
        {
            {"develop", 11},
            {"preview", 10},
            {"experimental", 9},
            {"unknown", 0}
        };

        public static bool Compare( string v1, string op, string v2, bool ignoreTags = false )
        {
            var result = Compare( v1, v2, ignoreTags );

            switch ( op )
            {
                case "=": return result == 0;
                case ">": return result == 1;
                case "<": return result == -1;
                case ">=": return result == 0 || result == 1;
                case "<=": return result == 0 || result == -1;
                default:
                {
                    Common.Log( $"Unable to compare {v1} and {v2}. Unknown operator: {op}." );
                    return false;
                }
            }
        }

        public static int Compare( string v1, string v2, bool ignoreTags = false, bool comparingUnityVersions = false )
        {
            if ( string.IsNullOrEmpty( v1 ) || string.IsNullOrEmpty( v2 ) )
            {
                Common.Log( $"Incorrect input parameters {v1} or {v2}." );
                return 0;
            }

            var v1version = v1.Split( '-' );
            var v2version = v2.Split( '-' );
            var v1HasTag = v1version.Length > 1 && v1version[1] != "develop";
            var v2HasTag = v2version.Length > 1 && v2version[1] != "develop";
            var v1arr = Array.ConvertAll( v1version[0].Split( '.' ), int.Parse );
            var v2arr = Array.ConvertAll( v2version[0].Split( '.' ), int.Parse );

            if ( v1arr.Length != v2arr.Length && !comparingUnityVersions )
            {
                Common.Log( $"Incorrect version length {v1arr.Length} != {v2arr.Length}." );
                return v1arr.Length > v2arr.Length ? -1 : 1;
            }

            int maxVersionArrayLength = Mathf.Max( v1arr.Length, v2arr.Length );
            var result = 0;

            for ( var i = 0; i < maxVersionArrayLength; ++i )
            {
                if ( i >= v2arr.Length )
                {
                    result = -1;
                    break;
                }

                if ( i >= v1arr.Length || v1arr[i] > v2arr[i] )
                {
                    result = 1;
                    break;
                }

                if ( v1arr[i] < v2arr[i] )
                {
                    result = -1;
                    break;
                }
            }

            if ( ignoreTags || ( !v1HasTag && !v2HasTag ) )
            {
                return result;
            }

            if ( v1HasTag && !v2HasTag )
            {
                return -1;
            }

            if ( !v1HasTag )
            {
                return 1;
            }

            tags.TryGetValue( v1version[1], out var v1tag );
            tags.TryGetValue( v2version[1], out var v2tag );

            if ( v1tag == v2tag )
            {
                return result;
            }

            return v1tag > v2tag ? 1 : -1;
        }

#if HUF_TESTS
        [MenuItem( "HUF/Debug/Version comparing test" )]
        public static void TestComparision()
        {
            List<string> versions = new List<string>()
            {
                "1.0.0",
                "1.0.0-preview",
                "1.0.0-experimental",
                "1.0.0-develop",
                "0.9.0",
                "0.9.0-preview",
                "0.9.0-experimental",
                "1.1.0",
                "1.1.0-preview",
                "1.1.0-experimental",
                "2.0.0",
                "2.0.0-preview",
                "2.0.0-experimental",
                "2.0.0-rc.1",
            };
            Debug.Log( "Compare:" );

            foreach ( var v1 in versions )
            {
                foreach ( var v2 in versions )
                {
                    Debug.Log( $"Test: {v1} : {v2} => {Compare( v1, v2 )}" );
                }
            }

            foreach ( var v1 in versions )
            {
                foreach ( var v2 in versions )
                {
                    Debug.Log( $"Test: {v1} : {v2} => {Compare( v1, v2, true )}" );
                }
            }
        }
#endif
    }
}