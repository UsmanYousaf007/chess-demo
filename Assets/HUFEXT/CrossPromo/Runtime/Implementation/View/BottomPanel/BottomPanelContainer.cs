using HUFEXT.CrossPromo.API;
using HUFEXT.CrossPromo.Implementation.View.Common;
using UnityEngine;

namespace HUFEXT.CrossPromo.Implementation.View.BottomPanel
{
    public class BottomPanelContainer : MonoBehaviour
    {
        [SerializeField] RectTransform self = default;
        [SerializeField] LogoImageView logoImageViewView = default;
        [SerializeField] ActionButton closeButton = default;

        void OnEnable()
        {
            closeButton.SetAction(HandleCloseButton);
        }

        void HandleCloseButton()
        {
            HCrossPromo.ClosePanel();
        }

        public void SetLogoImageSprite(string filePath)
        {
            if (logoImageViewView == null)
                return;

            var sprite = Resources.Load<Sprite>(filePath);
            Debug.Log(filePath);
            Debug.Log(sprite);
            logoImageViewView.SetSprite(sprite);
        }

        public void SetButtonColor(Color color)
        {
            closeButton.SetColor(color);
        }

        public void UpdateTexts()
        {
            closeButton.SetText(CrossPromoService.CloseButtonText);
        }
    }
}