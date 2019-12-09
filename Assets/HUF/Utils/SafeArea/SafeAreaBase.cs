using System;
using System.Collections;
using UnityEngine;

namespace HUF.Utils.SafeArea
{
    public abstract class SafeAreaBase : MonoBehaviour
    {
        Canvas canvas;
        protected Rect safeArea;
        float lastScaleFactor;
        protected float scaleFactor = 1f;

        [SerializeField] protected bool isUsingCanvasScaler;
        [SerializeField] protected bool autoRefresh;
        [SerializeField] protected bool modifyTop;
        [SerializeField] protected bool modifyBottom;
        [SerializeField] protected bool modifyLeft;
        [SerializeField] protected bool modifyRight;

        protected void Awake()
        {
            canvas = GetComponent<Canvas>();
            
            if (!autoRefresh)
                Adjust();
        }

        IEnumerator ReadScaleFactor()
        {
            yield return null;

            if (canvas == null)
                yield break;
            
            if (Math.Abs(lastScaleFactor - canvas.scaleFactor) > 0.1)
            {
                lastScaleFactor = canvas.scaleFactor;
                scaleFactor = 1f / lastScaleFactor;
                ForceAdjust();
            }
        }
        
        void OnEnable()
        {
            if (autoRefresh)
            {
                SafeAreaManager.OnSafeAreaChange += Adjust;
                Adjust();
            }
            if (isUsingCanvasScaler)
                CoroutineManager.StartCoroutine(ReadScaleFactor());
        }

        void OnDisable()
        {
            SafeAreaManager.OnSafeAreaChange -= Adjust;
        }

        void Adjust()
        {
            if (safeArea == SafeAreaManager.Instance.SafeArea)
                return;

            safeArea = SafeAreaManager.Instance.SafeArea;
            ForceAdjust();
        }

        protected abstract void ForceAdjust();

    }
}