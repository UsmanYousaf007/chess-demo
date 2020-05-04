namespace HUFEXT.PackageManager.Editor.Views
{
    public abstract class PackageManagerView
    {
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

        public virtual void OnEventEnter( ViewEvent ev )
        {
            Utils.Common.Log( $"OnEventEnter: {GetType().Name} via {ev.ToString()}" );
        }
        
        public virtual void RefreshView( ViewEvent ev )
        {
            Utils.Common.Log( $"RefreshView: {GetType().Name} via {ev.ToString()}" );
        }
        
        protected virtual void OnGUI() { }
    }
}
