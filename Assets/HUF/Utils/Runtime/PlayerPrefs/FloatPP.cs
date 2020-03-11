namespace HUF.Utils.PlayerPrefs  
{
    public class FloatPP : CustomPP<float>
    {
        protected override void SetValue(float value)
        {
            HPlayerPrefs.SetFloat(key, value);
        }

        protected override float GetValue()
        {
            return HPlayerPrefs.GetFloat(key, defaultValue);
        }

        public FloatPP(string key, float defaultValue = default) : base(key, defaultValue) { }
    }
}