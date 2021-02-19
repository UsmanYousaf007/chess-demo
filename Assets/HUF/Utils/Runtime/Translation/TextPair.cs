using UnityEngine;

namespace HUF.Utils.Runtime.Translation
{
    [System.Serializable]
    public class TextPair
    {
        [SerializeField] string language = "en";
        [SerializeField] string text = "Text";

        public string Text => text;
        public string Language => language;
    }
}