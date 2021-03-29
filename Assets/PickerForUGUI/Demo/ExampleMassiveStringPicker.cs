using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Picker
{
	public class ExampleMassiveStringPicker : MonoBehaviour
    {
        public virtual string GetText(int columnIndex, int index)
        {
            return "Item" + columnIndex + "-" + index;
        }

        public virtual void OnSelectItem( int index )
        {
        }
	}

}
