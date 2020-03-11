using HUFEXT.CrossPromo.Implementation.View.BottomPanel;
using HUFEXT.CrossPromo.Implementation.View.ContentPanel;
using HUFEXT.CrossPromo.Implementation.View.TopPanel;
using UnityEngine;

namespace HUFEXT.CrossPromo.Implementation
{
    public class CrossPromoView : MonoBehaviour
    {
        [SerializeField] TopPanelContainer topPanelContainer = default;
        [SerializeField] ContentPanelContainer contentPanelContainer = default;
        //[SerializeField] BottomPanelContainer bottomPanelContainer = default;
        
        [SerializeField] BottomPanelContainer bottomPanelContainerDefaultPrefab = default;

        public TopPanelContainer TopPanelContainer => topPanelContainer;
        public ContentPanelContainer ContentPanelContainer => contentPanelContainer;
        public BottomPanelContainer BottomPanelContainer;

        public BottomPanelContainer BottomPanelContainerDefaultPrefab => bottomPanelContainerDefaultPrefab;

    }
}