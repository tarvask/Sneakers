using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class ModelBinTrack : AbstractSpecialTrack
    {
        [SerializeField] private Image modelBoxSprite;
        
        private int _modelId;

        public int ModelId => _modelId;

        public void Init(SortingController sortingController, bool isAvailable, SneakerConfig modelConfig)
        {
            base.Init(sortingController, isAvailable);

            ChangeModel(modelConfig);
        }

        public void ChangeModel(SneakerConfig modelConfig)
        {
            _modelId = modelConfig.Id;
            modelBoxSprite.sprite = modelConfig.BoxSprite;
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