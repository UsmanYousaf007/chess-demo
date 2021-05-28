using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Facebook.Unity.Editor;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEngine;

namespace HUF.AdsIronSourceMediation.Editor
{
    public class IronSourceSDKFixer : AssetPostprocessor
    {
        const string FILE_PATERN = "/IronSource/Editor/";
        const string ASSETS = "Assets";

        private static readonly Dictionary<string, string> toReplace = new Dictionary<string, string>
        {
            {"Assets/IronSource", "Assets/HUF/AdsIronSourceMediation/SDK/IronSource"},
            {"Application.dataPath + \"/IronSource/Editor/\"", "Application.dataPath + \"/HUF/AdsIronSourceMediation/SDK/IronSource/Editor/\""},
        };

        static readonly HLogPrefix logPrefix =
            new HLogPrefix( HAdsIronSourceMediation.logPrefix, nameof(IronSourceSDKFixer) );

        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            var filePath = imported.FirstOrDefault( asset => asset.Contains( FILE_PATERN ) );
            var file = new FileInfo( Application.dataPath.Replace( ASSETS, "" ) + filePath );

            if (!file.Exists) return;
            
            var text = File.ReadAllText( file.FullName );
            text = toReplace.Aggregate(text, (current, replaceData) => current.Replace(replaceData.Key, replaceData.Value));
            File.WriteAllText( file.FullName, text );
        }
    }
}