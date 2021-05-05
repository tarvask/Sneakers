using System;
using UniRx;

namespace Sneakers
{
    public class UpgradeShopUiController
    {
        private readonly UpgradeShopUiView _view;

        public UpgradeShopUiController(UpgradeShopUiView view)
        {
            _view = view;
        }
        
        public void Show(ReactiveProperty<int> coinsReactiveProperty, TrackLevelParams[] washTrackLevelParams, TrackLevelParams[] laceTrackLevelParams,
            ReactiveProperty<int> washTrackLevel, ReactiveProperty<int> laceTrackLevel,
            Action onUpgradeWashTrackAction,
            Action onUpgradeLaceTrackAction,
            Action onContinueAction)
        {
            UpdateInfo(coinsReactiveProperty, washTrackLevelParams, laceTrackLevelParams,
                washTrackLevel, laceTrackLevel);
            
            _view.WashTrackUpgradeButton.onClick.AddListener(() =>
            {
                onUpgradeWashTrackAction();
                UpdateInfo(coinsReactiveProperty, washTrackLevelParams, laceTrackLevelParams,
                    washTrackLevel, laceTrackLevel);
            });
            _view.LaceTrackUpgradeButton.onClick.AddListener(() =>
            {
                onUpgradeLaceTrackAction();
                UpdateInfo(coinsReactiveProperty, washTrackLevelParams, laceTrackLevelParams,
                    washTrackLevel, laceTrackLevel);
            });
            _view.ContinueButton.onClick.AddListener(() => onContinueAction());
            
            _view.gameObject.SetActive(true);
        }

        private void UpdateInfo(ReactiveProperty<int> coinsReactiveProperty,
            TrackLevelParams[] washTrackLevelParams, TrackLevelParams[] laceTrackLevelParams,
            ReactiveProperty<int> washTrackLevel, ReactiveProperty<int> laceTrackLevel)
        {
            bool hasUpgradesOnWashTrack = washTrackLevel.Value + 1 < washTrackLevelParams.Length;
            bool hasUpgradesOnLaceTrack = laceTrackLevel.Value + 1 < laceTrackLevelParams.Length;

            if (hasUpgradesOnWashTrack)
            {
                _view.WashTrackUpgradePriceText.text = washTrackLevelParams[washTrackLevel.Value + 1].Price.ToString();
                _view.WashTrackUpgradePriceText.gameObject.SetActive(true);
            }
            else
                _view.WashTrackUpgradePriceText.gameObject.SetActive(false);

            if (hasUpgradesOnLaceTrack)
            {
                _view.LaceTrackUpgradePriceText.text = laceTrackLevelParams[laceTrackLevel.Value + 1].Price.ToString();
                _view.LaceTrackUpgradePriceText.gameObject.SetActive(true);
            }
            else
                _view.LaceTrackUpgradePriceText.gameObject.SetActive(false);

            bool canUpgradeWashTrack = hasUpgradesOnWashTrack && coinsReactiveProperty.Value >= washTrackLevelParams[washTrackLevel.Value].Price;
            bool canUpgradeLaceTrack = hasUpgradesOnLaceTrack && coinsReactiveProperty.Value >= laceTrackLevelParams[laceTrackLevel.Value].Price;

            _view.WashTrackUpgradeButton.enabled = canUpgradeWashTrack;
            _view.LaceTrackUpgradeButton.enabled = canUpgradeLaceTrack;
        }

        public void Hide()
        {
            _view.gameObject.SetActive(false);
            
            _view.WashTrackUpgradeButton.onClick.RemoveAllListeners();
            _view.LaceTrackUpgradeButton.onClick.RemoveAllListeners();
            _view.ContinueButton.onClick.RemoveAllListeners();
        }
    }
}