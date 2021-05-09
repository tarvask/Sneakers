using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class LaceTrack : AbstractSpecialTrack
    {
        private float _laceTrackMovementSpeed;
        private float _laceProcessDelay;

        public void Init(SortingController sortingController, bool isAvailable, TrackLevelParams trackLevelParams)
        {
            base.Init(sortingController, isAvailable);

            _laceTrackMovementSpeed = trackLevelParams.TrackSpeed;
            _laceProcessDelay = trackLevelParams.ProcessDuration;
        }
        
        public void Upgrade(TrackLevelParams trackLevelParams)
        {
            _laceTrackMovementSpeed = trackLevelParams.TrackSpeed;
            _laceProcessDelay = trackLevelParams.ProcessDuration;
        }

        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (sneaker.State == SneakerState.Unlaced)
            {
                // stop waiting coroutine
                if (sneaker.DragDropItem.IsHold)
                {
                    if (sneaker.CurrentCoroutine != null)
                        sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
                    
                    sneaker.DragDropItem.IsHold = false;
                    //sneaker.DragDropItem.isDropped = false;
                }
                
                sneaker.SetPosition(trackPoints[0].localPosition);
                _sortingController.SendToLaceTransporter(sneaker);
            }
            else
            {
                OnWrongTrackDropped(sneaker);
            }
        }
        
        public IEnumerator LaceRoute(SneakerController sneaker, int mover)
        {
            //sneaker.GetComponent<DragDropItem>().isDropped = false;
            sneaker.SetTransporterType(TransporterType.Waiting);
            
            if (mover == 2)
            {
                sneaker.SwitchVisibility(false);
                yield return new WaitForSeconds(_laceProcessDelay);
                    
                sneaker.SetState(SneakerState.Normal);
                sneaker.SetCurrentPoint(2);
                sneaker.View.GetComponent<CanvasGroup>().alpha = 1F;
                sneaker.View.GetComponent<CanvasGroup>().blocksRaycasts = true;
                sneaker.SwitchVisibility(true);
                
                while (Vector3.SqrMagnitude(trackPoints[1].localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[1].localPosition, _laceTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 3)
            {
                sneaker.SetCurrentPoint(3);
                while (Vector3.SqrMagnitude(_sortingController.SneakersSpawnPoint.localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(_sortingController.SneakersSpawnPoint.localPosition, _laceTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 4)
            {
                sneaker.SetCurrentPoint(4);
                _sortingController.SendToMainTransporter(sneaker);
            }
        }
    }
}