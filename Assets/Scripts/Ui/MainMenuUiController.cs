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

        public void Show(int currentLevel, bool isEndlessModeEnabled,
            Action onRegularModePlayAction, Action onEndlessModePlayAction,
            Action onLegendaryInventoryAction, Action onSettingsButtonAction)
        {
            _view.CurrentLevelText.text = $"Current level: {currentLevel}";
            _view.EndlessModeButton.gameObject.SetActive(isEndlessModeEnabled);
            _view.RegularModeButton.onClick.AddListener(() => onRegularModePlayAction());
            _view.EndlessModeButton.onClick.AddListener(() => onEndlessModePlayAction());
            _view.LegendaryInventoryButton.onClick.AddListener(() => onLegendaryInventoryAction());
            _view.SettingsButton.onClick.AddListener(() => onSettingsButtonAction());
            
            _view.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _view.gameObject.SetActive(false);
            
            _view.RegularModeButton.onClick.RemoveAllListeners();
            _view.EndlessModeButton.onClick.RemoveAllListeners();
            _view.LegendaryInventoryButton.onClick.RemoveAllListeners();
            _view.SettingsButton.onClick.RemoveAllListeners();
        }
    }
}