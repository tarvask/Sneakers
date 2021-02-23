using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class WashTrack : AbstractSpecialTrack
    {
        [SerializeField] private float washDelay = 5f;
        [SerializeField] private float washTrackMovementSpeed;
        
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
                float x1 = _movement.points[5].position.x;
                float y1 = _movement.points[5].position.y;
                float x2 = _movement.points[4].position.x;
                float y2 = _movement.points[4].position.y;
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
                while (_movement.points[5].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        _movement.points[5].position, washTrackMovementSpeed);
                    yield return new WaitForFixedUpdate();
                }
                
                sneaker.SwitchVisibility(false);
                yield return new WaitForSeconds(washDelay);
                
                sneaker.SwitchVisibility(true);
                mover++;
            }
            if (mover == 3)
            {
                sneaker.SetState(SneakerState.Normal);
                sneaker.currentPoint = 3;
                while (_movement.points[6].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        _movement.points[6].position, washTrackMovementSpeed);
                    yield return new WaitForFixedUpdate();
                }
                mover++;
            }
            if (mover == 4)
            {
                sneaker.currentPoint = 4;
                while (_movement.SneakersSpawnPoint.position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position,
                        _movement.SneakersSpawnPoint.position, washTrackMovementSpeed);
                    yield return new WaitForFixedUpdate();
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