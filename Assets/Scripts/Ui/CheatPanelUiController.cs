namespace Sneakers
{
    public class CheatPanelUiController
    {
        private readonly CheatPanelUiView _view;

        public CheatPanelUiController(CheatPanelUiView view, bool isEnabled)
        {
            _view = view;
            
            if (isEnabled)
                Show();
            else
                Hide();
        }
        
        private void Show()
        {
            _view.ExpandButton.onClick.AddListener(SwitchExpand);
            
            // cheat buttons
            _view.WinLevelButton.onClick.AddListener(GameController.WinLevelInEditor);
            _view.LoseLevelButton.onClick.AddListener(GameController.LoseLevelInEditor);
            _view.AddCoinsButton.onClick.AddListener(GameController.AddCoinsInEditor);
            _view.RemoveCoinsButton.onClick.AddListener(GameController.RemoveCoinsInEditor);
            _view.DropProgressButton.onClick.AddListener(GameController.DropProgressInEditor);
            
            _view.gameObject.SetActive(true);
        }

        private void Hide()
        {
            _view.gameObject.SetActive(false);
            
            _view.ExpandButton.onClick.RemoveAllListeners();
            
            // cheat buttons
            _view.WinLevelButton.onClick.RemoveAllListeners();
            _view.LoseLevelButton.onClick.RemoveAllListeners();
            _view.AddCoinsButton.onClick.RemoveAllListeners();
            _view.RemoveCoinsButton.onClick.RemoveAllListeners();
            _view.DropProgressButton.onClick.RemoveAllListeners();
        }

        private void SwitchExpand()
        {
            if (_view.PanelGo.activeSelf)
                _view.PanelGo.SetActive(false);
            else
                _view.PanelGo.SetActive(true);
        }
    }
}