using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sneakers
{
    public class WaitTrack : AbstractTrack
    {
        private float _waitTrackMovementSpeed;
        
        private List<SneakerModel> _waitList = new List<SneakerModel>();
        
        public void Init(Movement movement, float waitTrackMovementSpeed)
        {
            base.Init(movement);

            _waitTrackMovementSpeed = waitTrackMovementSpeed;
        }
        
        protected override void OnDropSneaker(SneakerModel sneaker)
        {
            if (sneaker.DragDropItem.isHold)
            {
                sneaker.transform.position = sneaker.DragDropItem.vector;
            }
            else
            {
                // if (_waitList.Count != 0)
                // {
                //     SneakerModel currentWaitingSneaker = _waitList[_waitList.Count - 1];
                //     _movement.SendToMainTransporter(currentWaitingSneaker, 2);
                //     _movement.SendToWaitTransporter(sneaker, 2);
                // }
                //
                // _waitList.Add(sneaker);
                sneaker.transform.position = _movement.points[9].position;
                _movement.SendToWaitTransporter(sneaker, 1);
                sneaker.DragDropItem.isHold = true;
            }
        }
        
        public IEnumerator WaitRoute(SneakerModel sneaker, int mover)
        {
            int baseWaitTransporterIndex = 9;
            
            if (mover == 1 || mover == 2)
            {
                while (_movement.points[baseWaitTransporterIndex + mover].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        _movement.points[baseWaitTransporterIndex + mover].position, _waitTrackMovementSpeed);
                    yield return new WaitForFixedUpdate();
                }
                
                sneaker.StopCoroutine(sneaker.route);
                Debug.Log("stopped");
            }
        }
    }
}