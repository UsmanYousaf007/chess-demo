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

            var padding = layout.padding;
            baseOffsetValue = new RectOffset(padding.left, padding.right, padding.top, padding.bottom);
            base.Awake();
        }

        protected override void ForceAdjust()
        {
            if (modifyTop)
                layout.padding.top =  (int) (baseOffsetValue.top + (ScreenSize.Height - safeArea.yMax) * scaleFactor);

            if (modifyBottom)
                layout.padding.bottom = (int) (baseOffsetValue.bottom + safeArea.yMin * scaleFactor);

            if (modifyLeft)
                layout.padding.left = (int) (baseOffsetValue.left + safeArea.xMin * scaleFactor);

            if (modifyRight)
                layout.padding.right = (int) (baseOffsetValue.right + (ScreenSize.Width - safeArea.xMax) * scaleFactor);

            if (!layout.enabled)
            {
                return;
            }

            layout.enabled = false;
            layout.enabled = true;
        }
    }
}