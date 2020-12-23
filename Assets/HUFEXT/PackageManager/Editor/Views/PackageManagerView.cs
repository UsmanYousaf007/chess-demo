using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public abstract class PackageManagerView
    {
        protected PackageManagerWindow window = null;

        protected PackageManagerView( PackageManagerWindow parent )
        {
            window = parent;
        }

        public virtual Models.PackageManagerViewType Type => Models.PackageManagerViewType.Unknown;

        public void Repaint()
        {
            if ( window == null )
            {
                return;
            }

            OnGUI();
        }

        public virtual void OnEventCompleted( Models.PackageManagerViewEvent ev ) { }

        protected void RegisterEvent( Models.PackageManagerViewEvent ev )
        {
            if ( window == null )
            {
                return;
            }

            window.RegisterEvent( ev );
        }

        protected virtual void OnGUI() { }
    }
}