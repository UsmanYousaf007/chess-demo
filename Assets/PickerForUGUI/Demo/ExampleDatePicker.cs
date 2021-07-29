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
	public class ExampleDatePicker : ExampleMassiveStringPicker
    {
        [SerializeField] MassivePickerScrollRect m_DayPickerScrollRect = null;
        [SerializeField] MassivePickerScrollRect m_HourPickerScrollRect = null;
        [SerializeField] MassivePickerScrollRect m_MinutePickerScrollRect = null;

        System.DateTime m_BaseDay = System.DateTime.Now.Date;
        
        protected System.DateTime _IndexToDate(int dayIndex, int hourIndex, int minuteIndex)
        {
            int num = m_DayPickerScrollRect.itemCount;
            int dayOffset = dayIndex - num / 2;
            return m_BaseDay.AddDays(dayOffset).AddHours(hourIndex).AddMinutes(minuteIndex);
        }

        public override string GetText( int columnIndex, int index)
        {
            switch( columnIndex )
            {
                //day
                case 0:
                    {
                        System.DateTime date = _IndexToDate(index, 0, 0);
                        return date.ToString("ddd d MMM");
                    }

                //hour
                case 1:
                    return string.Format("{0:00}", index);

                //minute
                case 2:
                    return string.Format("{0:00}", index);

                default:
                    return "";
            }
        }

        public override void OnSelectItem( int index )
        {
            System.DateTime date = _IndexToDate(
                m_DayPickerScrollRect.GetSelectedItemIndex(),
                m_HourPickerScrollRect.GetSelectedItemIndex(),
                m_MinutePickerScrollRect.GetSelectedItemIndex()
                );
#if UNITY_EDITOR
            Debug.Log("Select " + date.ToString());
#endif
        }
	}

}
