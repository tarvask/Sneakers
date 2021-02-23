using UnityEngine;

namespace Sneakers
{
    public class ModelBinTrack : AbstractSpecialTrack
    {
        [SerializeField] private int modelId;
        
        protected override void OnDropSneaker(SneakerModel sneaker)
        {
            // sorting
            if (sneaker.Id == modelId && sneaker.State == SneakerState.Normal)
            {
                _movement.OnSortSucceeded(sneaker);
            }
            else
            {
                OnWrongTrackDropped(sneaker);
            }
        }
    }
}