#if UNITY_IOS
using System.Linq;
using HUF.Utils.Editor;
using HUF.Utils.Runtime.Configs.API;
using HUFEXT.CrossPromo.Runtime.Implementation;
using HUF.Utils.BuildSupport.Editor.iOS;
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace HUFEXT.CrossPromo.Editor
{
    [UsedImplicitly]
    public class iOSUrlSchemaFiller : iOSPlistBaseFixer
    {
        const string LOG_PREFIX = nameof(iOSUrlSchemaFiller);
        private const string HUUUGE_GAMES_DEFAULT_KEY = "huuuge-game-";
        public override int callbackOrder => 20;

        protected override bool Process( PlistElementDict rootDict, string projectPath )
        {
            var configs = Resources.LoadAll<AbstractConfig>("")
                .Select(x => x as CrossPromoLocalConfig)
                .Where(x => x != null).ToList();

            if ( configs.Count == 0 )
            {
                Debug.LogWarning( $"[{LOG_PREFIX}] Can't find any config of type: {nameof(CrossPromoLocalConfig)} " +
                                  "in your project" );
                return false;
            }

            if ( configs.Count > 1 )
            {
                Debug.LogWarning(
                    $"[{LOG_PREFIX}] There is more than one {nameof(CrossPromoLocalConfig)} in your project" );
            }

            var config = configs[0];
            PlistElementArray schemesArray = rootDict.CreateArray( "LSApplicationQueriesSchemes" );

            for ( int i = 0; i < 50; i++ )
            {
                schemesArray.AddString( $"{HUUUGE_GAMES_DEFAULT_KEY}{i}" );
            }

            if ( config.EnabledCustomURLSchemesiOS != null )
            {
                for ( int i = 0; i < config.EnabledCustomURLSchemesiOS.Length; i++ )
                {
                    schemesArray.AddString( config.EnabledCustomURLSchemesiOS[i] );
                }
            }

            if ( config.GameCustomURLSchemesiOS != null )
            {
                PlistElementArray bundleURLTypes;

                if ( rootDict.values.ContainsKey( "CFBundleURLTypes" ) == false )
                {
                    bundleURLTypes = rootDict.CreateArray( "CFBundleURLTypes" );
                    PlistElementDict dict = bundleURLTypes.AddDict();
                    dict.SetString( "CFBundleTypeRole", "Editor" );
                    dict.SetString( "CFBundleURLName", Application.identifier );
                    dict.CreateArray( "CFBundleURLSchemes" );
                }
                else
                {
                    bundleURLTypes = rootDict.values["CFBundleURLTypes"].AsArray();
                }

                for ( int i = 0; i < bundleURLTypes.values.Count; i++ )
                {
                    PlistElementDict dict = bundleURLTypes.values[i].AsDict();

                    if ( dict.values.ContainsKey( "CFBundleURLName" ) &&
                         dict.values["CFBundleURLName"].AsString() == Application.identifier )
                    {
                        PlistElementArray schemeArrayDict = dict.values["CFBundleURLSchemes"].AsArray();

                        for ( int j = 0; j < config.GameCustomURLSchemesiOS.Length; j++ )
                        {
                            schemeArrayDict.AddString( config.GameCustomURLSchemesiOS[j] );
                        }
                    }
                }
            }

            return true;
        }
    }
}
#endif