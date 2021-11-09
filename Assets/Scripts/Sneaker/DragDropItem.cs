using UnityEngine;
using UnityEngine.EventSystems;

namespace Sneakers
{
    public class DragDropItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
    {
        private const float CollisionResolveSafeDistance = 200f;

        private const float CollisionResolveSafeDistanceSqr = CollisionResolveSafeDistance * CollisionResolveSafeDistance;
        
        public Vector2 vector;
        public bool isDropped;

        public bool IsDragged
        {
            get;
            private set;
        }

        public bool IsHold
        {
            get;
            set;
        }
    
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private SneakerController _sneaker;
    
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Init(SneakerController sneaker, Canvas canvas)
        {
            _sneaker = sneaker;
            _canvas = canvas;
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_sneaker.CurrentCoroutine != null)
                _sneaker.View.StopCoroutine(_sneaker.CurrentCoroutine);
            
            _canvasGroup.alpha = 0.4f;
            _canvasGroup.blocksRaycasts = false;
            vector = transform.localPosition;
            IsDragged = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            isDropped = false;
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
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
                    // do not return sneaker from wait track to main
                    if (_sneaker.DragDropItem.IsHold)
                        SortingController.instance.SendToWaitTransporter(_sneaker, _sneaker.CurrentPoint);
                    else
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
            // drag is ended on next frame with the start of coroutine
            //IsDragged = false;
        }

        public void StopDrag()
        {
            IsDragged = false;
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

        private void OnTriggerEnter2D(Collider2D otherSneakerCollider)
        {
            DragDropItem otherSneakerDragDrop = otherSneakerCollider.gameObject.GetComponent<DragDropItem>();
            ResolveCollision(otherSneakerDragDrop);
        }

        private void OnTriggerStay2D(Collider2D otherSneakerCollider)
        {
            DragDropItem otherSneakerDragDrop = otherSneakerCollider.gameObject.GetComponent<DragDropItem>();
            ResolveCollision(otherSneakerDragDrop);
        }

        private void ResolveCollision(DragDropItem otherSneakerDragDrop)
        {
            // do not resolve collisions will one of participants is dragged
            if (IsDragged || otherSneakerDragDrop.IsDragged)
                return;
            
            // do not resolve collisions of sneakers from different tracks
            if (otherSneakerDragDrop._sneaker.CurrentTargetPosition != _sneaker.CurrentTargetPosition)
                return;

            // do not resolve collisions for participants in safe distance
            if ((otherSneakerDragDrop._sneaker.LocalPosition - _sneaker.LocalPosition).sqrMagnitude >= CollisionResolveSafeDistanceSqr)
                return;

            Vector3 thisSneakerDestination = _sneaker.CurrentTargetPosition - _sneaker.LocalPosition;
            float thisSneakerDistanceToTargetSqr = thisSneakerDestination.sqrMagnitude;
            Vector3 otherSneakerDestination = otherSneakerDragDrop._sneaker.CurrentTargetPosition -
                                              otherSneakerDragDrop._sneaker.LocalPosition;
            float otherSneakerDistanceToTargetSqr = otherSneakerDestination.sqrMagnitude;

            if (thisSneakerDistanceToTargetSqr < otherSneakerDistanceToTargetSqr)
            {
                // move back other
                Vector3 targetPosition = otherSneakerDragDrop._sneaker.LocalPosition - otherSneakerDestination;
                otherSneakerDragDrop._sneaker.MoveByDelta(_sneaker.LocalPosition,
                    targetPosition, CollisionResolveSafeDistance);
            }
            else
            {
                // move back this
                Vector3 targetPosition = _sneaker.LocalPosition - thisSneakerDestination;
                _sneaker.MoveByDelta(otherSneakerDragDrop._sneaker.LocalPosition,
                    targetPosition, CollisionResolveSafeDistance);
            }
        }
    }
}
