using HUF.Utils.Runtime.Configs.API;
using HUFEXT.CrossPromo.Runtime.Implementation.View.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.BannerTile
{
    public class BannerTileImage : BaseImage, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
    {
        float pointerDownFadeValue;
        float pointerDownFadeTime;
        float horizontalSwipeThreshold;
        float clickDetectionThreshold;
        bool isInteractive = true;

        public event UnityAction<RotationDirection> OnBannerRotate;
        
        Vector2 startPosition;

        void OnEnable()
        {
            var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();
            pointerDownFadeValue = config.ImagesInteractionFadeValue;
            pointerDownFadeTime = config.ImagesInteractionFadeTime;
            horizontalSwipeThreshold = config.TopBannerHorizontalSwipeThreshold;
            clickDetectionThreshold = config.ClickDetectionThreshold;
        }

        public void SetInteractive(bool isInteractive)
        {
            this.isInteractive = isInteractive;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isInteractive)
            {
                return;
            }
            
            startPosition = eventData.position;
            imageView.CrossFadeAlpha(pointerDownFadeValue, pointerDownFadeTime, false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isInteractive)
            {
                return;
            }
            
            var delta = startPosition - eventData.position;
            if (delta.x >= horizontalSwipeThreshold)
            {
                OnBannerRotate?.Invoke(RotationDirection.Left);
            }
            else if (delta.x <= -horizontalSwipeThreshold)
            {
                OnBannerRotate?.Invoke(RotationDirection.Right);
            }
            else if(delta.magnitude < clickDetectionThreshold)
            {
                HandlePointerUp();
            }
            
            RestoreAlpha();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInteractive)
            {
                return;
            }
            
            RestoreAlpha();
        }

        void RestoreAlpha()
        {
            imageView.CrossFadeAlpha(1.0f, pointerDownFadeTime, false);
        }
    }
}