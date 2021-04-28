using System;
using UnityEngine;

namespace Sneakers
{
    public class SneakerView : MonoBehaviour
    {
        [SerializeField] private SneakerStateViewPair[] statesViews;
        [SerializeField] private DragDropItem dragDropItem;

        public DragDropItem DragDropItem => dragDropItem;
        
        public event Action<Action<SneakerController>> OnSneakerDropped;

        public void OnDropSneakerEventHandler(Action<SneakerController> sneakerController)
        {
            OnSneakerDropped?.Invoke(sneakerController);
        }
        
        public void SetState(SneakerState state)
        {
            foreach (SneakerStateViewPair sneakerPair in statesViews)
            {
                sneakerPair.View.SetActive(sneakerPair.State == state);
            }
        }
        
        public void SwitchVisibility(SneakerState state, bool visible)
        {
            if (visible)
            {
                SetState(state);
            }
            else
            {
                foreach (SneakerStateViewPair sneakerPair in statesViews)
                {
                    sneakerPair.View.SetActive(false);
                }
            }
        }
    }
}