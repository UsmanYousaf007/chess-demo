using TMPro;
using UnityEngine;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.Common
{
    [RequireComponent(typeof(TMP_Text))]
    public class BaseText : MonoBehaviour
    {
        [SerializeField] TMP_Text textView = default;

        public void SetText(string text)
        {
            textView.text = text;
        }
    }
}