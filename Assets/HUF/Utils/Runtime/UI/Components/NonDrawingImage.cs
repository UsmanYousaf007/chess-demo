using UnityEngine.UI;

namespace HUF.Utils.Runtime.UI.Components
{
    public class NonDrawingImage: Graphic
    {
        public override void SetMaterialDirty()
        {
            return;
        }
        public override void SetVerticesDirty()
        {
            return;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            return;
        }
    }
}