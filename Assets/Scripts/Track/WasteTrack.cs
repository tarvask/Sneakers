namespace Sneakers
{
    public class WasteTrack : AbstractSpecialTrack
    {
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (sneaker.State == SneakerState.Wasted)
            {
                _sortingController.OnSortSucceeded(sneaker);
            }
            else
            {
                OnWrongTrackDropped(sneaker);
            }
        }
        
        protected override void OnWrongTrackDropped(SneakerController sneaker)
        {
            base.OnWrongTrackDropped(sneaker);
            
            if (sneaker.IsLegendary)
                _sortingController.OnSortLegendaryError(sneaker);
        }
    }
}