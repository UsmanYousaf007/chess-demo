using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace HUFEXT.GenericGDPR.Runtime.Utils
{
    [RequireComponent( typeof( TextMeshProUGUI ) )]
    public class Hyperlink : MonoBehaviour, IPointerClickHandler
    {
        TextMeshProUGUI hyperlink;
        
        private void Awake()
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