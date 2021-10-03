using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class WashTrack : AbstractSpecialTrack
    {
        [SerializeField] private Image quickFixActiveHighlight;
        
        private TrackLevelParams _trackLevelParams;
        private float _currentProcessDuration;
        private bool _isQuickFixActive;

        public void Init(SortingController sortingController, bool isAvailable, TrackLevelParams trackLevelParams)
        {
            base.Init(sortingController, isAvailable);

            _trackLevelParams = trackLevelParams;
            ResetQuickFix();
        }

        public void Upgrade(TrackLevelParams trackLevelParams)
        {
            _trackLevelParams = trackLevelParams;
            _currentProcessDuration = trackLevelParams.ProcessDuration;
            _isQuickFixActive = false;
        }

        public void SetQuickFix(float processDuration)
        {
            _currentProcessDuration = processDuration;
            _isQuickFixActive = true;

            quickFixActiveHighlight.gameObject.SetActive(true);
        }
        
        private void ResetQuickFix()
        {
            _currentProcessDuration = _trackLevelParams.ProcessDuration;
            _isQuickFixActive = false;
            
            quickFixActiveHighlight.gameObject.SetActive(false);
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (IsBusy)
            {
                sneaker.SetPosition(sneaker.DragDropItem.vector);
                
                if (!sneaker.DragDropItem.IsHold)
                    _sortingController.SendToMainTransporter(sneaker, sneaker.CurrentPoint);
            }
            else if (sneaker.State == SneakerState.Dirty)
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
                StartProcessingSneaker(sneaker);
            }
            else
            {
                OnWrongTrackDropped(sneaker);
            }
        }
        
        public IEnumerator WashRoute(SneakerController sneaker, int mover)
        {
            sneaker.SetTransporterType(TransporterType.Washing);
            sneaker.OnRouteStart();
            //sneaker.DragDropItem.isDropped = false;
            
            if (mover == 2)
            {
                sneaker.SetCurrentPoint(2);
                while (Vector3.SqrMagnitude(trackPoints[1].localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[1].localPosition, _trackLevelParams.TrackSpeed);
                    yield return null;
                }
                
                sneaker.SwitchVisibility(false);
                yield return new WaitForSeconds(_currentProcessDuration);
                
                sneaker.SwitchVisibility(true);
                mover++;
            }
            if (mover == 3)
            {
                sneaker.SetState(SneakerState.Normal);
                StopProcessingSneaker();
                
                if (_isQuickFixActive)
                    ResetQuickFix();
                
                sneaker.SetCurrentPoint(3);
                while (Vector3.SqrMagnitude(trackPoints[2].localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[2].localPosition, _trackLevelParams.TrackSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 4)
            {
                sneaker.SetCurrentPoint(4);
                while (Vector3.SqrMagnitude(_sortingController.SneakersSpawnPoint.localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(_sortingController.SneakersSpawnPoint.localPosition, _trackLevelParams.TrackSpeed);
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