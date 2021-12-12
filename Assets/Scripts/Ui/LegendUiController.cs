using System;

namespace Sneakers
{
    public class LegendUiController
    {
        private readonly LegendUiView _view;

        public LegendUiController(LegendUiView view)
        {
            _view = view;
        }
        
        public void Show(SneakerConfig legendarySneakerConfig, Action onContinueAction)
        {
            _view.LegendIcon.sprite = legendarySneakerConfig.Prefab.Icon.sprite;
            //_view.LegendNameText.text = legendarySneakerConfig.Model;
            _view.ContinueButton.onClick.AddListener(() => onContinueAction());
            
            _view.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _view.gameObject.SetActive(false);
            
            _view.ContinueButton.onClick.RemoveAllListeners();
        }
    }
}