using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class MainTrack : AbstractTrack
    {
        [SerializeField] protected Transform[] trackPoints;
        
        private float _mainTrackMovementSpeed;

        public void Init(Movement movement, bool isAvailable, float mainTrackMovementSpeed)
        {
            base.Init(movement, isAvailable);

            _mainTrackMovementSpeed = mainTrackMovementSpeed;
        }
        
        protected override void OnDropSneaker(SneakerModel sneaker)
        {
            OnWrongTrackDropped(sneaker);
        }
        
        public IEnumerator MainRoute(SneakerModel sneaker, int mover)
        {
            //sneaker.GetComponent<DragDropItem>().isDropped = false;
            sneaker.SetTransporterType(TransporterType.Main);
            while (mover == 0 || mover == 1 || mover == 2)
            {
                sneaker.currentPoint = mover;
                while (trackPoints[sneaker.currentPoint + 1].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position, trackPoints[sneaker.currentPoint + 1].position, _mainTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 3)
            {
                sneaker.currentPoint = 3;
                _movement.OnSortFailed(sneaker);
            }
        }
    }
}