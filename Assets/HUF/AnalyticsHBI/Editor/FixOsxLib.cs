#if UNITY_EDITOR_OSX && UNITY_2018_4
using System.IO;
using UnityEditor;

[InitializeOnLoad]
public static class FixOsxLib
{
    static readonly string osxLib = "Assets/HUF/AnalyticsHBI/Plugins/HBI/Native/macOS/hbi-wrapper-native";
    static FixOsxLib()
    {
        if( File.Exists( $"{osxLib}.dylib" ) )
            File.Move( $"{osxLib}.dylib", $"{osxLib}.bundle" );
    }
}
#endif
