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
    }
}