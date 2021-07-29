using System;
using System.Collections;
using System.Collections.Generic;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.SafeArea;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace HUF.Utils.Runtime.UI.CanvasBlocker
{
    public class DebugButtonsScreen : HSingleton<DebugButtonsScreen>
    {
        const int MARGIN = 10;
        const int MAXIMUM_NUMBER_OF_BUTTONS = 12;
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(DebugButtonsScreen) );
        readonly Color backgroundColor = new Color( 0, 0, 0, 0.7f );

        List<GUIButtonData> guiButtonDatas = new List<GUIButtonData>();
        string screenName;
        static float ButtonWidth => ScreenSize.Width / 2;
        static float ButtonHeight => ScreenSize.Height / MAXIMUM_NUMBER_OF_BUTTONS;
        CanvasBlocker canvasBlocker;

        /// <summary>
        /// Adds a GUI button.
        /// <para>Buttons are deleted when DebugButtonsScreen is hidden or a button is clicked.</para>
        /// </summary>
        /// <param name="buttonText">A text of the button</param>
        /// <param name="onClickAction">An action called after clicking the button.</param>
        [PublicAPI]
        public void AddGUIButton( string buttonText, Action onClickAction )
        {
            guiButtonDatas.Add( new GUIButtonData( buttonText, onClickAction ) );
        }

        /// <summary>
        /// Shows DebugButtonsScreen.
        /// <para>It will hide after the button is clicked.</para>
        /// </summary>
        /// <param name="inScreenName">A name of the screen, which will be shown on the top of it.</param>
        [PublicAPI]
        public void Show( string inScreenName )
        {
            if ( guiButtonDatas.Count == 0 )
            {
                HLog.LogWarning( logPrefix, "Cannot be shown without any buttons!" );
                return;
            }

            screenName = inScreenName;
            gameObject.SetActive( true );
            canvasBlocker.ShowFullScreen( backgroundColor );
        }

        /// <summary>
        /// Hides DebugButtonsScreen and deletes buttons.
        /// </summary>
        [PublicAPI]
        public void Hide()
        {
            gameObject.SetActive( false );
            guiButtonDatas.Clear();
            canvasBlocker.Hide();
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
                ScreenSize.Height / 2 - guiButtonDatas.Count * ButtonHeight / 2f - MARGIN );

            for ( var i = 0; i < guiButtonDatas.Count; i++ )
            {
                DrawGUIButton( guiButtonDatas[i], ref buttonPost );
            }
        }

        static Rect GetButtonRect( Vector2 position )
        {
            return new Rect( position, new Vector2( ButtonWidth, ButtonHeight ) );
        }

        void DrawGUIButton( GUIButtonData guiButtonData, ref Vector2 position )
        {
            if ( GUI.Button( GetButtonRect( position ), guiButtonData.buttonText ) )
            {
                guiButtonData.clickAction.Dispatch();
                Hide();
            }

            position.y += MARGIN * 2 + ButtonHeight;
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
    }
}