namespace Sneakers
{
    public class ModelBinTrack : AbstractSpecialTrack
    {
        private int _modelId;
        
        public void Init(Movement movement, int modelId)
        {
            base.Init(movement);

            _modelId = modelId;
        }
        
        protected override void OnDropSneaker(SneakerModel sneaker)
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