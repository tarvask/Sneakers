using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class MainTrack : AbstractTrack
    {
        [SerializeField] private Transform[] trackPoints;
        [SerializeField] private MainTrackAnimator trackAnimator;
        
        private float _mainTrackMovementSpeed;

        public void Init(SortingController sortingController, bool isAvailable, float mainTrackMovementSpeed)
        {
            base.Init(sortingController, isAvailable);

            _mainTrackMovementSpeed = mainTrackMovementSpeed;
            
            trackAnimator.SetSpeed(_mainTrackMovementSpeed);
        }

        public void Stop()
        {
            trackAnimator.SetSpeed(-1);
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (sneaker.View.DragDropItem.IsHold)
                return;

            OnWrongTrackDropped(sneaker);
        }
        
        public IEnumerator MainRoute(SneakerController sneaker, int mover, bool isImmediately = false)
        {
            // ugly safety, in case of moving from side track to Wait track and then to Main
            if (mover >= 3)
            {
                mover = 0;
                sneaker.SetCurrentPoint(0);
                isImmediately = true;
            }

            //sneaker.GetComponent<DragDropItem>().isDropped = false;
            sneaker.SetTransporterType(TransporterType.Main);
            sneaker.OnRouteStart();
            
            if (isImmediately)
                sneaker.SetPosition(trackPoints[sneaker.CurrentPoint].localPosition);
            
            while (mover == 0 || mover == 1 || mover == 2)
            {
                sneaker.SetCurrentPoint(mover);
                while (Vector3.SqrMagnitude(trackPoints[sneaker.CurrentPoint + 1].localPosition - sneaker.LocalPosition)
                       > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[sneaker.CurrentPoint + 1].localPosition, _mainTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 3)
            {
                sneaker.SetCurrentPoint(3);
                
                if (!sneaker.IsLegendary)
                    _sortingController.OnSortFailed(sneaker);
                else
                    _sortingController.OnSortLegendaryError(sneaker);
            }
        }

        public void SpeedUp(float speedUpDelta)
        {
            _mainTrackMovementSpeed += speedUpDelta;
        }
    }
}