using UnityEngine;

namespace HUF.Utils.Runtime.SafeArea
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaScaleRectTransform : SafeAreaBase
    {
        RectTransform thisRect;
        Vector2 baseSizeDelta;
        Vector2 baseAnchoredPosition;

        new void Awake()
        {
            thisRect = GetComponent<RectTransform>();
            baseSizeDelta = thisRect.sizeDelta;
            baseAnchoredPosition = thisRect.anchoredPosition;
            base.Awake();
        }

        protected override void ForceAdjust()
        {
            var pivot = thisRect.pivot;
            var sizeDelta = baseSizeDelta;
            var anchoredPosition = baseAnchoredPosition;
            var top = modifyTop ? (ScreenSize.Height - safeArea.yMax) * scaleFactor : 0f;
            var bottom = modifyBottom ? safeArea.yMin * scaleFactor : 0f;
            var left = modifyLeft ? safeArea.xMin * scaleFactor : 0f;
            var right = modifyRight ? (ScreenSize.Width - safeArea.xMax) * scaleFactor : 0f;

            sizeDelta -= new Vector2(left + right,top + bottom);
            anchoredPosition += (bottom - top) * pivot.y * Vector2.up;
            anchoredPosition += (left - right) * pivot.x * Vector2.right;

            thisRect.anchoredPosition = anchoredPosition;
            thisRect.sizeDelta = sizeDelta;
        }
    }
}