using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class LaceTrack : AbstractSpecialTrack
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
            _currentProcessDuration = _trackLevelParams.ProcessDuration;
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
            else if (sneaker.State == SneakerState.Unlaced)
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
                StartProcessingSneaker(sneaker);
            }
            else
            {
                OnWrongTrackDropped(sneaker);
            }
        }
        
        public IEnumerator LaceRoute(SneakerController sneaker, int mover)
        {
            //sneaker.GetComponent<DragDropItem>().isDropped = false;
            sneaker.SetTransporterType(TransporterType.Lacing);
            sneaker.OnRouteStart();
            
            if (mover == 2)
            {
                sneaker.SwitchVisibility(false);
                yield return new WaitForSeconds(_currentProcessDuration);
                    
                sneaker.SetState(SneakerState.Normal);
                StopProcessingSneaker();
                
                if (_isQuickFixActive)
                    ResetQuickFix();
                
                sneaker.SetCurrentPoint(2);
                sneaker.View.GetComponent<CanvasGroup>().alpha = 1F;
                sneaker.View.GetComponent<CanvasGroup>().blocksRaycasts = true;
                sneaker.SwitchVisibility(true);
                
                while (Vector3.SqrMagnitude(trackPoints[1].localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[1].localPosition, _trackLevelParams.TrackSpeed);
                    yield return null;
                }
                
                while (Vector3.SqrMagnitude(trackPoints[2].localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[2].localPosition, _trackLevelParams.TrackSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 3)
            {
                sneaker.SetCurrentPoint(3);
                while (Vector3.SqrMagnitude(_sortingController.SneakersSpawnPoint.localPosition - sneaker.LocalPosition) > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(_sortingController.SneakersSpawnPoint.localPosition, _trackLevelParams.TrackSpeed);
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