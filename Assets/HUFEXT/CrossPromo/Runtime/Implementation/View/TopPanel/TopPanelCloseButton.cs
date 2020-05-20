using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.TopPanel
{
    public class TopPanelCloseButton : MonoBehaviour
    {
        [SerializeField] Button button = default;
        
        UnityAction onClickAction;
        
        public void SetAction(UnityAction action)
        {
            onClickAction = action;
        }
        
        void OnEnable()
        {
            button.onClick.AddListener(HandleClick);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(HandleClick);
        }
        
        void HandleClick()
        {
            onClickAction?.Invoke();
        }
    }
}