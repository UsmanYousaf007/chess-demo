using System;
using HUF.GenericDialog.Runtime.Configs;
using HUF.Utils.Runtime.Logging;
#if HUF_TRANSLATION_SYSTEM
using HUF.TranslationSystem.Runtime.API;
#endif
using HUF.Utils.Runtime.UnityEvents;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HUF.GenericDialog.Runtime.API
{

    /// <summary>
    /// Class used for marking HGenericDialog's prefabs and binding UI with a logic
    /// </summary>
    [PublicAPI]
    public abstract class HGenericDialogInstance : MonoBehaviour
    {
        public const string COLOR_TAG = "{COLOR}";

        /// <summary>
        /// <see cref="Color"/> of links in texts.
        /// </summary>
        [PublicAPI]
        [SerializeField] Color linkColor;

        /// <summary>
        /// <see cref="Button"/> reference that acts as a primary (accented) button.
        /// </summary>
        [PublicAPI]
        [SerializeField] Button primaryButtonHandler = null;

        /// <summary>
        /// <see cref="Button"/> reference that acts as a secondary (blended) button.
        /// </summary>
        [PublicAPI]
        [SerializeField] Button secondaryButtonHandler = null;

        /// <summary>
        /// <see cref="Button"/> reference that acts as a tertiary (denied) button.
        /// </summary>
        [PublicAPI]
        [SerializeField] Button tertiaryButtonHandler = null;

        /// <summary>
        /// Root object that gets deactivated if <see cref="HGenericDialogConfig"/> specifies so.
        /// </summary>
        [PublicAPI]
        [SerializeField] GameObject secondaryButtonRoot = null;

        /// <summary>
        /// Root object that gets deactivated if <see cref="HGenericDialogConfig"/> specifies so.
        /// </summary>
        [PublicAPI]
        [SerializeField] GameObject tertiaryButtonRoot = null;

        [NonSerialized] protected HGenericDialogConfig config = null;

        /// <summary>
        /// <para>A function that can be used to obtain current session number.
        /// Should be provided right after the dialog is shown.</para>
        /// <para>It is not mandatory. If not provided postponing will be disabled.</para>
        /// </summary>
        [PublicAPI]
        public Func<int> SessionProvider { get; set; }

        /// <summary>
        /// Log prefix used for logging
        /// </summary>
        [PublicAPI]
        protected abstract HLogPrefix LogPrefix { get; }

        /// <summary>
        /// <para>Event that indicates that the dialog should be closed. Can be used to trigger a closing animation.</para>
        /// <para>Whether an animation is used or not a <see cref="Destroy"/> function must be called
        /// to dispose of the dialog instance.</para>
        /// </summary>
        [PublicAPI]
        public UnityEvent OnClosePopup;

        /// <summary>
        /// Event triggered to set the content of the dialog's header.
        /// </summary>
        [PublicAPI]
        public StringUnityEvent OnHeaderTextOverride;

        /// <summary>
        /// Event triggered to set the dialog's content
        /// </summary>
        [PublicAPI]
        public StringUnityEvent OnContentTextOverride;

        /// <summary>
        /// Event triggered to set the content of the primary button.
        /// </summary>
        [PublicAPI]
        public StringUnityEvent OnPrimaryTextOverride;

        /// <summary>
        /// Event triggered to set the content of the secondary button.
        /// </summary>
        [PublicAPI]
        public StringUnityEvent OnSecondaryTextOverride;

        /// <summary>
        /// Event triggered to set the content of the tertiary button.
        /// </summary>
        [PublicAPI]
        public StringUnityEvent OnTertiaryTextOverride;

        public void Initialize( HGenericDialogConfig config )
        {
            this.config = config;

            if ( primaryButtonHandler != null )
                primaryButtonHandler.onClick.AddListener( HandlePrimaryButtonClick );

            if ( secondaryButtonRoot != null )
            {
                secondaryButtonRoot.SetActive( config.showSecondaryButton );

                if ( secondaryButtonHandler != null )
                    secondaryButtonHandler.onClick.AddListener( HandleSecondaryButtonClick );
            }

            if ( tertiaryButtonRoot != null )
            {
                tertiaryButtonRoot.SetActive( config.showTertiaryButton );

                if ( tertiaryButtonHandler != null )
                    tertiaryButtonHandler.onClick.AddListener( HandleTertiaryButtonClick );
            }

            HandleTranslation( config );
            HandleInitialization( config );
        }

        /// <summary>
        /// Destroys the GameObject this component is attached to.
        /// </summary>
        [PublicAPI]
        public void Destroy()
        {
            Destroy( this.gameObject );
        }

        /// <summary>
        /// Sends <see cref="OnClosePopup"/> event and destroys the popup.
        /// </summary>
        [PublicAPI]
        public virtual void Close()
        {
            OnClosePopup?.Invoke();
            Destroy();
        }

        /// <summary>
        /// Called after the component is instantiated and initialized by the <see cref="HGenericDialog.ShowDialog"/> method
        /// but before the instance is returned to the caller.
        /// </summary>
        /// <param name="config"></param>
        [PublicAPI]
        protected virtual void HandleInitialization( HGenericDialogConfig config ) { }

        /// <summary>
        /// Called during initialization to allow handling of custom and complex translations.
        /// </summary>
        /// <param name="config"></param>
        [PublicAPI]
        protected virtual void HandleTranslation( HGenericDialogConfig config )
        {
#if HUF_TRANSLATION_SYSTEM
            var getContent = new Func<string, string>( ( id ) =>
            {
                string text = HTranslator.GetContent( id );
                return text?.Length > 0 ? text : id;
            } );
#else
            var getContent = new Func<string, string>( ( id ) => id );
#endif
            OnHeaderTextOverride?.Invoke( getContent(
                config.headerTranslation.Replace( COLOR_TAG, $"#{ColorUtility.ToHtmlStringRGBA( linkColor )}" ) ) );

            OnContentTextOverride?.Invoke( getContent(
                config.contentTranslation.Replace( COLOR_TAG, $"#{ColorUtility.ToHtmlStringRGBA( linkColor )}" ) ) );

            OnPrimaryTextOverride?.Invoke( getContent(
                config.primaryButtonTranslation.Replace( COLOR_TAG,
                    $"#{ColorUtility.ToHtmlStringRGBA( linkColor )}" ) ) );

            OnSecondaryTextOverride?.Invoke( getContent(
                config.secondaryButtonTranslation.Replace( COLOR_TAG,
                    $"#{ColorUtility.ToHtmlStringRGBA( linkColor )}" ) ) );

            OnTertiaryTextOverride?.Invoke( getContent(
                config.tertiaryButtonTranslation.Replace( COLOR_TAG,
                    $"#{ColorUtility.ToHtmlStringRGBA( linkColor )}" ) ) );
        }

        /// <summary>
        /// Called when user clicks the primary button.
        /// </summary>
        [PublicAPI]
        protected abstract void HandlePrimaryButtonClick();

        /// <summary>
        /// Called when user clicks the secondary button.
        /// </summary>
        [PublicAPI]
        protected abstract void HandleSecondaryButtonClick();

        /// <summary>
        /// Called when user clicks the Tertiary button.
        /// </summary>
        [PublicAPI]
        protected abstract void HandleTertiaryButtonClick();

        /// <summary>
        /// Marks the dialog as postponed.
        /// </summary>
        [PublicAPI]
        protected void Postpone()
        {
            if ( SessionProvider == null )
                HLog.LogWarning( LogPrefix, "Postponing functions are disabled without a session provider." );
            else
                HGenericDialog.Postpone( config, SessionProvider() );
        }
    }
}