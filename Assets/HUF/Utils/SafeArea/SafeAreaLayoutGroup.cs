using UnityEngine;
using UnityEngine.UI;

namespace HUF.Utils.SafeArea
{
    [RequireComponent(typeof(LayoutGroup))]
    public class SafeAreaLayoutGroup : SafeAreaBase
    {
        LayoutGroup layout;
        RectOffset baseOffsetValue;

        new void Awake()
        {
            layout = GetComponent<LayoutGroup>();
            if (layout == null)
                return;

            baseOffsetValue = new RectOffset(layout.padding.left, layout.padding.right, layout.padding.top, layout.padding.bottom);
            base.Awake();
        }

        protected override void ForceAdjust()
        {
            if (modifyTop)
                layout.padding.top =  (int) (baseOffsetValue.top + (Screen.height - safeArea.yMax) * scaleFactor);

            if (modifyBottom)
                layout.padding.bottom = (int) (baseOffsetValue.bottom + safeArea.yMin * scaleFactor);

            if (modifyLeft)
                layout.padding.left = (int) (baseOffsetValue.left + safeArea.xMin * scaleFactor);

            if (modifyRight)
                layout.padding.right = (int) (baseOffsetValue.right + (Screen.width - safeArea.xMax) * scaleFactor);
                
            layout.CalculateLayoutInputHorizontal();
            layout.CalculateLayoutInputVertical();
        }
    }
}