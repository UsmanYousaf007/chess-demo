using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.Common
{
    public class BaseImage : MonoBehaviour
    {
        [SerializeField] protected Image imageView = default;

        UnityAction onBannerClick;
        
        public void SetSprite(Sprite sprite)
        {
            imageView.sprite = sprite;
        }
        
        public void SetAction(UnityAction action)
        {
            onBannerClick = action;
        }

        protected void HandlePointerUp()
        {
            onBannerClick?.Invoke();
        }
    }
}