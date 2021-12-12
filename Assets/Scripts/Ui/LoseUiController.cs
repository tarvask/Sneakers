using System;
using System.Collections;
using UnityEngine;

namespace Sneakers
{
    public class LoseUiController
    {
        private readonly LoseUiView _view;
        private const int CooldownDuration = 5;
        private int _countdownTimer;
        private Coroutine _cooldownCoroutine;
        
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

            // set up countdown timer
            _countdownTimer = CooldownDuration;
            
            // show timer instead of buttons
            _view.ReplayButton.gameObject.SetActive(false);
            _view.MainMenuButton.gameObject.SetActive(false);
            _view.CountdownTimerText.gameObject.SetActive(true);
            _view.NoThanksText.gameObject.SetActive(false);
            
            _cooldownCoroutine = _view.StartCoroutine(WaitAndShowButtons());
        }

        public void Hide()
        {
            _view.gameObject.SetActive(false);
            
            _view.ContinueWithAdsButton.onClick.RemoveAllListeners();
            _view.ReplayButton.onClick.RemoveAllListeners();
            _view.MainMenuButton.onClick.RemoveAllListeners();
        }

        private IEnumerator WaitAndShowButtons()
        {
            while (_countdownTimer > 0)
            {
                _view.CountdownTimerText.text = $"{_countdownTimer}";
                yield return new WaitForSecondsRealtime(1f);
                _countdownTimer--;
            }
            
            // hide timer, restore buttons
            _view.ReplayButton.gameObject.SetActive(true);
            _view.MainMenuButton.gameObject.SetActive(true);
            _view.CountdownTimerText.gameObject.SetActive(false);
            _view.NoThanksText.gameObject.SetActive(true);
        }
    }
}