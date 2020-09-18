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
    public abstract class HideFilesDuringBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => -100;
        public abstract bool Enabled { get; }
        public abstract IEnumerable<string> FilesAndDirectoriesToHide { get; }
        public abstract HLogPrefix LogPrefix { get; }

        public void HandleBuildError() => OnPostprocessBuild( null );

        public void OnPreprocessBuild( BuildReport report )
        {
            if ( !Enabled )
                return;

            HUFBuildAssetsResolver.OnBuildError += HandleBuildError;
            HideShowFilesAndDirectory( true );
            AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
        }

        public void OnPostprocessBuild( BuildReport report )
        {
            if ( !Enabled )
                return;

            HideShowFilesAndDirectory( false );
            AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
            HUFBuildAssetsResolver.OnBuildError -= HandleBuildError;
        }

        public virtual void HideShowFilesAndDirectory( bool hide )
        {
            foreach ( string source in FilesAndDirectoriesToHide )
            {
                HideShowFileOrDirectory( hide, source );
                HideShowFileOrDirectory( hide, source + HUFBuildAssetsResolver.META_FILES );
            }
        }

        void HideShowFileOrDirectory( bool hide, string dir )
        {
            string path = Path.Combine( Application.dataPath, dir );
            string oldPath = hide ? path : $"{path}~";
            string newPath = hide ? $"{path}~" : path;
            MoveFileOrDirectory( oldPath, newPath );
        }

        void MoveFileOrDirectory( string oldPath, string newPath )
        {
            try
            {
                FileUtil.MoveFileOrDirectory( oldPath, newPath );
            }
            catch ( Exception e )
            {
                HLog.LogError( LogPrefix, $"Exception while moving directory from {oldPath} to {newPath}: {e}" );
            }
        }
    }
}