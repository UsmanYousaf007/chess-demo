using System;
using HUF.GenericDialog.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;

namespace HUF.PolicyGuard.Runtime.Implementations
{
    public class GenericEventDialog : HGenericDialogInstance
    {
        /// <summary>
        /// Raised after a primary button is pressed.
        /// </summary>
        [PublicAPI]
        public event Action OnPrimaryButtonClicked;

        /// <summary>
        /// Raised after a secondary button is pressed.
        /// </summary>
        [PublicAPI]
        public event Action OnSecondaryButtonClicked;

        /// <summary>
        /// Raised after a tertiary button is pressed.
        /// </summary>
        [PublicAPI]
        public event Action OnTertiaryButtonClicked;

        protected override HLogPrefix LogPrefix => new HLogPrefix( nameof(GenericEventDialog) );

        public override void Close()
        {
            OnPrimaryButtonClicked = null;
            OnSecondaryButtonClicked = null;
            OnTertiaryButtonClicked = null;
            base.Close();
        }

        protected override void HandlePrimaryButtonClick()
        {
            OnPrimaryButtonClicked.Dispatch();
            OnClosePopup.Invoke();
            Close();
        }

        protected override void HandleSecondaryButtonClick()
        {
            OnSecondaryButtonClicked.Dispatch();
            OnClosePopup.Invoke();
            Close();
        }

        protected override void HandleTertiaryButtonClick()
        {
            OnTertiaryButtonClicked.Dispatch();
            OnClosePopup.Invoke();
            Close();
        }

    }
}