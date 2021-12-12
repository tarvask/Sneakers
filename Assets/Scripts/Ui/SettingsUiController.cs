using System;

namespace Sneakers
{
    public class SettingsUiController
    {
        private readonly SettingsUiView _view;
        private SettingSwitchController _musicSwitch;
        private SettingSwitchController _vibrationSwitch;

        public SettingsUiController(SettingsUiView view)
        {
            _view = view;
            _musicSwitch = new SettingSwitchController(_view.MusicSwitch, false);
            _vibrationSwitch = new SettingSwitchController(_view.VibrationSwitch, false);
        }

        public void Show(Action onBackButtonAction)
        {
            _view.BackButton.onClick.AddListener(() => onBackButtonAction());
            _view.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _view.BackButton.onClick.RemoveAllListeners();
            _view.gameObject.SetActive(false);
        }
    }
}