using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sneakers
{
    public abstract class AbstractTrack : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Image[] colliderImages;

        protected Movement _movement;
        private bool _isAvailable;

        public void Init(Movement movement, bool isAvailable)
        {
            _movement = movement;
            _isAvailable = isAvailable;

            foreach (Image colliderImage in colliderImages)
            {
                colliderImage.color = new Color(0, 0, 0,
                    _isAvailable ? 0 : 0.5f);
            }
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (!_isAvailable || eventData.pointerDrag == null)
                return;
            
            SneakerModel sneaker = eventData.pointerDrag.GetComponent<SneakerModel>();

            if (sneaker == null)
                return;
            
            // end drag
            sneaker.DragDropItem.isDropped = true;

            OnDropSneaker(sneaker);
        }

        protected abstract void OnDropSneaker(SneakerModel sneaker);

        protected virtual void OnWrongTrackDropped(SneakerModel sneaker)
        {
            // back to position before drag
            if (!sneaker.DragDropItem.isHold)
            {
                sneaker.transform.position = sneaker.DragDropItem.vector;
                _movement.SendToMainTransporter(sneaker, sneaker.currentPoint);
            }
            // back to wait
            else
            {
                sneaker.transform.position = sneaker.DragDropItem.vector;
            }
        }
    }
}
