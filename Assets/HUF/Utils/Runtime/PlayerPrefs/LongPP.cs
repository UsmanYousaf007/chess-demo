namespace HUF.Utils.Runtime.PlayerPrefs 
{
    public class LongPP : CustomPP<long>
    {
        protected override void SetValue(long value)
        {
            HPlayerPrefs.SetLong(key, value);
        }

        protected override long GetValue()
        {
            return HPlayerPrefs.GetLong(key);
        }

        public LongPP(string key, long defaultValue = default) : base(key, defaultValue) { }
    }
}