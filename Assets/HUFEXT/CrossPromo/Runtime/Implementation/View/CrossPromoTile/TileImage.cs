using HUF.Utils.Runtime.Configs.API;
using HUFEXT.CrossPromo.Runtime.Implementation.View.Common;
using UnityEngine.EventSystems;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.CrossPromoTile
{
    public class TileImage : BaseImage, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        float pointerDownFadeValue;
        float pointerDownFadeTime;
        bool isInteractive;

        void OnEnable()
        {
            var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();
            pointerDownFadeValue = config.ImagesInteractionFadeValue;
            pointerDownFadeTime = config.ImagesInteractionFadeTime;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isInteractive)
            {
                imageView.CrossFadeAlpha(pointerDownFadeValue, pointerDownFadeTime, false);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isInteractive)
            {
                RestoreAlpha();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isInteractive)
            {
                HandlePointerUp();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isInteractive)
            {
                RestoreAlpha();
            }
        }

        void RestoreAlpha()
        {
            imageView.CrossFadeAlpha(1.0f, pointerDownFadeTime, false);
        }

        public void SetInteractive(bool isInteractive)
        {
            this.isInteractive = isInteractive;
            RestoreAlpha();
        }
    }
}