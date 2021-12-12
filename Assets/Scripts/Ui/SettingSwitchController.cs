namespace Sneakers
{
    public class SettingSwitchController
    {
        private readonly SettingSwitchView _view;
        private bool _isOn;

        public bool IsOn => _isOn;

        public SettingSwitchController(SettingSwitchView view, bool state)
        {
            _view = view;
            
            SetState(state);
            _view.SwitchButton.onClick.AddListener(SwitchState);
        }

        private void SwitchState()
        {
            SetState(!_isOn);
        }

        private void SetState(bool newState)
        {
            _isOn = newState;
            _view.OnSwitchVariantGo.SetActive(_isOn);
            _view.OffSwitchVariantGo.SetActive(!_isOn);
        }
    }
}