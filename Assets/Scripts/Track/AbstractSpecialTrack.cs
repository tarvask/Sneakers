using UnityEngine;

namespace Sneakers
{
    public abstract class AbstractSpecialTrack : AbstractTrack
    {
        [SerializeField] protected Transform[] trackPoints;
        
        protected override void OnWrongTrackDropped(SneakerModel sneaker)
        {
            base.OnWrongTrackDropped(sneaker);
            
            _movement.OnSortError();
        }
    }
}