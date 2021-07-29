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
	public class ExampleYearAndMonthPicker : ExampleMassiveStringPicker
    {
        [SerializeField] MassivePickerScrollRect m_PickerScrollRect = null;

        System.DateTime m_BaseMonth = System.DateTime.Now;
        
        protected System.DateTime _IndexToDate(int index)
        {
            int num = m_PickerScrollRect.itemCount;
            int monthOffset = index - num / 2;

            return m_BaseMonth.AddMonths(monthOffset);
        }

        public override string GetText( int columnIndex, int index)
        {
            System.DateTime date = _IndexToDate(index);
            return date.ToString("MMMM yyyy");
        }

        public override void OnSelectItem( int index )
        {
            System.DateTime date = _IndexToDate(index);
#if UNITY_EDITOR
            Debug.Log("Select " + date.ToString("MMMM yyyy"));
#endif
        }
	}

}
