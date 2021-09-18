using System;

namespace Sneakers
{
    public class QuickFixBonusChoosingUiController
    {
        private readonly QuickFixBonusChoosingUiView _view;

        public QuickFixBonusChoosingUiController(QuickFixBonusChoosingUiView view)
        {
            _view = view;
        }

        public void Show(bool isWashTrackAvailable, Action onWashTrackChosenAction,
            bool isLaceTrackAvailable, Action onLaceTrackChosenAction)
        {
            // subscribe
            _view.ChooseWashTrackButton.onClick.AddListener(() => onWashTrackChosenAction());
            _view.ChooseLaceTrackButton.onClick.AddListener(() => onLaceTrackChosenAction());
            
            // wash track stuff
            _view.ChooseWashTrackButton.gameObject.SetActive(isWashTrackAvailable);
            _view.ChooseWashTrackButtonBlocker.gameObject.SetActive(!isWashTrackAvailable);
            
            // lace track stuff
            _view.ChooseLaceTrackButton.gameObject.SetActive(isLaceTrackAvailable);
            _view.ChooseLaceTrackButtonBlocker.gameObject.SetActive(!isLaceTrackAvailable);
            
            _view.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _view.gameObject.SetActive(false);
            
            // unsubscribe
            _view.ChooseWashTrackButton.onClick.RemoveAllListeners();
            _view.ChooseLaceTrackButton.onClick.RemoveAllListeners();
        }
    }
}