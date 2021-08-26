using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HUFEXT.PackageManager.Editor.Commands.Data;
using HUFEXT.PackageManager.Editor.Models;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    public class RefreshPackagesCommand : Core.Command.Base
    {
        public bool downloadLatest = true;
        public bool downloadInTheBackground = false;

        public override void Execute()
        {
            if ( !downloadInTheBackground )
            {
                Core.Packages.UpdateInProgress = true;
            }

            Core.Command.Execute( new GetLocalPackagesCommand() );

            if ( !downloadLatest )
            {
                MergePackages( Core.Packages.Local, Core.Packages.Remote );
                return;
            }

            Core.Command.Execute( new Data.GetUnityPackagesCommand
            {
                OnComplete = ( unityPackagesResult, unityPackageSerializedData ) =>
                {
                    Core.Command.Execute( new Data.GetRemotePackagesCommand()
                    {
                        OnComplete = ( result, serializedData ) =>
                        {
                            Core.Registry.Save( Models.Keys.PACKAGE_MANAGER_LAST_FETCH_KEY,
                                DateTime.Now.ToString( CultureInfo.InvariantCulture ) );
                            var next = Utils.Common.GetTimestamp( Models.Keys.AUTO_FETCH_DELAY );
                            Core.Registry.Save( Models.Keys.PACKAGE_MANAGER_NEXT_AUTO_FETCH, next );
                            MergePackages( Core.Packages.Local, Core.Packages.Remote );
                        }
                    } );
                }
            } );
        }

        protected override void Complete( bool result, string serializedData = "" )
        {
            Core.Packages.UpdateInProgress = false;
            AssetDatabase.Refresh();
            base.Complete( result, serializedData );
        }

        void MergePackages( List<Models.PackageManifest> local, List<Models.PackageManifest> remote )
        {
            Core.Packages.Clear();
            var packages = new List<Models.PackageManifest>();
            packages.AddRange( local );
            DetectVendorPackages();
            CheckMinimumVersionNeededInOptionalDependencies();
            int newestRollout = 0;

            foreach ( var package in remote.Where( package =>
                !string.IsNullOrEmpty( package.huf.rollout ) && package.huf.rollout.All( char.IsDigit ) ) )
            {
                if ( int.TryParse( package.huf.rollout, out int rollout ) )
                {
                    if ( rollout > newestRollout )
                        newestRollout = rollout;
                }
                else
                    Utils.Common.Log( $"Wrong rollout string {package.name}: {package.huf.rollout}" );
            }

            foreach ( var remotePackage in remote )
            {
                var index = packages.FindIndex( package => package.name == remotePackage.name ||
                                                           package.huf.path == remotePackage.huf.path );

                if ( index == -1 )
                {
                    packages.Add( remotePackage );
                    continue;
                }

                packages[index].huf.isLocal = false;
                packages[index].huf.config = remotePackage.huf.config;
                packages[index].huf.channel = remotePackage.huf.channel;
                packages[index].huf.scope = remotePackage.huf.scope;
                packages[index].huf.scopes = remotePackage.huf.scopes;
                var nameIsEmpty = packages[index].name == string.Empty;

                if ( packages[index].huf.status == Models.PackageStatus.Git )
                {
                    if ( nameIsEmpty )
                    {
                        packages[index].name = remotePackage.name;
                    }

                    switch ( Utils.VersionComparer.Compare( remotePackage.version, packages[index].version, true ) )
                    {
                        case 0:
                            packages[index].huf.status = Models.PackageStatus.GitUpdate;
                            break;
                        case 1:
                            packages[index].huf.status = Models.PackageStatus.GitError;
                            break;
                    }

                    continue;
                }

                if ( nameIsEmpty )
                {
                    packages[index] = remotePackage;
                    packages[index].huf.status = Models.PackageStatus.Migration;
                    packages[index].version = "0.0.0-unknown";
                }
                else if ( remotePackage.SupportsCurrentUnityVersion )
                {
                    var compare = Utils.VersionComparer.Compare( remotePackage.version, packages[index].version, true );

                    switch ( compare )
                    {
                        case 0:
                        {
                            var compareTag =
                                Utils.VersionComparer.Compare( remotePackage.version, packages[index].version );

                            if ( compareTag == 1 )
                            {
                                packages[index].huf.status = Models.PackageStatus.UpdateAvailable;
                            }
                            else
                            {
                                packages[index].huf.rollout = newestRollout.ToString();
                            }

                            break;
                        }
                        case 1:
                        {
                            var compareMin = Utils.VersionComparer.Compare( packages[index].version,
                                remotePackage.huf.config.minimumVersion,
                                true );

                            packages[index].huf.status = ( compareMin == -1 )
                                ? Models.PackageStatus.ForceUpdate
                                : Models.PackageStatus.UpdateAvailable;
                            break;
                        }
                    }
                }
            }

            CheckIfPackagesAreSupportedByCurrentUnity();
            SetMissingPackageNames();
            AddUnityPackages();
            Core.Packages.Data = packages.OrderBy( package => package.name ).ToList();
            Complete( true );

            void DetectVendorPackages()
            {
                for ( var i = 0; i < packages.Count; ++i )
                {
                    if ( !packages[i].IsHufPackage )
                    {
                        packages[i].huf.rollout = Models.Rollout.NOT_HUF_LABEL;
                    }
                }
            }

            void SetMissingPackageNames()
            {
                for ( var i = 0; i < packages.Count; ++i )
                {
                    if ( string.IsNullOrEmpty( packages[i].name ) )
                    {
                        packages[i].name = $"com.unknown.{packages[i].displayName.ToLower()}";
                    }
                }
            }

            void AddUnityPackages()
            {
                var unityPackages = Core.Packages.Unity;
                var minimumVersionsDictionary = new Dictionary<string, Dependency>();

                foreach ( var localPackage in local )
                {
                    foreach ( var str in localPackage.huf.dependencies )
                    {
                        var dependency = new Dependency( str );

                        if ( !dependency.IsHufPackage )
                        {
                            if ( minimumVersionsDictionary.ContainsKey( dependency.name ) )
                            {
                                var currentDependency = minimumVersionsDictionary[dependency.name];

                                if ( dependency.IsVersionHigherTo( currentDependency ) )
                                    minimumVersionsDictionary[dependency.name] = dependency;
                            }
                            else
                                minimumVersionsDictionary.Add( dependency.name, dependency );
                        }
                    }
                }

                foreach ( var unityPackage in unityPackages )
                {
                    if ( minimumVersionsDictionary.ContainsKey( unityPackage.name ) )
                    {
                        var dependency = minimumVersionsDictionary[unityPackage.name];

                        if ( dependency.IsVersionHigherTo( unityPackage ) )
                        {
                            unityPackage.huf.config.minimumVersion = dependency.version;
                            unityPackage.huf.config.latestVersion = dependency.version;
                            unityPackage.huf.status = PackageStatus.ForceUpdate;
                        }
                    }

                    packages.Add( unityPackage );
                }
            }

            void CheckMinimumVersionNeededInOptionalDependencies()
            {
                var minimumVersionInOptionalDependencies = new Dictionary<string, Dependency>();

                foreach ( var localPackage in local )
                {
                    foreach ( var optionalDependency in localPackage.huf.optionalDependencies )
                    {
                        var dependency = new Dependency( optionalDependency );

                        if ( minimumVersionInOptionalDependencies.ContainsKey( dependency.name ) )
                        {
                            var currentDependency = minimumVersionInOptionalDependencies[dependency.name];

                            if ( Utils.VersionComparer.Compare( dependency.version, currentDependency.version, true ) >
                                 0 )
                                minimumVersionInOptionalDependencies[dependency.name] = dependency;
                        }
                        else
                            minimumVersionInOptionalDependencies.Add( dependency.name, dependency );
                    }
                }

                foreach ( var remotePackage in remote )
                {
                    if ( minimumVersionInOptionalDependencies.ContainsKey( remotePackage.name ) )
                    {
                        var dependency = minimumVersionInOptionalDependencies[remotePackage.name];

                        if ( Utils.VersionComparer.Compare( dependency.version,
                                 remotePackage.huf.config.minimumVersion ) >
                             0 )
                        {
                            remotePackage.huf.config.minimumVersion = dependency.version;
                        }
                    }
                }
            }

            void CheckIfPackagesAreSupportedByCurrentUnity()
            {
                foreach ( var package in packages )
                {
                    package.CheckIfCurrentUnitySupportsThisPackage();

                    if ( !package.SupportsCurrentUnityVersion && local.Contains( package ) &&
                         package.LatestPackageVersion().SupportsCurrentUnityVersion )
                    {
                        package.huf.status = PackageStatus.ForceUpdate;
                    }
                }
            }
        }
    }
}