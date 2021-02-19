namespace HUF.Utils.Runtime.PlayerPrefs
{
    public class BoolPP : CustomPP<bool>
    {
        protected override void SetValue( bool value )
        {
            HPlayerPrefs.SetBool( key, value );
        }

        protected override bool GetValue()
        {
            return HPlayerPrefs.GetBool( key );
        }

        public BoolPP( string key, bool defaultValue = default ) : base( key, defaultValue ) { }
    }
}