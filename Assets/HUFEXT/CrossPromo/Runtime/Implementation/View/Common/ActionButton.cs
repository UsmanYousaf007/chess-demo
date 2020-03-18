using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HUFEXT.CrossPromo.Implementation.View.Common
{
    public class ActionButton : MonoBehaviour
    {
        [SerializeField] Button button = default;
        [SerializeField] TMP_Text textView = default;
        [SerializeField] Image backgroundImage = default;
        [SerializeField] Image shadowImage = default;
        
        UnityAction onClickAction;
        
        void OnEnable()
        {
            button.onClick.AddListener(HandleClick);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(HandleClick);
        }

        public void SetText(string text)
        {
            textView.text = text;
        }

        public void SetAction(UnityAction action)
        {
            onClickAction = action;
        }

        void HandleClick()
        {
            onClickAction?.Invoke();
        }

        public void SetColor(Color color)
        {
            backgroundImage.color = color;
        }

        public void SetInteractive(bool isInteractive)
        {
            button.gameObject.SetActive(isInteractive);
            shadowImage.gameObject.SetActive(isInteractive);
        }
    }
}