using HUFEXT.CrossPromo.Implementation.View.Common;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.CrossPromo.Implementation.View.CrossPromoTile
{
    public class TileContainer : MonoBehaviour
    {
        [SerializeField] TileTitle titleView = default;
        [SerializeField] TileImage imageView = default;
        [SerializeField] TileLabel tileLabel = default;
        [SerializeField] ActionButton actionButtonView = default;

        public void SetTitle(string title)
        {
            titleView.SetText(title);
        }

        public void SetSprite(Sprite sprite)
        {
            imageView.SetSprite(sprite);
        }

        public void SetLabelSprite(Sprite sprite)
        {
            tileLabel.SetSprite(sprite);
        }

        public void SetLabelText(string text)
        {
            tileLabel.SetText(text);
        }

        public void SetLabelColor(Color color)
        {
            tileLabel.SetColor(color);
        }

        public void SetLabelActive(bool isActive)
        {
            tileLabel.gameObject.SetActive(isActive);
        }

        public void SetButtonAction(UnityAction action)
        {
            actionButtonView.SetAction(action);
        }

        public void SetButtonText(string text)
        {
            actionButtonView.SetText(text);
        }

        public void SetButtonColor(Color color)
        {
            actionButtonView.SetColor(color);
        }

        public void SetImageAction(UnityAction action)
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
    }
}