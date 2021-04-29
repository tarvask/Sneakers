using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sneakers
{
    public abstract class AbstractTrack : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Image[] colliderImages;

        protected SortingController _sortingController;
        private bool _isAvailable;

        public void Init(SortingController sortingController, bool isAvailable)
        {
            _sortingController = sortingController;
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
            
            SneakerView sneaker = eventData.pointerDrag.GetComponent<SneakerView>();

            if (sneaker == null)
                return;
            
            // end drag
            sneaker.DragDropItem.isDropped = true;

            sneaker.OnDropSneakerEventHandler(OnDropSneaker);
        }

        protected abstract void OnDropSneaker(SneakerController sneaker);

        protected virtual void OnWrongTrackDropped(SneakerController sneaker)
        {
            // back to position before drag
            if (!sneaker.DragDropItem.isHold)
            {
                sneaker.SetPosition(sneaker.DragDropItem.vector);
                _sortingController.SendToMainTransporter(sneaker, sneaker.CurrentPoint);
            }
            // back to wait
            else
            {
                sneaker.SetPosition(sneaker.DragDropItem.vector);
            }
        }
    }
}
