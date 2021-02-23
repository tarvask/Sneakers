namespace Sneakers
{
    public abstract class AbstractSpecialTrack : AbstractTrack
    {
        protected override void OnWrongTrackDropped(SneakerModel sneaker)
        {
            base.OnWrongTrackDropped(sneaker);
            
            _movement.OnSortError();
        }
    }
}