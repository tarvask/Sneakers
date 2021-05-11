using UnityEngine;

namespace Sneakers
{
    public abstract class AbstractSpecialTrack : AbstractTrack
    {
        [SerializeField] protected Transform[] trackPoints;

        private SneakerController _processingSneaker;

        protected bool IsBusy => _processingSneaker != null;
        
        protected override void OnWrongTrackDropped(SneakerController sneaker)
        {
            base.OnWrongTrackDropped(sneaker);
            
            _sortingController.OnSortError();
        }

        protected void StartProcessingSneaker(SneakerController sneaker)
        {
            _processingSneaker = sneaker;
        }

        protected void StopProcessingSneaker()
        {
            _processingSneaker = null;
        }

        public void CheckAndRemoveFromTrack(SneakerController sneaker)
        {
            if (_processingSneaker == sneaker)
                StopProcessingSneaker();
        }
    }
}