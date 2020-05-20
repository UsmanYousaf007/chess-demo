namespace HUF.Utils.Runtime.PlayerPrefs
{
    public class UIntPP : CustomPP<uint>
    {
        protected override void SetValue(uint value)
        {
            HPlayerPrefs.SetUInt(key, value);
        }

        protected override uint GetValue()
        {
            return HPlayerPrefs.GetUInt(key);
        }

        public UIntPP(string key, uint defaultValue = default) : base(key, defaultValue) { }
    }
}