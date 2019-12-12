namespace HUF.Utils.PlayerPrefs  
{
    public class IntPP : CustomPP<int>
    {
        protected override void SetValue(int value)
        {
            HPlayerPrefs.SetInt(key, value);
        }

        protected override int GetValue()
        {
            return HPlayerPrefs.GetInt(key, defaultValue);
        }

        public IntPP(string key, int defaultValue = default) : base(key, defaultValue) { }
    }
}