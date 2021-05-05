using System;

namespace Sneakers
{
    public class TutorialUiController
    {
        private readonly TutorialUiView _view;

        public TutorialUiController(TutorialUiView view)
        {
            _view = view;
        }

        public void Show(string tutorialText, Action onContinueAction)
        {
            _view.TutorialText.text = tutorialText;
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