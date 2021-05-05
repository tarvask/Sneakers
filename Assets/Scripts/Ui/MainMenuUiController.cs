using System;

namespace Sneakers
{
    public class MainMenuUiController
    {
        private readonly MainMenuUiView _view;
        
        public MainMenuUiController(MainMenuUiView view)
        {
            _view = view;
        }

        public void Show(int currentLevel, Action onPlayAction)
        {
            _view.CurrentLevelText.text = $"Current level: {currentLevel}";
            _view.PlayButton.onClick.AddListener(() => onPlayAction());
            _view.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _view.gameObject.SetActive(false);
            
            _view.PlayButton.onClick.RemoveAllListeners();
        }
    }
}