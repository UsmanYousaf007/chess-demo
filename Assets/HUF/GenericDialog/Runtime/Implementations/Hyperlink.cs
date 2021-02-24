using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HUF.GenericDialog.Runtime.Implementations
{
    [RequireComponent( typeof( TextMeshProUGUI ) )]
    public class Hyperlink : MonoBehaviour, IPointerClickHandler
    {
        TextMeshProUGUI hyperlink;
        
        void Awake()
        {
            hyperlink = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerClick( PointerEventData eventData )
        {
            var index = TMP_TextUtilities.FindIntersectingLink( hyperlink, eventData.position, null );
            if ( index != -1 )
            {
                Application.OpenURL( hyperlink.textInfo.linkInfo[index].GetLinkID() );
            }
        }
    }
}