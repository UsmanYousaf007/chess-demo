using System;
using System.Collections;
using System.Collections.Generic;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.SafeArea;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace HUF.Utils.Runtime.UI.CanvasBlocker
{
    public class DebugButtonsScreen : HSingleton<DebugButtonsScreen>
    {
        const int MARGIN = 10;
        const int MAXIMUM_NUMBER_OF_BUTTONS = 12;
        readonly Color backgroundColor = new Color( 0, 0, 0, 0.7f );

        List<GUIButtonData> guiButtonDatas = new List<GUIButtonData>();
        string screenName;
        static float ButtonWidth => ScreenSize.Width / 2;
        static float ButtonHeight => ScreenSize.Height / MAXIMUM_NUMBER_OF_BUTTONS;
        CanvasBlocker canvasBlocker;

        public void AddGUIButton( string buttonText, Action onPressAction )
        {
            guiButtonDatas.Add( new GUIButtonData( buttonText, onPressAction ) );
        }

        public void Show( string inScreenName )
        {
            screenName = inScreenName;
            gameObject.SetActive( true );
            canvasBlocker.ShowFullScreen( backgroundColor );
        }

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
                guiButtonData.pressAction.Dispatch();
                Hide();
            }

            position.y += MARGIN * 2 + ButtonHeight;
        }

        readonly struct GUIButtonData
        {
            public readonly string buttonText;
            public readonly Action pressAction;

            public GUIButtonData( string buttonText, Action pressAction )
            {
                this.buttonText = buttonText;
                this.pressAction = pressAction;
            }
        }
    }
}