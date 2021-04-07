using UnityEngine;

namespace HUF.Utils.Runtime.SafeArea
{
    [RequireComponent( typeof(RectTransform) )]
    public class SafeAreaScaleRectTransform : SafeAreaBase
    {
        RectTransform thisRect;
        Vector2 startOffsetMin;
        Vector2 startOffsetMax;
        
        new void Awake()
        {    
            base.Awake();
            thisRect = GetComponent<RectTransform>();
            startOffsetMin = thisRect.offsetMin;
            startOffsetMax = thisRect.offsetMax;
        }

        protected override void ForceAdjust()
        {
            var bottom = modifyBottom ? safeArea.yMin : 0f;
            var top = modifyTop ? ( safeArea.yMax - ScreenSize.Height ) : 0f;
            var left = modifyLeft ? safeArea.xMin : 0f;
            var right = modifyRight ?( safeArea.xMax - ScreenSize.Width ) : 0f;
            thisRect.offsetMin = startOffsetMin + new Vector2( left, bottom ) * scaleFactor;
            thisRect.offsetMax = startOffsetMax + new Vector2( right, top ) * scaleFactor;
        }
    }
}