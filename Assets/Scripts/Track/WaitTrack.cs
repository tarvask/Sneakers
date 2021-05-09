using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sneakers
{
    public class WaitTrack : AbstractTrack
    {
        [SerializeField] protected Transform[] trackPoints;
        
        private float _waitTrackMovementSpeed;
        private bool _moveToWaste;
        
        private SneakerController _waitingSneaker;
        
        public void Init(SortingController sortingController, bool isAvailable, float waitTrackMovementSpeed, bool moveToWaste)
        {
            base.Init(sortingController, isAvailable);

            _waitTrackMovementSpeed = waitTrackMovementSpeed;
            _moveToWaste = moveToWaste;
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (sneaker.DragDropItem.IsHold)
            {
                sneaker.SetPosition(sneaker.DragDropItem.vector);
            }
            else if (_waitingSneaker != null)
            {
                // or maybe just send new sneaker back to it's roots
                //_movement.SendToMainTransporter(_waitingSneaker, 2);
                //_movement.SendToWaitTransporter(sneaker, 2);
                sneaker.SetPosition(sneaker.DragDropItem.vector);
                _sortingController.SendToMainTransporter(sneaker, sneaker.CurrentPoint);
            }
            else
            {
                _waitingSneaker = sneaker;
                sneaker.SetPosition(trackPoints[0].position);
                sneaker.DragDropItem.IsHold = true;
                _sortingController.SendToWaitTransporter(sneaker, 1);
            }
        }
        
        public IEnumerator WaitRoute(SneakerController sneaker, int mover)
        {
            StartCoroutine(CheckPresence(sneaker));
            
            if (!_moveToWaste)
            {
                if (sneaker.CurrentCoroutine != null)
                    sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
                
                yield break;
            }

            int baseWaitTransporterIndex = 0;
            
            while (mover == 1 || mover == 2)
            {
                while (Vector3.SqrMagnitude(trackPoints[baseWaitTransporterIndex + mover].localPosition - sneaker.LocalPosition)
                       > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[baseWaitTransporterIndex + mover].localPosition, _waitTrackMovementSpeed);
                    yield return null;
                }
                
                mover++;
            }
            
            if (sneaker.CurrentCoroutine != null)
                sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
            
            DropToWaste(sneaker);
        }

        private IEnumerator CheckPresence(SneakerController sneaker)
        {
            while (sneaker != null && sneaker.DragDropItem.IsHold)
            {
                yield return null;
            }
            
            _waitingSneaker = null;
            StopAllCoroutines();
        }

        private void DropToWaste(SneakerController sneaker)
        {
            _waitingSneaker = null;
            
            if (sneaker.State == SneakerState.Wasted)
            {
                _sortingController.OnSortSucceeded(sneaker);
            }
            else
            {
                if (sneaker.IsLegendary)
                    _sortingController.OnSortLegendaryError(sneaker);
                else
                    _sortingController.OnSortFailed(sneaker);
            }
        }
    }
}