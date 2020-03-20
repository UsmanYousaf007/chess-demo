namespace HUF.Utils.PlayerPrefs
{
    public class StringPP : CustomPP<string>
    {
        protected override void SetValue(string value)
        {
            HPlayerPrefs.SetString(key, value);
        }

        protected override string GetValue()
        {
            return HPlayerPrefs.GetString(key, defaultValue);
        }

        public StringPP(string key, string defaultValue = default) : base(key, defaultValue) { }
    }
}