using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvenDropUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private Image img;
    private RectTransform rect;

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.color = Color.blue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = Color.white;
    }

    void Awake()
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }
}
