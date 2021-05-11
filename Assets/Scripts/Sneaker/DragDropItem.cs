using UnityEngine;
using UnityEngine.EventSystems;

namespace Sneakers
{
    public class DragDropItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
    {
        public Canvas canvas;
        public Vector2 vector;
        public bool isDropped;

        public bool IsHold
        {
            get;
            set;
        }
    
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
            if (_sneaker.CurrentCoroutine != null)
                _sneaker.View.StopCoroutine(_sneaker.CurrentCoroutine);
            
            _canvasGroup.alpha = 0.4f;
            _canvasGroup.blocksRaycasts = false;
            vector = transform.localPosition;
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
                transform.localPosition = vector;
                if (_sneaker.TransporterType == TransporterType.Main)
                {
                    SortingController.instance.SendToMainTransporter(_sneaker, _sneaker.CurrentPoint, _sneaker.DragDropItem.IsHold);
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_sneaker.IsLegendary)
                return;

            if (_sneaker.State == SneakerState.Normal)
                _sneaker.CollectLegendary();
        }
    }
}
