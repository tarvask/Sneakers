using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sneakers
{
    public class LegendaryInventoryUiController
    {
        private readonly LegendaryInventoryUiView _view;

        public LegendaryInventoryUiController(LegendaryInventoryUiView view)
        {
            _view = view;
        }
        
        public void Show(Dictionary<int, int> currentLegends, Action onBackButtonAction)
        {
            Time.timeScale = 1;
            int legendsCount = 0;
            
            // load current legends to free slots
            foreach (SneakerConfig legendsPair in _view.LegendarySneakers)
            {
                if (currentLegends.ContainsKey(legendsPair.Id))
                {
                    _view.Sneakers[legendsCount].sprite = legendsPair.Prefab.Icon.sprite;
                    _view.Sneakers[legendsCount].transform.parent.gameObject.SetActive(true);
                    legendsCount++;

                    if (legendsCount == _view.Sneakers.Length)
                        break;
                }
            }

            // switch off the rest
            for (int sneakerIndex = legendsCount; sneakerIndex < _view.Sneakers.Length; sneakerIndex++)
            {
                _view.Sneakers[sneakerIndex].transform.parent.gameObject.SetActive(false);
            }
            
            _view.BackButton.onClick.AddListener(() => onBackButtonAction());
            _view.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Time.timeScale = 0;
            _view.BackButton.onClick.RemoveAllListeners();
            _view.gameObject.SetActive(false);
        }
    }
}