﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Sneakers
{
    public class DragDropItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        public Canvas canvas;
        public Vector2 vector;
        public bool isDropped;
        public bool isHold;
    
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private SneakerModel _sneaker;
    
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _sneaker = gameObject.GetComponent<SneakerModel>();
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            _sneaker.StopCoroutine(_sneaker.route);
            _canvasGroup.alpha = 0.4f;
            _canvasGroup.blocksRaycasts = false;
            vector = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }

        public void OnDrop(PointerEventData eventData)
        {
       
        }
    }
}
