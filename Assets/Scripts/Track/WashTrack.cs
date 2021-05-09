using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class WashTrack : AbstractSpecialTrack
    {
        private float _washTrackMovementSpeed;
        private float _washProcessDelay;

        public void Init(SortingController sortingController, bool isAvailable, TrackLevelParams trackLevelParams)
        {
            base.Init(sortingController, isAvailable);

            _washTrackMovementSpeed = trackLevelParams.TrackSpeed;
            _washProcessDelay = trackLevelParams.ProcessDuration;
        }

        public void Upgrade(TrackLevelParams trackLevelParams)
        {
            _washTrackMovementSpeed = trackLevelParams.TrackSpeed;
            _washProcessDelay = trackLevelParams.ProcessDuration;
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (sneaker.State == SneakerState.Dirty)
            {
                // stop waiting coroutine
                if (sneaker.DragDropItem.IsHold)
                {
                    if (sneaker.CurrentCoroutine != null)
                        sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
                    
                    sneaker.DragDropItem.IsHold = false;
                    //sneaker.DragDropItem.isDropped = false;
                }

                // move to some place and start washing
                float x1 = trackPoints[1].localPosition.x;
                float y1 = trackPoints[1].localPosition.y;
                float x2 = trackPoints[0].localPosition.x;
                float y2 = trackPoints[0].localPosition.y;
                float x = sneaker.LocalPosition.x;
                float y = ((x1 * y2 - x2 * y1) + x * (y1 - y2)) / (x1 - x2);
                sneaker.SetPosition(new Vector2(x, y));
                _sortingController.SendToWashTransporter(sneaker);
            }
            else
            {
                OnWrongTrackDropped(sneaker);
            }
        }
        
        public IEnumerator WashRoute(SneakerController sneaker, int mover)
        {
            sneaker.SetTransporterType(TransporterType.Washing);
            //sneaker.DragDropItem.isDropped = false;
            
            if (mover == 2)
            {
                sneaker.SetCurrentPoint(2);
                while (Vector3.SqrMagnitude(trackPoints[1].localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[1].localPosition, _washTrackMovementSpeed);
                    yield return null;
                }
                
                sneaker.SwitchVisibility(false);
                yield return new WaitForSeconds(_washProcessDelay);
                
                sneaker.SwitchVisibility(true);
                mover++;
            }
            if (mover == 3)
            {
                sneaker.SetState(SneakerState.Normal);
                sneaker.SetCurrentPoint(3);
                while (Vector3.SqrMagnitude(trackPoints[2].localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[2].localPosition, _washTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 4)
            {
                sneaker.SetCurrentPoint(4);
                while (Vector3.SqrMagnitude(_sortingController.SneakersSpawnPoint.localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(_sortingController.SneakersSpawnPoint.localPosition, _washTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 5)
            {
                sneaker.SetCurrentPoint(5);
                _sortingController.SendToMainTransporter(sneaker);
            }
        }
    }
}