namespace Sneakers
{
    public class ModelBinTrack : AbstractSpecialTrack
    {
        private int _modelId;
        
        public void Init(Movement movement, bool isAvailable, int modelId)
        {
            base.Init(movement, isAvailable);

            _modelId = modelId;
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            // sorting
            if (sneaker.Id == _modelId && sneaker.State == SneakerState.Normal)
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