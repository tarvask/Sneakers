namespace Sneakers
{
    public class MainTrackPart : AbstractTrack 
    {
        protected override void OnDropSneaker(SneakerModel sneaker)
        {
            OnWrongTrackDropped(sneaker);
        }
    }
}