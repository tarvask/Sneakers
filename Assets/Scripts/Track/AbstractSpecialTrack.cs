using UnityEngine;

namespace Sneakers
{
    public abstract class AbstractSpecialTrack : AbstractTrack
    {
        [SerializeField] protected Transform[] trackPoints;
        
        protected override void OnWrongTrackDropped(SneakerController sneaker)
        {
            base.OnWrongTrackDropped(sneaker);
            
            _movement.OnSortError();
        }
    }
}