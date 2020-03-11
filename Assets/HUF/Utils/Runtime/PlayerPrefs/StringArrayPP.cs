using HUF.Utils.Extensions;

namespace HUF.Utils.PlayerPrefs
{
    public class StringArrayPP
    {
        readonly StringPP pref;
        readonly char separator;

        public StringArrayPP(string key, char separator)
        {
            pref = new StringPP(key);
            this.separator = separator;
        }

        public string[] Value
        {
            get { return pref.Value.IsNullOrEmpty() ? null : pref.Value.Split(separator); }
            set
            {
                if (value != null)
                    pref.Value = string.Join(separator.ToString(), value);
                else
                    Clear();
            }
        }

        public void Add(string newElement)
        {
            pref.Value = pref.Value.IsNullOrEmpty() ? newElement : pref.Value + separator + newElement;
        }
        
        public void AddUnique(string value)
        {
            if (!HasElement(value))
            {
                Add(value);
            }
        }

        public bool HasElement(string element)
        {
            string[] values = Value;
            
            if (values == null)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (value == element)
                {
                    return true;
                }   
            }

            return false;
        }

        public void Clear()
        {
            pref.Value = string.Empty;
        }
    }
}