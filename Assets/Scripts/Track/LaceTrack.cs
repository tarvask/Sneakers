using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class LaceTrack : AbstractSpecialTrack
    {
        private float _laceTrackMovementSpeed;
        private float _laceProcessDelay;

        public void Init(Movement movement, float laceTrackMovementSpeed, float laceProcessDelay)
        {
            base.Init(movement);

            _laceTrackMovementSpeed = laceTrackMovementSpeed;
            _laceProcessDelay = laceProcessDelay;
        }

        protected override void OnDropSneaker(SneakerModel sneaker)
        {
            if (sneaker.State == SneakerState.Unlaced)
            {
                // stop waiting coroutine
                if (sneaker.DragDropItem.isHold)
                {
                    sneaker.StopCoroutine(sneaker.route);
                    sneaker.DragDropItem.isHold = false;
                    //sneaker.DragDropItem.isDropped = false;
                }
                
                sneaker.transform.position = _movement.points[7].position;
                _movement.SendToLaceTransporter(sneaker);
            }
            else
            {
                OnWrongTrackDropped(sneaker);
            }
        }
        
        public IEnumerator LaceRoute(SneakerModel sneaker, int mover)
        {
            //sneaker.GetComponent<DragDropItem>().isDropped = false;
            sneaker.SetTransporterType(TransporterType.Waiting);
            
            if (mover == 2)
            {
                sneaker.SwitchVisibility(false);
                yield return new WaitForSeconds(_laceProcessDelay);
                    
                sneaker.SetState(SneakerState.Normal);
                sneaker.currentPoint = 2;
                sneaker.GetComponent<CanvasGroup>().alpha = 1F;
                sneaker.GetComponent<CanvasGroup>().blocksRaycasts = true;
                sneaker.SwitchVisibility(true);
                
                while (_movement.points[8].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        _movement.points[8].position, _laceTrackMovementSpeed);
                    yield return new WaitForFixedUpdate();
                }
                mover++;
            }
            if (mover == 3)
            {
                sneaker.currentPoint = 3;
                while (_movement.SneakersSpawnPoint.position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        _movement.SneakersSpawnPoint.position, _laceTrackMovementSpeed);
                    yield return new WaitForFixedUpdate();
                }
                mover++;
            }
            if (mover == 4)
            {
                sneaker.currentPoint = 4;
                _movement.SendToMainTransporter(sneaker);
            }
        }
    }
}