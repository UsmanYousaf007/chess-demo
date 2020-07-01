using System;
using System.Collections.Generic;
using System.IO;
using HUF.Utils.Editor.BuildSupport.AssetsBuilder;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HUF.Utils.Editor.BuildSupport
{
    public abstract class BaseDummyPreprocessBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public abstract bool Enabled { get; }
        public abstract IEnumerable<string> DirectoriesToHide { get; }
        public abstract HLogPrefix LogPrefix { get; }

        public void HandleBuildError() => OnPostprocessBuild( null );

        public void OnPreprocessBuild( BuildReport report )
        {
            if ( !Enabled )
                return;

            HUFBuildAssetsResolver.OnBuildError += HandleBuildError;
            HideFolders( true );
            AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
        }

        public void OnPostprocessBuild( BuildReport report )
        {
            if ( !Enabled )
                return;

            HideFolders( false );
            AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
            HUFBuildAssetsResolver.OnBuildError -= HandleBuildError;
        }

        public void HideFolders( bool hide )
        {
            foreach ( string dir in DirectoriesToHide )
            {
                string path = Path.Combine( Application.dataPath, dir );
                string oldPath = hide ? path : $"{path}~";
                string newPath = hide ? $"{path}~" : path;
                MoveDirectory( oldPath, newPath );
            }
        }

        void MoveDirectory( string oldPath, string newPath )
        {
            try
            {
                var directory = new DirectoryInfo( oldPath );
                directory.MoveTo( newPath );
            }
            catch ( Exception e )
            {
                HLog.LogError( LogPrefix, $"Exception while moving directory from {oldPath} to {newPath}: {e}" );
            }
        }
    }
}