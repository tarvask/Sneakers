namespace Sneakers
{
    public class WasteTrack : AbstractSpecialTrack
    {
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            if (sneaker.State == SneakerState.Wasted)
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