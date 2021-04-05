using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollRectAlphaHandler : MonoBehaviour
{
    public RectTransform scrollRect;

    private List<CanvasGroup> _scrollItems;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        _scrollItems = new List<CanvasGroup>();
    }

    public void AddScrollItem(GameObject item)
    {
        if (_scrollItems == null)
        {
            Reset();
        }

        var cg = item.GetComponent<CanvasGroup>();

        if (cg == null)
        {
            cg = item.AddComponent<CanvasGroup>();
        }

        _scrollItems.Add(cg);
    }

    public void OnScroll(Vector2 position)
    {
        var center = scrollRect.position.y;
        var height = scrollRect.sizeDelta.y * 0.5f;
        var top = center + height;
        var bottom = center - height;

        foreach (var item in _scrollItems)
        {
            var y = item.transform.position.y;
            var itemHeight = ((RectTransform)item.transform).sizeDelta.y * 0.5f;

            if( y + itemHeight > top || y - itemHeight < bottom)
            {
                item.alpha = 0.3f;
                continue;
            }

            item.alpha = 1 - (0.5f * (Mathf.Abs(y - center) / height));
        }
    }

}
