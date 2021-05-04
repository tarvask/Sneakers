using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class SneakerView : MonoBehaviour
    {
        [SerializeField] private SneakerStateViewPair[] statesViews;
        [SerializeField] private DragDropItem dragDropItem;
        [SerializeField] private Image icon;

        public DragDropItem DragDropItem => dragDropItem;
        public Image Icon => icon;

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