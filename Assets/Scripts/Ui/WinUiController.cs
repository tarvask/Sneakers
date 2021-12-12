using System;

namespace Sneakers
{
    public class WinUiController
    {
        private readonly WinUiView _view;

        public WinUiController(WinUiView view)
        {
            _view = view;
        }

        public void Show(int score, Action onContinueAction)
        {
            _view.LevelScoreText.text = $"{score}";
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