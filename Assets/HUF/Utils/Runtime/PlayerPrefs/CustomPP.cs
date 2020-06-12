using UnityEngine;

namespace HUF.Utils.Runtime.PlayerPrefs  
{
    public class CustomPP<T>
    {
        protected readonly string key;
        protected readonly T defaultValue;

        public T Value
        {
            get => HPlayerPrefs.HasKey(key) == false ? defaultValue : GetValue();
            set => SetValue(value);
        }

        protected virtual void SetValue(T value)
        {
            HPlayerPrefs.SetString(key, JsonUtility.ToJson(value));
        }

        protected virtual T GetValue()
        {
            return JsonUtility.FromJson<T>(HPlayerPrefs.GetString(key));
        }

        public CustomPP(string key, T defaultValue = default(T))
        {
            this.key = key;
            this.defaultValue = defaultValue;
        }

        public static implicit operator T(CustomPP<T> pref)
        {
            return pref.Value;
        }
    }
}