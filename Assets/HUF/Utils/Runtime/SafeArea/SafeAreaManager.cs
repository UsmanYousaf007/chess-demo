using HUF.Utils.Runtime.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Utils.Runtime.SafeArea
{
#if UNITY_EDITOR
    public struct SafeAreaSimulatorHelper
    {
        public const string EDITOR_SAVE_VALUES_KEY = "EditorSafeAreaSimulator";
        public int top;
        public int bottom;
        public int left;
        public int right;
    }
#endif

    [AddComponentMenu( "" )]
    [DefaultExecutionOrder( -31900 )]
    public class SafeAreaManager : HSingleton<SafeAreaManager>
    {
        public static event UnityAction OnSafeAreaChange;
        Rect cachedSafeArea;
        ScreenOrientation lastScreenOrientation;

        public Rect SafeArea
        {
            get
            {
                if ( cachedSafeArea == Rect.zero )
                {
                    CalculateSafeArea();
                }

                return cachedSafeArea;
            }
        }

        public void CalculateSafeArea()
        {
            cachedSafeArea = GetSafeArea();
            OnSafeAreaChange.Dispatch();
            Canvas.ForceUpdateCanvases();
        }

        void Awake()
        {
            lastScreenOrientation = Screen.orientation;
            CalculateSafeArea();
        }

        void Update()
        {
            var newSafeArea = GetSafeArea();

            if ( lastScreenOrientation != Screen.orientation || cachedSafeArea != newSafeArea )
            {
                lastScreenOrientation = Screen.orientation;
                CalculateSafeArea();
            }
        }

        Rect GetSafeArea()
        {
#if UNITY_EDITOR
            if ( !EditorPrefs.HasKey( SafeAreaSimulatorHelper.EDITOR_SAVE_VALUES_KEY ) )
                return Screen.safeArea;

            var editorArea =
                JsonUtility.FromJson<SafeAreaSimulatorHelper>(
                    EditorPrefs.GetString( SafeAreaSimulatorHelper.EDITOR_SAVE_VALUES_KEY ) );

            var editorSafeArea = new Rect(
                editorArea.left,
                editorArea.bottom,
                ScreenSize.Width - editorArea.left - editorArea.right,
                ScreenSize.Height - editorArea.top - editorArea.bottom );
            return editorSafeArea;
#else
            return Screen.safeArea;
#endif
        }
    }
}