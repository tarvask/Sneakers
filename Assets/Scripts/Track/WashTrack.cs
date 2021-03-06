using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class WashTrack : AbstractSpecialTrack
    {
        private float _washTrackMovementSpeed;
        private float _washProcessDelay;

        public void Init(Movement movement, float washTrackMovementSpeed, float washProcessDelay)
        {
            base.Init(movement);

            _washTrackMovementSpeed = washTrackMovementSpeed;
            _washProcessDelay = washProcessDelay;
        }
        
        protected override void OnDropSneaker(SneakerModel sneaker)
        {
            if (sneaker.State == SneakerState.Dirty)
            {
                // stop waiting coroutine
                if (sneaker.DragDropItem.isHold)
                {
                    sneaker.StopCoroutine(sneaker.route);
                    sneaker.DragDropItem.isHold = false;
                    //sneaker.DragDropItem.isDropped = false;
                }

                // move to some place and start washing
                float x1 = trackPoints[1].position.x;
                float y1 = trackPoints[1].position.y;
                float x2 = trackPoints[0].position.x;
                float y2 = trackPoints[0].position.y;
                float x = sneaker.transform.position.x;
                float y = ((x1 * y2 - x2 * y1) + x * (y1 - y2)) / (x1 - x2);
                sneaker.transform.position = new Vector2(x, y);
                _movement.SendToWashTransporter(sneaker);
            }
            else
            {
                OnWrongTrackDropped(sneaker);
            }
        }
        
        public IEnumerator WashRoute(SneakerModel sneaker, int mover)
        {
            sneaker.SetTransporterType(TransporterType.Washing);
            //sneaker.DragDropItem.isDropped = false;
            
            if (mover == 2)
            {
                sneaker.currentPoint = 2;
                while (trackPoints[1].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        trackPoints[1].position, _washTrackMovementSpeed);
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
                sneaker.currentPoint = 3;
                while (trackPoints[2].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        trackPoints[2].position, _washTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 4)
            {
                sneaker.currentPoint = 4;
                while (_movement.SneakersSpawnPoint.position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        _movement.SneakersSpawnPoint.position, _washTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 5)
            {
                sneaker.currentPoint = 5;
                _movement.SendToMainTransporter(sneaker);
            }
        }
    }
}