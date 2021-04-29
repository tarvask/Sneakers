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
        private SneakerController _sneaker;
    
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Init(SneakerController sneaker)
        {
            _sneaker = sneaker;
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            _sneaker.View.StopCoroutine(_sneaker.CurrentCoroutine);
            _canvasGroup.alpha = 0.4f;
            _canvasGroup.blocksRaycasts = false;
            vector = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            isDropped = false;
            _rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            
            if (!isDropped)
            {
                transform.position = vector;
                if (_sneaker.TransporterType == TransporterType.Main)
                {
                    SortingController.instance.SendToMainTransporter(_sneaker, _sneaker.CurrentPoint);
                }
                if (_sneaker.TransporterType == TransporterType.Washing)
                {
                    SortingController.instance.SendToWashTransporter(_sneaker, _sneaker.CurrentPoint);
                }
                if (_sneaker.TransporterType == TransporterType.Lacing)
                {
                    SortingController.instance.SendToLaceTransporter(_sneaker, _sneaker.CurrentPoint);
                }
            }

            isDropped = false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            
        }
    }
}
