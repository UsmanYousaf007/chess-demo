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
    public class ExampleMassiveStringItem : Picker.MassiveZoomPickerItem
    {
        [SerializeField] ExampleMassiveStringPicker m_Parent = null;
        [SerializeField] int m_ColumnIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            m_Texts = GetComponentsInChildren<Text>();
        }

        protected Text[] m_Texts;

        string _GetText(int index)
        {
            if (m_Parent != null)
            {
                return m_Parent.GetText(m_ColumnIndex, index);
            }
                
            return "";
        }

        public override void SetItemContents(Picker.MassivePickerScrollRect scrollRect, int itemIndex)
        {
            if (m_Texts != null && m_Texts.Length > 0)
            {
                string t = _GetText(itemIndex);

                foreach (Text text in m_Texts)
                {
                    text.text = t;
                }
            }
        }
    }
}