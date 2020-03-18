using UnityEngine;
using UnityEngine.UI;

namespace HUFEXT.CrossPromo.Implementation.View.Common
{
    [RequireComponent(typeof(RectTransform))]
    public class PersistentAspectRatioLayout : MonoBehaviour, ILayoutElement
    {
        [SerializeField] RectTransform self = default;
        [SerializeField] float widthToHeightAspectRatio = default;

        public void CalculateLayoutInputHorizontal()
        {

        }

        public void CalculateLayoutInputVertical()
        {

        }

        public float minWidth { get; }
        public float preferredWidth { get; }
        public float flexibleWidth { get; }
        public float minHeight => self.sizeDelta.x / widthToHeightAspectRatio;
        public float preferredHeight { get; }
        public float flexibleHeight { get; }
        public int layoutPriority { get; }
    }
}