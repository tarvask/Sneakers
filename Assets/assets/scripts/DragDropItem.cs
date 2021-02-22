using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Vector2 vector = new Vector2();
    public bool isDropped = false;
    public bool isHold = false;
    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
       
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        StopCoroutine(eventData.pointerDrag.GetComponent<SneakerScript>().route);
        canvasGroup.alpha = 0.4F;
        canvasGroup.blocksRaycasts = false;
        vector = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        canvasGroup.alpha = 1F;
        canvasGroup.blocksRaycasts = true;
        if (!isDropped)
        {
            transform.position = vector;
            if(gameObject.GetComponent<SneakerScript>().route_index == 0)
            {
                gameObject.GetComponent<SneakerScript>().route = StartCoroutine(Movement.instance.MainRoute(gameObject, gameObject.GetComponent<SneakerScript>().currentPoint));
            }
            if(gameObject.GetComponent<SneakerScript>().route_index == 1)
            {
                gameObject.GetComponent<SneakerScript>().route = StartCoroutine(Movement.instance.WashRoute(gameObject, gameObject.GetComponent<SneakerScript>().currentPoint));
            }
            if(gameObject.GetComponent<SneakerScript>().route_index == 2)
            {
                gameObject.GetComponent<SneakerScript>().route = StartCoroutine(Movement.instance.ShnurRoute(gameObject, gameObject.GetComponent<SneakerScript>().currentPoint));
            }
        }
        
       
    }


    public void OnDrop(PointerEventData eventData)
    {
       
    }
}
