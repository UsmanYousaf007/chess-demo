using UnityEngine;
using UnityEngine.UI;

namespace HUFEXT.CrossPromo.Implementation.View.Common
{
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup)), ExecuteAlways]
    public class ImageWidthAdjuster : MonoBehaviour
    {
        [SerializeField] int minMargin = default;
        [SerializeField] RectTransform self = default;
        [SerializeField] RectTransform background = default;
        [SerializeField] HorizontalLayoutGroup layoutGroup = default;

        void OnRectTransformDimensionsChange()
        {
            var padding = layoutGroup.padding; 
            padding.left = minMargin;
            padding.right = minMargin;
            layoutGroup.padding = padding;

            background.sizeDelta = self.sizeDelta;
        }
    }
}