using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sneakers
{
    public class WaitTrack : AbstractTrack
    {
        [SerializeField] protected Transform[] trackPoints;
        
        private float _waitTrackMovementSpeed;
        
        private SneakerController _waitingSneaker;
        
        public void Init(Movement movement, bool isAvailable, float waitTrackMovementSpeed)
        {
            base.Init(movement, isAvailable);

            _waitTrackMovementSpeed = waitTrackMovementSpeed;
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (sneaker.DragDropItem.isHold)
            {
                sneaker.SetPosition(sneaker.DragDropItem.vector);
            }
            else if (_waitingSneaker != null)
            {
                // or maybe just send new sneaker back to it's roots
                //_movement.SendToMainTransporter(_waitingSneaker, 2);
                //_movement.SendToWaitTransporter(sneaker, 2);
                sneaker.SetPosition(sneaker.DragDropItem.vector);
                _movement.SendToMainTransporter(sneaker, sneaker.CurrentPoint);
            }
            else
            {
                _waitingSneaker = sneaker;
                sneaker.SetPosition(trackPoints[0].position);
                _movement.SendToWaitTransporter(sneaker, 1);
                sneaker.DragDropItem.isHold = true;
            }
        }
        
        public IEnumerator WaitRoute(SneakerController sneaker, int mover)
        {
            int baseWaitTransporterIndex = 0;
            
            while (mover == 1 || mover == 2)
            {
                while (trackPoints[baseWaitTransporterIndex + mover].position != sneaker.Position)
                {
                    sneaker.Move(trackPoints[baseWaitTransporterIndex + mover].position, _waitTrackMovementSpeed);
                    yield return null;
                }
                
                mover++;
            }
            
            sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
        }
    }
}