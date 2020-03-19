﻿using BlowFishCS;

namespace HUF.Utils.PlayerPrefs.SecureTypes
{
    public class SecureStringArrayPP
    {
        readonly StringArrayPP pref;
        readonly BlowFish encryption;

        public SecureStringArrayPP(string key, char separator, BlowFish encryption = null)
        {
            pref = new StringArrayPP(key, separator);
            this.encryption = encryption;
        }

        public string[] Value
        {
            get
            {
                if (pref.Value == null)
                    return null;
                
                var outputValues = pref.Value;
                
                for (int i = 0; i < outputValues.Length; i++)
                {
                    outputValues[i] = SecurePPHelper.EncryptString(outputValues[i], encryption);
                }
                return outputValues;
            }
            set
            {
                if (value == null)
                    return;
                
                string[] inputValues = value;
                for (int i = 0; i < inputValues.Length; i++)
                {
                    inputValues[i] = SecurePPHelper.DecryptString(inputValues[i], encryption);
                }
                pref.Value = inputValues;
            }
        }

        public void Add(string newElement)
        {
            pref.Add(SecurePPHelper.EncryptString(newElement, encryption));
        }
        
        public void AddUnique(string value)
        {
            if (!HasElement(value))
            {
                Add(value);
            }
        }

        bool HasElement(string element)
        {
            return pref.HasElement(SecurePPHelper.EncryptString(element, encryption));
        }

        public void Clear()
        {
            pref.Clear();
        }

    }
}