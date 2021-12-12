using UnityEngine;

namespace Sneakers
{
    public abstract class AbstractSpecialTrack : AbstractTrack
    {
        [SerializeField] protected Transform[] trackPoints;

        private SneakerController _processingSneaker;

        protected bool IsBusy => _processingSneaker != null;
        
        public override void Init(SortingController sortingController, bool isAvailable)
        {
            base.Init(sortingController, isAvailable);
            
            // safety reasons
            StopProcessingSneaker();
        }
        
        protected override void OnWrongTrackDropped(SneakerController sneaker)
        {
            base.OnWrongTrackDropped(sneaker);
            
            _sortingController.OnSortError();
        }

        protected void StartProcessingSneaker(SneakerController sneaker)
        {
            _processingSneaker = sneaker;
            StartWorkAnimation();
        }

        protected void StopProcessingSneaker()
        {
            _processingSneaker = null;
            StopWorkAnimation();
        }

        public void CheckAndRemoveFromTrack(SneakerController sneaker)
        {
            if (_processingSneaker == sneaker)
                StopProcessingSneaker();
        }

        protected virtual void StartWorkAnimation()
        {
            
        }
        
        protected virtual void StopWorkAnimation()
        {
            
        }
    }
}