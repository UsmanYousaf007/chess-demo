using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HUF.Utils.Runtime.UI.CanvasBlocker
{
    public class CanvasBlocker : MonoBehaviour
    {
        const int MAXIMUM_SORTING_ORDER = short.MaxValue;
        const string NAME_PREFIX = "CanvasBlocker.";
        Canvas canvas;
        Image image;
        RectTransform imageRectTransform;

        /// <summary>
        /// Creates an instance of CanvasBlocker.
        /// </summary>
        /// <param name="objectName">A name of created CanvasBlocker.</param>
        /// <returns></returns>
        [PublicAPI]
        public static CanvasBlocker Create( string objectName )
        {
            var gameObject = new GameObject( $"{NAME_PREFIX}{objectName}",
                typeof(Canvas),
                typeof(GraphicRaycaster) );
            return gameObject.AddComponent<CanvasBlocker>();
        }

        /// <summary>
        /// Shows CanvasBlocker in fullscreen.
        /// </summary>
        /// <param name="backgroundColor">A color of the background.</param>
        [PublicAPI]
        public void ShowFullScreen( Color backgroundColor )
        {
            Show( backgroundColor );
            imageRectTransform.anchorMin = Vector2.zero;
            imageRectTransform.anchorMax = Vector2.one;
            imageRectTransform.offsetMin = imageRectTransform.offsetMax = Vector2.zero;
        }

        
        /// <summary>
        ///  Shows CanvasBlocker in the panel mode.
        /// </summary>
        /// <param name="backgroundColor">A color of the background.</param>
        /// <param name="positionInPixels">A position of the panel in pixels.</param>
        /// <param name="sizeInPixels">A size of the panel in pixels.</param>
        [PublicAPI]
        public void ShowPanel( Color backgroundColor, Vector2 positionInPixels, Vector2 sizeInPixels )
        {
            Show( backgroundColor );
            imageRectTransform = image.rectTransform;
            imageRectTransform.pivot = Vector2.up;
            var canvasPixelRect = canvas.pixelRect;

            imageRectTransform.localPosition = new Vector2( positionInPixels.x - canvasPixelRect.width / 2,
                -positionInPixels.y + canvasPixelRect.height / 2 );
            imageRectTransform.sizeDelta = sizeInPixels;
        }

        /// <summary>
        /// Hides CanvasBlocker.
        /// </summary>
        [PublicAPI]
        public void Hide()
        {
            gameObject.SetActive( false );
        }

        void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MAXIMUM_SORTING_ORDER;
            image = new GameObject().AddComponent<Image>();
            image.transform.SetParent( transform );
            imageRectTransform = image.rectTransform;
            DontDestroyOnLoad( gameObject );
        }

        void Show( Color backgroundColor )
        {
            image.color = backgroundColor;
            gameObject.SetActive( true );
        }
    }
}