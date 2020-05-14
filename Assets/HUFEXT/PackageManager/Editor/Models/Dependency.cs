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

        public bool IsValid => !string.IsNullOrEmpty( name ) && !string.IsNullOrEmpty( version );
        
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
            version = index == 0 ? versionWithRange : versionWithRange.Substring( index, versionWithRange.Length - index );
            range = versionWithRange.Substring( 0, index );
        }

        public override string ToString()
        {
            return $"{name}@{range}{version}";
        }
    }
    
    [Serializable]
    internal class Dependencies : Utils.Common.Wrapper<Dependency> {}
}