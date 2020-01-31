using UnityEngine;

namespace HUF.Utils.SafeArea
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaScaleRectTransform : SafeAreaBase
    {
        RectTransform thisRect;
        Vector2 baseSizeDelta;
        Vector2 baseAnchoredPosition;
        
        void Awake()
        {
            thisRect = GetComponent<RectTransform>();
            baseSizeDelta = thisRect.sizeDelta;
            baseAnchoredPosition = thisRect.anchoredPosition;
            base.Awake();
        }
        
        protected override void ForceAdjust()
        {
            var sizeDelta = baseSizeDelta;
            var anchoredPosition = baseAnchoredPosition;
            var top = (Screen.height - safeArea.yMax) * scaleFactor;
            var bottom = safeArea.yMin * scaleFactor;
            var left = safeArea.xMin * scaleFactor;
            var right =  (Screen.width - safeArea.xMax) * scaleFactor;
            
            if (modifyTop && safeArea.yMax < Screen.height && modifyBottom && safeArea.yMin > 0.0f)
            {
                sizeDelta.y -= top + bottom;
                anchoredPosition += new Vector2(
                    0.0f,
                    (bottom - top) * thisRect.pivot.y);
            }
            else
            {
                if (modifyTop && safeArea.yMax < Screen.height)
                {
                    sizeDelta.y -= top;
                    anchoredPosition += new Vector2(
                        0.0f,
                        top * thisRect.pivot.y);
                }
                else if (modifyBottom && safeArea.yMin > 0.0f)
                {
                    sizeDelta.y += bottom;
                    anchoredPosition += new Vector2(
                        0.0f,
                        -bottom);
                }
            }

            if (modifyLeft && safeArea.xMin > 0.0f && modifyRight && safeArea.xMax < Screen.width)
            {
                sizeDelta.x -= left + right;
                anchoredPosition += new Vector2(
                    (left - right) * thisRect.pivot.x,
                    0.0f);
            }
            else
            {
                if (modifyLeft && safeArea.xMin > 0.0f)
                {
                    sizeDelta.x -= left;
                    anchoredPosition += new Vector2(
                        left * thisRect.pivot.x,
                        0.0f);
                }
                else if (modifyRight && safeArea.xMax < Screen.width)
                {
                    sizeDelta.x -= right;
                    anchoredPosition += new Vector2(
                        -right * thisRect.pivot.x,
                        0.0f);
                }
            }

            thisRect.anchoredPosition = anchoredPosition;
            thisRect.sizeDelta = sizeDelta;
        }
    }
}