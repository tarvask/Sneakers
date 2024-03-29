using System;
using UnityEngine;

namespace Sneakers
{
    public class TutorialUiController
    {
        private readonly TutorialUiView _view;

        public TutorialUiController(TutorialUiView view)
        {
            _view = view;
        }

        public void Show(string tutorialHeader, string tutorialText, Action onContinueAction)
        {
            _view.TutorialHeader.text = tutorialHeader;
            _view.TutorialText.text = tutorialText;

            if (string.IsNullOrEmpty(tutorialHeader))
                _view.TutorialText.rectTransform.anchoredPosition = _view.TutorialHeader.rectTransform.anchoredPosition;
            else
                _view.TutorialText.rectTransform.anchoredPosition = _view.TutorialHeader.rectTransform.anchoredPosition
                                                                    - new Vector2(0, _view.TutorialHeader.rectTransform.sizeDelta.y);
            
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