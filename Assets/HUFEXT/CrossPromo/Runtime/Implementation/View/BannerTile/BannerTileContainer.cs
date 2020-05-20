using HUFEXT.CrossPromo.Runtime.Implementation.View.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.BannerTile
{
    [RequireComponent(typeof(RectTransform))]
    public class BannerTileContainer : MonoBehaviour, ILayoutElement
    {
        [SerializeField] RectTransform self = default;
        [SerializeField] BannerTileImage imageView = default;
        [SerializeField] ActionButton actionButtonView = default;
        
        public event UnityAction<RotationDirection> OnBannerRotation;

        RectTransform sizeArchetype;
        
        void OnEnable()
        {
            imageView.OnBannerRotate += HandleBannerRotation;
        }

        void OnDisable()
        {
            imageView.OnBannerRotate -= HandleBannerRotation;
        }

        public void SetSizeArchetype(RectTransform sizeArchetype)
        {
            this.sizeArchetype = sizeArchetype;
        }
        
        void HandleBannerRotation(RotationDirection rotationDirection)
        {
            OnBannerRotation?.Invoke(rotationDirection);
        }

        public void SetSprite(Sprite sprite)
        {
            imageView.SetSprite(sprite);
        }

        public void SetButtonText(string text)
        {
            actionButtonView.SetText(text);
        }

        public void SetButtonAction(UnityAction action)
        {
            actionButtonView.SetAction(action);
        }
        
        public void SetButtonColor(Color color)
        {
            actionButtonView.SetColor(color);
        }

        public void SetImageClickAction(UnityAction action)
        {
            imageView.SetAction(action);
        }

        public void SetInteractive(bool isInteractive)
        {
            imageView.SetInteractive(isInteractive);
        }

        public void SetButtonActive(bool isButtonActive)
        {
            actionButtonView.SetInteractive(isButtonActive);
        }

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        public float minWidth => sizeArchetype.sizeDelta.x;
        public float preferredWidth { get; }
        public float flexibleWidth { get; }
        public float minHeight { get; }
        public float preferredHeight { get; }
        public float flexibleHeight { get; }
        public int layoutPriority { get; }
    }
}