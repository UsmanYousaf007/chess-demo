using HUFEXT.CrossPromo.Runtime.Implementation.View.Common;
using TMPro;
using UnityEngine;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.CrossPromoTile
{
    public class TileLabel : BaseImage
    {
        [SerializeField] TMP_Text labelText = default;

        public void SetText(string text)
        {
            labelText.SetText(text);
        }

        public void SetColor(Color color)
        {
            imageView.color = color;
        }
    }
}