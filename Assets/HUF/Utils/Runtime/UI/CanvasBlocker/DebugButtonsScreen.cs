using System;
using System.Collections.Generic;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.SafeArea;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Utils.Runtime.UI.CanvasBlocker
{
    public class DebugButtonsScreen : HSingleton<DebugButtonsScreen>
    {
        const int MARGIN = 10;
        const int MAXIMUM_NUMBER_OF_BUTTONS = 12;
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(DebugButtonsScreen) );
        readonly Color backgroundColor = new Color( 0, 0, 0, 0.7f );

        readonly List<GUIButtonData> guiButtonData = new List<GUIButtonData>();
        readonly List<GUIInputData> guiInputData = new List<GUIInputData>();

        string screenName;
        CanvasBlocker canvasBlocker;

        static float ButtonWidth => ScreenSize.Width / 2;

        static float ButtonHeight =>
            ( ScreenSize.Height - ( MAXIMUM_NUMBER_OF_BUTTONS + 1 ) * MARGIN )
            / MAXIMUM_NUMBER_OF_BUTTONS;

        /// <summary>
        /// States if it is possible to open the debug screen
        /// </summary>
        public static bool IsAvailable { get; private set; } = true;

        /// <summary>
        /// Adds a GUI button.
        /// <para>Buttons are deleted when DebugButtonsScreen is hidden or a button is clicked.</para>
        /// </summary>
        /// <param name="buttonText">A text of the button</param>
        /// <param name="onClickAction">An action called after clicking the button.</param>
        [PublicAPI]
        public void AddGUIButton( string buttonText, Action onClickAction )
        {
            guiButtonData.Add( new GUIButtonData( buttonText, onClickAction ) );
        }

        /// <summary>
        /// Adds a GUI input field.
        /// <para>Controls are deleted when DebugButtonsScreen is hidden or a button is clicked.</para>
        /// </summary>
        /// <param name="label">A label of the input field</param>
        /// <param name="dataCallback">A callback sent before the window is closed, containing entered data.</param>
        [PublicAPI]
        public void AddGUIInput( string label, Action<string> dataCallback )
        {
            guiInputData.Add( new GUIInputData( label, dataCallback ) );
        }

        /// <summary>
        /// Shows DebugButtonsScreen.
        /// <para>It will hide after the button is clicked.</para>
        /// </summary>
        /// <param name="inScreenName">A name of the screen, which will be shown on the top of it.</param>
        [PublicAPI]
        public void Show( string inScreenName )
        {
            if ( guiButtonData.Count == 0 )
            {
                HLog.LogWarning( logPrefix, "Cannot be shown without any buttons!" );
                return;
            }

            screenName = inScreenName;
            gameObject.SetActive( true );
            IsAvailable = false;
            canvasBlocker.ShowFullScreen( backgroundColor );
        }

        /// <summary>
        /// Hides DebugButtonsScreen and deletes buttons.
        /// </summary>
        [PublicAPI]
        public void Hide()
        {
            gameObject.SetActive( false );
            guiButtonData.Clear();
            guiInputData.Clear();
            canvasBlocker.Hide();
            IsAvailable = true;
        }

        static Rect GetButtonRect( Vector2 position )
        {
            return new Rect( position, new Vector2( ButtonWidth, ButtonHeight ) );
        }

        void Awake()
        {
            canvasBlocker = CanvasBlocker.Create( nameof(DebugButtonsScreen) );
        }

        void OnGUI()
        {
            GUI.Box( new Rect( MARGIN, MARGIN, ScreenSize.Width - 2 * MARGIN, ScreenSize.Height - 2 * MARGIN ),
                screenName );

            var buttonPost = new Vector2( ScreenSize.Width / 2 - ButtonWidth / 2f + MARGIN,
                 MARGIN + ButtonHeight );

            for ( var i = 0; i < guiButtonData.Count; i++ )
            {
                DrawGUIElement( guiButtonData[i], ref buttonPost );
            }

            for ( var i = 0; i < guiInputData.Count; i++ )
            {
                DrawGUIElement( guiInputData[i], ref buttonPost );
            }
        }

        void DrawGUIElement( GUIButtonData guiButtonData, ref Vector2 position )
        {
            if ( GUI.Button( GetButtonRect( position ), guiButtonData.buttonText ) )
            {
                SendInputCallbacks();
                guiButtonData.clickAction.Dispatch();
                Hide();
            }

            position.y += MARGIN + ButtonHeight;
        }

        void DrawGUIElement( GUIInputData data, ref Vector2 position )
        {
            data.text = GUI.TextField( GetButtonRect( position ), data.text );

            Vector2 labelSize = GUI.skin.label.CalcSize( new GUIContent( data.label ) );
            var labelRect = new Rect( position - new Vector2( labelSize.x + MARGIN, 0 ), labelSize );
            GUI.Label( labelRect, data.label );

            position.y += MARGIN + ButtonHeight;
        }

        void SendInputCallbacks()
        {
            for ( var i = 0; i < guiInputData.Count; i++ )
            {
                GUIInputData inputData = guiInputData[i];
                inputData.callback.Dispatch( inputData.text );
            }
        }

        readonly struct GUIButtonData
        {
            public readonly string buttonText;
            public readonly Action clickAction;

            public GUIButtonData( string buttonText, Action clickAction )
            {
                this.buttonText = buttonText;
                this.clickAction = clickAction;
            }
        }

        class GUIInputData
        {
            public readonly string label;
            public readonly Action<string> callback;
            public string text;

            public GUIInputData( string label, Action<string> callback )
            {
                this.label = label;
                this.callback = callback;
            }
        }
    }
}
