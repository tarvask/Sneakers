using System;

namespace Sneakers
{
    public class LoseUiController
    {
        private readonly LoseUiView _view;
        
        public LoseUiController(LoseUiView view)
        {
            _view = view;
        }

        public void Show(Action onContinueWithAdsAction, Action onReplayAction, Action onMainMenuAction)
        {
            _view.ContinueWithAdsButton.onClick.AddListener(() => onContinueWithAdsAction());
            _view.ReplayButton.onClick.AddListener(() => onReplayAction());
            _view.MainMenuButton.onClick.AddListener(() => onMainMenuAction());
            
            _view.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _view.gameObject.SetActive(false);
            
            _view.ContinueWithAdsButton.onClick.RemoveAllListeners();
            _view.ReplayButton.onClick.RemoveAllListeners();
            _view.MainMenuButton.onClick.RemoveAllListeners();
        }
    }
}