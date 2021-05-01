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

        public void Show(int score, Action onContinueAction, Action onMainMenuAction)
        {
            _view.LevelScoreText.text = $"And got {score} coins";
            _view.ContinueButton.onClick.AddListener(() => onContinueAction());
            _view.MainMenuButton.onClick.AddListener(() => onMainMenuAction());
            _view.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _view.gameObject.SetActive(false);
        }
    }
}