using System;
using System.Linq;

namespace HUFEXT.PackageManager.Editor.Models
{
    [Serializable]
    public class Dependency
    {
        public string name;
        public string version;
        public string range;
        public Models.PackageChannel channel;
        public string scope;
        
        
        public Dependency( string package )
        {
            if ( !package.Contains( "@" ) )
            {
                name = package;
                version = string.Empty;
                range = string.Empty;
                return;
            }

            var elements = package.Split( '@' );
            var versionWithRange = elements[1];
            var index = versionWithRange.IndexOf( versionWithRange.FirstOrDefault( char.IsDigit ) );
            name = elements[0];

            version = index == 0
                ? versionWithRange
                : versionWithRange.Substring( index, versionWithRange.Length - index );
            range = versionWithRange.Substring( 0, index );
        }

        public bool IsValid => !string.IsNullOrEmpty( name ) && !string.IsNullOrEmpty( version );

        public bool IsHufPackage => name.Contains( ".huuuge." );

        public override string ToString()
        {
            return $"{name}@{range}{version}";
        }

        public bool IsVersionHigherOrEqualTo( Dependency dependency, bool ignoreTags = true ) =>
            IsVersionHigherOrEqualTo( dependency.version, ignoreTags );

        public bool IsVersionHigherOrEqualTo( PackageManifest packageManifest, bool ignoreTags = true ) =>
            IsVersionHigherOrEqualTo( packageManifest.version, ignoreTags );

        public bool IsVersionHigherOrEqualTo( string version, bool ignoreTags = true ) =>
            Utils.VersionComparer.Compare( this.version, version, ignoreTags ) >= 0;

        public bool IsVersionHigherTo( Dependency dependency, bool ignoreTags = true ) =>
            IsVersionHigherTo( dependency.version, ignoreTags );

        public bool IsVersionHigherTo( PackageManifest packageManifest, bool ignoreTags = true ) =>
            IsVersionHigherTo( packageManifest.version, ignoreTags );

        public bool IsVersionHigherTo( string version, bool ignoreTags = true ) =>
            Utils.VersionComparer.Compare( this.version, version, ignoreTags ) > 0;

        public bool HaveSameMajorVersion( Dependency dependency ) =>
            IsVersionHigherTo( dependency.version );

        public bool HaveSameMajorVersion( PackageManifest packageManifest ) =>
            IsVersionHigherTo( packageManifest.version );

        public bool HaveSameMajorVersion( string version )
        {
            if ( this.version.Contains( '.' ) && version.Contains( '.' ) && version.Substring( 0,
                version.IndexOf( '.' ) ) == this.version.Substring( 0, this.version.IndexOf( '.' ) ) )
                return true;

            return false;
        }
    }

    [Serializable]
    internal class Dependencies : Utils.Common.Wrapper<Dependency> { }
}