namespace Sneakers
{
    public class MainTrack : AbstractTrack
    {
        protected override void OnDropSneaker(SneakerModel sneaker)
        {
            OnWrongTrackDropped(sneaker);
        }
    }
}