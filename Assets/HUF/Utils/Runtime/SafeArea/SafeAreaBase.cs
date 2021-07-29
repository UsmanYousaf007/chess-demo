using System;
using System.Collections;
using UnityEngine;

namespace HUF.Utils.Runtime.SafeArea
{
    public abstract class SafeAreaBase : MonoBehaviour
    {
        const float MINIMUM_SCALE_FACTOR_THRESHOLD = 0.01f;

        [SerializeField] protected bool isUsingCanvasScaler;
        [SerializeField] protected bool autoRefresh;
        [SerializeField] protected bool modifyTop;
        [SerializeField] protected bool modifyBottom;
        [SerializeField] protected bool modifyLeft;
        [SerializeField] protected bool modifyRight;

        protected Rect safeArea;
        protected float scaleFactor = 1f;
        float lastScaleFactor = 1f;
        Canvas canvas;

        protected void Awake()
        {
            canvas = GetComponentInParent<Canvas>();

            if ( !autoRefresh )
                Adjust();
        }

        IEnumerator ReadScaleFactor()
        {
            yield return null;

            Adjust();
        }

        bool CheckScaleFactor()
        {
            if ( canvas == null || Math.Abs( lastScaleFactor - canvas.scaleFactor ) < MINIMUM_SCALE_FACTOR_THRESHOLD )
                return false;

            lastScaleFactor = canvas.scaleFactor;
            scaleFactor = 1f / lastScaleFactor;
            return true;
        }

        void OnEnable()
        {
            if ( autoRefresh )
            {
                SafeAreaManager.OnSafeAreaChange += Adjust;
                Adjust();
            }

            if ( isUsingCanvasScaler )
                CoroutineManager.StartCoroutine( ReadScaleFactor() );
        }

        void OnDisable()
        {
            SafeAreaManager.OnSafeAreaChange -= Adjust;
        }

        void Adjust()
        {
            if ( !CheckScaleFactor() && safeArea == SafeAreaManager.Instance.SafeArea )
                return;

            safeArea = SafeAreaManager.Instance.SafeArea;
            ForceAdjust();
        }

        protected abstract void ForceAdjust();
    }
}