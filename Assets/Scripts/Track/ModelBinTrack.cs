namespace Sneakers
{
    public class ModelBinTrack : AbstractSpecialTrack
    {
        private int _modelId;
        
        public void Init(SortingController sortingController, bool isAvailable, int modelId)
        {
            base.Init(sortingController, isAvailable);

            _modelId = modelId;
        }
        
        protected override void OnDropSneaker(SneakerController sneaker)
        {
            // sorting
            if (sneaker.Id == _modelId && sneaker.State == SneakerState.Normal)
            {
                sneaker.DragDropItem.IsHold = false;
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