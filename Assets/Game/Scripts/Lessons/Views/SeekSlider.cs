using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TurboLabz.InstantGame
{
    public class SeekSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Slider _slider;
        
        private Action SeekStartEvent;
        private Action<float> SeekEndEvent;

        public void Init(Action seekStartEvent, Action<float> seekEndEvent)
        {
            _slider = GetComponent<Slider>();
            _slider.normalizedValue = 0f;

            SeekStartEvent = seekStartEvent;
            SeekEndEvent = seekEndEvent;
        }

        public float GetNormalizedValue()
        {
            return _slider.normalizedValue;
        }

        public void OnPointerDown(PointerEventData e)
        {
            SeekStartEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData e)
        {
            SeekEndEvent?.Invoke(_slider.normalizedValue);
        }

        public void UpdateSliderPosition(float normalizedValue)
        {
            _slider.normalizedValue = normalizedValue;
        }
    }
}
