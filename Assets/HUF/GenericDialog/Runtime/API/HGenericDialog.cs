using HUF.GenericDialog.Runtime.Configs;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.GenericDialog.Runtime.API
{
    public static class HGenericDialog
    {
        const string POSTPONE_FORMAT = "HGDPPone_{0}";
        const string HANDLED_FORMAT = "HGDHandled_{0}";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HGenericDialog) );

        static HGenericDialogInstance currentInstance;

        /// <summary>
        /// Shows a custom dialog described by the <paramref name="config"/>. Only one dialog can be displayed at a time.
        /// </summary>
        /// <param name="config">Config that provides info about dialog to display.</param>
        /// <param name="instance">Instance to be displayed or currently displayed one if any.</param>
        /// <returns>Whether or not the requested dialog was created and displayed.</returns>
        [PublicAPI]
        public static bool ShowDialog( HGenericDialogConfig config, out HGenericDialogInstance instance )
        {
            if ( currentInstance != null )
            {
                HLog.LogError( logPrefix, "Only one dialog can be opened at a time. Close existing instance first!\n"
                + $"Attempted to open {config.ConfigId} while {currentInstance.gameObject.name} was opened" );
                instance = currentInstance;
                return false;
            }

            currentInstance = Object.Instantiate<HGenericDialogInstance>( config.prefab );
            GameObject instanceObject = currentInstance.gameObject;
            instanceObject.AddComponent<DontDestroyOnLoad>();
            instanceObject.name = config.ConfigId;
            currentInstance.OnClosePopup.AddListener( HandlePopupClosed );
            currentInstance.Initialize( config );

            instance = currentInstance;
            return true;
        }

        /// <summary>
        /// Checks whether or not a display attempt meets configured schedule.
        /// </summary>
        /// <param name="config">Target dialog config.</param>
        /// <param name="session">Current session number (counts from 1).</param>
        /// <returns></returns>
        [PublicAPI]
        public static bool CheckSchedule( HGenericDialogConfig config, int session )
        {
            if ( config.showSchema == null )
                return true;

            var currentConfig =
                HConfigs.GetConfig<HGenericDialogShowSchema>( config.showSchema.ConfigId ) ?? config.showSchema;

            return !IsHandled( config ) && currentConfig.CanShow( session, GetPostponedSession( config.ConfigId ) );
        }

        /// <summary>
        /// Marks a given dialog as postponed. See <see cref="HGenericDialogShowSchema"/>.
        /// </summary>
        /// <param name="config">Target dialog config.</param>
        /// <param name="session">Current session (counts from 1).</param>
        [PublicAPI]
        public static void Postpone( HGenericDialogConfig config, int session )
        {
            PlayerPrefs.SetInt( string.Format( POSTPONE_FORMAT, config.ConfigId ), session );
        }

        /// <summary>
        /// Marks a given dialog as handled and makes CheckSchedule returns always false when handled is set to true.
        /// </summary>
        /// <param name="config">Target dialog config.</param>
        /// <param name="handled">Bool value whether certain dialog is handled.</param>
        [PublicAPI]
        public static void SetAsHandled( HGenericDialogConfig config, bool handled = true )
        {
            if( handled )
                PlayerPrefs.SetInt( string.Format( HANDLED_FORMAT, config.ConfigId ), 1 );
            else
                PlayerPrefs.DeleteKey( string.Format( HANDLED_FORMAT, config.ConfigId ) );
        }

        /// <summary>
        /// Checks if given dialog is handled
        /// </summary>
        /// <param name="config">Target dialog config.</param>
        /// <returns></returns>
        [PublicAPI]
        public static bool IsHandled( HGenericDialogConfig config )
        {
            return PlayerPrefs.HasKey( string.Format( HANDLED_FORMAT, config.ConfigId ) );
        }

        static int GetPostponedSession( string id )
        {
            return PlayerPrefs.GetInt( string.Format( POSTPONE_FORMAT, id ), int.MinValue );
        }

        static void HandlePopupClosed()
        {
            currentInstance = null;
        }
    }
}