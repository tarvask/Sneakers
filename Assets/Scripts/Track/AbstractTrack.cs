﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Sneakers
{
    public abstract class AbstractTrack : MonoBehaviour, IDropHandler
    {
        //public Transform[] points;
        
        protected Movement _movement;

        public void Init(Movement movement)
        {
            _movement = movement;
        }
        
        public virtual void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
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
