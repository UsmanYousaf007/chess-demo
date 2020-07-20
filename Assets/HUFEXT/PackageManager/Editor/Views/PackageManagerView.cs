
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    public abstract class PackageManagerView
    {
        public virtual Models.PackageManagerViewType Type => Models.PackageManagerViewType.Unknown;
        protected PackageManagerWindow window = null;

        protected PackageManagerView( PackageManagerWindow parent )
        {
            window = parent;
        }
        
        public void Repaint()
        {
            if ( window == null )
            {
                return;
            }
            
            OnGUI();
        }

        protected void RegisterEvent( Models.PackageManagerViewEvent ev )
        {
            if ( window == null )
            {
                return;
            }
            
            window.RegisterEvent( ev );
        }

        public virtual void OnEventCompleted( Models.PackageManagerViewEvent ev )
        {
            
        }
        
        protected virtual void OnGUI() { }
    }
}