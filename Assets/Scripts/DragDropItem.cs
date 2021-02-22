using UnityEngine;
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
            StopCoroutine(eventData.pointerDrag.GetComponent<SneakerModel>().route);
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
        
            if (!isDropped)
            {
                transform.position = vector;
                if (_sneaker.route_index == 0)
                {
                    _sneaker.route = StartCoroutine(Movement.instance.MainRoute(gameObject, _sneaker.currentPoint));
                }
                if (_sneaker.route_index == 1)
                {
                    _sneaker.route = StartCoroutine(Movement.instance.WashRoute(gameObject, _sneaker.currentPoint));
                }
                if (_sneaker.route_index == 2)
                {
                    _sneaker.route = StartCoroutine(Movement.instance.ShnurRoute(gameObject, _sneaker.currentPoint));
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
       
        }
    }
}
