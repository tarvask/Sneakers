using System;
using UniRx;

namespace Sneakers
{
    public class UpgradeShopUiController
    {
        public struct Context
        {
            public UpgradeShopUiView View { get; }
            public TrackLevelParams[] WashTrackLevelParams { get; }
            public TrackLevelParams[] LaceTrackLevelParams { get; }
            public BonusesParameters BonusesParameters { get; }
            public IReadOnlyReactiveProperty<int> CoinsReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> WashTrackLevelReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> LaceTrackLevelReactiveProperty { get; }
            
            public IReadOnlyReactiveProperty<int> TrackFreezeBonusCountReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> QuickFixBonusCountReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> AutoUtilizationBonusCountReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> UndoBonusCountReactiveProperty { get; }
            
            public Action<BonusShopType> BuyBonusAction { get; }

            public Context(UpgradeShopUiView view,
                TrackLevelParams[] washTrackLevelParams,
                TrackLevelParams[] laceTrackLevelParams,
                BonusesParameters bonusesParameters,
                IReadOnlyReactiveProperty<int> coinsReactiveProperty,
                IReadOnlyReactiveProperty<int> washTrackLevelReactiveProperty,
                IReadOnlyReactiveProperty<int> laceTrackLevelReactiveProperty,
                IReadOnlyReactiveProperty<int> trackFreezeBonusCountReactiveProperty,
                IReadOnlyReactiveProperty<int> quickFixBonusCountReactiveProperty,
                IReadOnlyReactiveProperty<int> autoUtilizationBonusCountReactiveProperty,
                IReadOnlyReactiveProperty<int> undoBonusCountReactiveProperty,
                
                Action<BonusShopType> buyBonusAction)
            {
                View = view;
                BonusesParameters = bonusesParameters;
                WashTrackLevelParams = washTrackLevelParams;
                LaceTrackLevelParams = laceTrackLevelParams;
                CoinsReactiveProperty = coinsReactiveProperty;
                WashTrackLevelReactiveProperty = washTrackLevelReactiveProperty;
                LaceTrackLevelReactiveProperty = laceTrackLevelReactiveProperty;
                
                TrackFreezeBonusCountReactiveProperty = trackFreezeBonusCountReactiveProperty;
                QuickFixBonusCountReactiveProperty = quickFixBonusCountReactiveProperty;
                AutoUtilizationBonusCountReactiveProperty = autoUtilizationBonusCountReactiveProperty;
                UndoBonusCountReactiveProperty = undoBonusCountReactiveProperty;

                BuyBonusAction = buyBonusAction;
            }
        }

        private readonly Context _context;
        
        private readonly BonusShopItemController _freezeTrackBonus;
        private readonly BonusShopItemController _quickFixBonus;
        private readonly BonusShopItemController _autoUtilizationBonus;
        private readonly BonusShopItemController _undoBonus;
        private IDisposable _coinsChangingSubscription;

        public UpgradeShopUiController(Context context)
        {
            _context = context;
            
            BonusShopItemController.Context freezeTrackBonusContext = new BonusShopItemController.Context(
                _context.View.FreezeTrackBonus, BonusShopType.TrackFreeze, _context.BonusesParameters.FreezeTrackBonusParameters,
                _context.TrackFreezeBonusCountReactiveProperty,
                _context.CoinsReactiveProperty);
            _freezeTrackBonus = new BonusShopItemController(freezeTrackBonusContext);
            
            BonusShopItemController.Context quickFixBonusContext = new BonusShopItemController.Context(
                _context.View.QuickFixBonus, BonusShopType.QuickFix, _context.BonusesParameters.QuickFixBonusParameters,
                _context.QuickFixBonusCountReactiveProperty,
                _context.CoinsReactiveProperty);
            _quickFixBonus = new BonusShopItemController(quickFixBonusContext);
            
            BonusShopItemController.Context autoUtilizationBonusContext = new BonusShopItemController.Context(
                _context.View.AutoUtilizationBonus, BonusShopType.AutoUtilization, _context.BonusesParameters.AutoUtilizationBonusParameters,
                _context.AutoUtilizationBonusCountReactiveProperty,
                _context.CoinsReactiveProperty);
            _autoUtilizationBonus = new BonusShopItemController(autoUtilizationBonusContext);
            
            BonusShopItemController.Context undoBonusContext = new BonusShopItemController.Context(
                _context.View.UndoBonus, BonusShopType.Undo, _context.BonusesParameters.UndoBonusParameters,
                _context.UndoBonusCountReactiveProperty,
                _context.CoinsReactiveProperty);
            _undoBonus = new BonusShopItemController(undoBonusContext);
        }
        
        public void Show(
            BonusLevelLimitations freezeTrackBonusLimitations,
            BonusLevelLimitations quickFixWashBonusLimitations,
            BonusLevelLimitations autoUtilizationBonusLimitations,
            BonusLevelLimitations undoBonusLimitations,
            Action onUpgradeWashTrackAction,
            Action onUpgradeLaceTrackAction,
            Action onContinueAction)
        {
            UpdateInfo();
            
            _freezeTrackBonus.Init(freezeTrackBonusLimitations.IsBonusAvailable,
                (bonusShopType) =>
                {
                    _context.BuyBonusAction(bonusShopType);
                });
            _quickFixBonus.Init(quickFixWashBonusLimitations.IsBonusAvailable,
                (bonusShopType) =>
                {
                    _context.BuyBonusAction(bonusShopType);
                });
            _autoUtilizationBonus.Init(autoUtilizationBonusLimitations.IsBonusAvailable,
                (bonusShopType) =>
                {
                    _context.BuyBonusAction(bonusShopType);
                });
            _undoBonus.Init(undoBonusLimitations.IsBonusAvailable,
                (bonusShopType) =>
                {
                    _context.BuyBonusAction(bonusShopType);
                });
            
            _context.View.WashTrackUpgradeButton.onClick.AddListener(() =>
            {
                onUpgradeWashTrackAction();
            });
            _context.View.LaceTrackUpgradeButton.onClick.AddListener(() =>
            {
                onUpgradeLaceTrackAction();
            });
            _context.View.ContinueButton.onClick.AddListener(() => onContinueAction());
            _coinsChangingSubscription = _context.CoinsReactiveProperty.Subscribe(UpdateInfo);
            
            _context.View.gameObject.SetActive(true);
        }

        private void UpdateInfo(int coinsCount = 0)
        {
            _context.View.CoinsCountText.text = $"{_context.CoinsReactiveProperty.Value}";
            
            UpdateWashTrackUpgradeInfo(_context.WashTrackLevelParams, _context.WashTrackLevelReactiveProperty);
            UpdateLaceTrackUpgradeInfo(_context.LaceTrackLevelParams, _context.LaceTrackLevelReactiveProperty);
        }

        private void UpdateWashTrackUpgradeInfo(TrackLevelParams[] washTrackLevelParams, IReadOnlyReactiveProperty<int> washTrackLevel)
        {
            bool hasUpgradesOnWashTrack = washTrackLevel.Value + 1 < washTrackLevelParams.Length;

            if (hasUpgradesOnWashTrack)
            {
                _context.View.WashTrackUpgradePriceText.text = washTrackLevelParams[washTrackLevel.Value + 1].Price.ToString();
                _context.View.WashTrackUpgradePriceText.gameObject.SetActive(true);
            }
            else
                _context.View.WashTrackUpgradePriceText.gameObject.SetActive(false);

            bool canUpgradeWashTrack = hasUpgradesOnWashTrack && _context.CoinsReactiveProperty.Value >= washTrackLevelParams[washTrackLevel.Value + 1].Price;

            _context.View.WashTrackUpgradeButton.enabled = canUpgradeWashTrack;
        }

        private void UpdateLaceTrackUpgradeInfo(TrackLevelParams[] laceTrackLevelParams, IReadOnlyReactiveProperty<int> laceTrackLevel)
        {
            bool hasUpgradesOnLaceTrack = laceTrackLevel.Value + 1 < laceTrackLevelParams.Length;

            if (hasUpgradesOnLaceTrack)
            {
                _context.View.LaceTrackUpgradePriceText.text = laceTrackLevelParams[laceTrackLevel.Value + 1].Price.ToString();
                _context.View.LaceTrackUpgradePriceText.gameObject.SetActive(true);
            }
            else
                _context.View.LaceTrackUpgradePriceText.gameObject.SetActive(false);
            
            bool canUpgradeLaceTrack = hasUpgradesOnLaceTrack && _context.CoinsReactiveProperty.Value >= laceTrackLevelParams[laceTrackLevel.Value + 1].Price;
            
            _context.View.LaceTrackUpgradeButton.enabled = canUpgradeLaceTrack;
        }

        public void Hide()
        {
            _context.View.gameObject.SetActive(false);
            
            _context.View.WashTrackUpgradeButton.onClick.RemoveAllListeners();
            _context.View.LaceTrackUpgradeButton.onClick.RemoveAllListeners();
            _context.View.ContinueButton.onClick.RemoveAllListeners();
            _coinsChangingSubscription.Dispose();
        }
    }
}