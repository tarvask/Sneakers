using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class MainTrack : AbstractTrack
    {
        [SerializeField] protected Transform[] trackPoints;
        
        private float _mainTrackMovementSpeed;

        public void Init(SortingController sortingController, bool isAvailable, float mainTrackMovementSpeed)
        {
            base.Init(sortingController, isAvailable);

            _mainTrackMovementSpeed = mainTrackMovementSpeed;
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            OnWrongTrackDropped(sneaker);
        }
        
        public IEnumerator MainRoute(SneakerController sneaker, int mover)
        {
            //sneaker.GetComponent<DragDropItem>().isDropped = false;
            sneaker.SetTransporterType(TransporterType.Main);
            while (mover == 0 || mover == 1 || mover == 2)
            {
                sneaker.SetCurrentPoint(mover);
                while (Vector3.SqrMagnitude(trackPoints[sneaker.CurrentPoint + 1].position - sneaker.Position)
                       > GameConstants.SuperCloseDistanceSqr)
                {
                    sneaker.Move(trackPoints[sneaker.CurrentPoint + 1].position, _mainTrackMovementSpeed);
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
    }
}