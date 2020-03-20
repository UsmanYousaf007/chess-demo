namespace HUFEXT.PackageManager.Editor.Implementation.Huuuge.Utils
{
    [System.Serializable]
    public class RouteBuilder
    {
        private string route;
        public string Value => route;

        internal RouteBuilder( string path )
        {
            route = path;
        }

        public RouteBuilder Set( string pattern, string value )
        {
            route = route.Replace( pattern, value );
            return this;
        }
    }
}
