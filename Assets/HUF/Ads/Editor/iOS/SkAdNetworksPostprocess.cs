#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.IO;
using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Logging;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

public class SkAdNetworksPostprocess : IPostprocessBuildWithReport
{
    const string PLIST_FOLDER_PATH = "/Info.plist";
    public static readonly HLogPrefix logPrefix = new HLogPrefix( HAds.logPrefix, nameof(SkAdNetworksPostprocess) );
    public int callbackOrder => 0;

    const string SKAD_NETWORK_IDENTIFIER = "SKAdNetworkIdentifier";
    const string SKAD_NETWORK_ITEMS = "SKAdNetworkItems";
    const string SKAD_NETWORKS_FILE = "Assets/HUF/Ads/Editor/skadnetworks.txt";

    public void OnPostprocessBuild( BuildReport report )
    {
        UpdateInfoPlistWithSkAdNetworkIds( report.summary.outputPath );
    }

    static void UpdateInfoPlistWithSkAdNetworkIds( string plistFolderPath )
    {
        var infoPlistPath = plistFolderPath + PLIST_FOLDER_PATH;
        var plist = new PlistDocument();
        plist.ReadFromString( File.ReadAllText( infoPlistPath ) );
        var root = plist.root;

        if ( root == null )
        {
            HLog.LogWarning( logPrefix, "Unable to parse info.plist.  Unable to add SkAdNetwork Identifiers." );
            return;
        }

        if ( !root.values?.ContainsKey( SKAD_NETWORK_ITEMS ) ?? false )
        {
            root.CreateArray( SKAD_NETWORK_ITEMS );
        }

        var adNetworkItems = root[SKAD_NETWORK_ITEMS].AsArray();

        if ( adNetworkItems == null )
        {
            HLog.LogWarning( logPrefix,
                "Unable to modify existing info.plist.  Unable to add SkAdNetwork Identifiers." );
            return;
        }

        foreach ( var adNetworkId in GetSkAdNetworksIds() )
        {
            adNetworkItems.AddDict().SetString( SKAD_NETWORK_IDENTIFIER, adNetworkId );
        }

        File.WriteAllText( infoPlistPath, plist.WriteToString() );
    }

    static IEnumerable<string> GetSkAdNetworksIds()
    {
        string path = PathUtils.GetFullPath( SKAD_NETWORKS_FILE );

        if ( !File.Exists( path ) )
        {
            HLog.LogError( logPrefix, $"Cannot find file at path: {path}" );
            return new string[] { };
        }

        return File.ReadAllLines( path );
    }
}
#endif
