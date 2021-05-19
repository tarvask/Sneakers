using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class WaitTrack : AbstractSpecialTrack
    {
        private float _waitTrackMovementSpeed;
        private bool _moveToWaste;
        
        public void Init(SortingController sortingController, bool isAvailable, float waitTrackMovementSpeed, bool moveToWaste)
        {
            base.Init(sortingController, isAvailable);

            _waitTrackMovementSpeed = waitTrackMovementSpeed;
            _moveToWaste = moveToWaste;
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (IsBusy)
            {
                sneaker.SetPosition(sneaker.DragDropItem.vector);
                
                if (!sneaker.DragDropItem.IsHold)
                    _sortingController.SendToMainTransporter(sneaker, sneaker.CurrentPoint);
            }
            else
            {
                StartProcessingSneaker(sneaker);
                sneaker.SetPosition(trackPoints[0].localPosition);
                sneaker.DragDropItem.IsHold = true;
                _sortingController.SendToWaitTransporter(sneaker, 1);
            }
        }
        
        protected override void OnWrongTrackDropped(SneakerController sneaker)
        {
            // back to position before drag
            if (!sneaker.DragDropItem.IsHold)
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
        
        public IEnumerator WaitRoute(SneakerController sneaker, int mover)
        {
            sneaker.OnRouteStart();
            
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

        private void DropToWaste(SneakerController sneaker)
        {
            StopProcessingSneaker();
            
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