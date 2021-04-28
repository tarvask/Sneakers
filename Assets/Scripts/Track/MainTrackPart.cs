namespace Sneakers
{
    public class MainTrackPart : AbstractTrack 
    {
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            OnWrongTrackDropped(sneaker);
        }
    }
}