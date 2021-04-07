using System.Collections.Generic;
using HUF.Notifications.Runtime.Data.Structs;
using HUF.NotificationsUnity.Runtime.Implementation.Dummy;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEditor;
using UnityEngine;

namespace HUF.Notifications.Editor
{
    public class EditorNotificationHelper : EditorWindow
    {
        const int FILES_WIDTH = 170;
        List<NotificationData> notifications = new List<NotificationData>();

        [MenuItem( "HUF/Windows/Notifications Helper" )]
        static void OpenSimulatorWindow()
        {
            var editorWindow = (EditorNotificationHelper)GetWindow(
                typeof(EditorNotificationHelper),
                false,
                "Notifications Helper" );
            editorWindow.RefreshNotifications();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            DrawLabels();
            DrawNotifications();
            GUILayout.EndVertical();
            DrawButtons();
        }

        void DrawButtons()
        {
            if ( GUILayout.Button( "Refresh" ) )
            {
                RefreshNotifications();
            }

            if ( GUILayout.Button( "Clear" ) )
            {
                HPlayerPrefs.SetInt( DummyNotificationsService.EDITOR_NOTIFICATIONS_VALUES_KEY, 0 );
                RefreshNotifications();
            }

            if ( GUILayout.Button(
                $"Set auto clear notification to {!HPlayerPrefs.GetBool( DummyNotificationsService.EDITOR_NOTIFICATION_CAN_CLEAN_KEY )}" ) )
            {
                HPlayerPrefs.SetBool( DummyNotificationsService.EDITOR_NOTIFICATION_CAN_CLEAN_KEY,
                    !HPlayerPrefs.GetBool( DummyNotificationsService.EDITOR_NOTIFICATION_CAN_CLEAN_KEY ) );
                RefreshNotifications();
            }
        }

        void DrawNotifications()
        {
            for ( int i = 0; i < notifications.Count; i++ )
            {
                DrawNotification( notifications[i] );
            }
        }

        void DrawNotification( NotificationData notification )
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField( notification.title, GUILayout.Width( FILES_WIDTH ) );
            EditorGUILayout.TextField( notification.text, GUILayout.Width( FILES_WIDTH ) );
            EditorGUILayout.IntField( notification.delayInSeconds, GUILayout.Width( FILES_WIDTH ) );
            EditorGUILayout.FloatField( notification.delayInSeconds / 60, GUILayout.Width( FILES_WIDTH ) );
            EditorGUILayout.FloatField( notification.delayInSeconds / 3600, GUILayout.Width( FILES_WIDTH ) );
            GUILayout.EndHorizontal();
        }

        static void DrawLabels()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Title", GUILayout.Width( FILES_WIDTH ) );
            EditorGUILayout.LabelField( "Text", GUILayout.Width( FILES_WIDTH ) );
            EditorGUILayout.LabelField( "Delay in seconds", GUILayout.Width( FILES_WIDTH ) );
            EditorGUILayout.LabelField( "Delay in minutes", GUILayout.Width( FILES_WIDTH ) );
            EditorGUILayout.LabelField( "Delay in hours", GUILayout.Width( FILES_WIDTH ) );
            GUILayout.EndHorizontal();
        }

        public void RefreshNotifications()
        {
            int notificationsCount =
                HPlayerPrefs.GetInt( DummyNotificationsService.EDITOR_NOTIFICATIONS_VALUES_KEY );
            notifications.Clear();

            for ( int i = 0; i < notificationsCount; i++ )
            {
                notifications.Add( JsonUtility.FromJson<NotificationData>(
                    HPlayerPrefs.GetString( $"{DummyNotificationsService.EDITOR_NOTIFICATION_KEY}{i}" ) ) );
            }
        }
    }
}