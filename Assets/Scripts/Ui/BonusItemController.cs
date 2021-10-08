using System;
using UniRx;

namespace Sneakers
{
    public class BonusItemController
    {
        public struct Context
        {
            public BonusItemView View { get; }
            public BonusShopType BonusType { get; }
            public IReadOnlyReactiveProperty<int> BonusCountReactiveProperty { get; }
            public IReadOnlyReactiveProperty<bool> BonusReadinessReactiveProperty { get; }
            public Action<BonusShopType> OnBonusClickedAction { get; }

            public Context(BonusItemView view, BonusShopType bonusType,
                IReadOnlyReactiveProperty<int> bonusCountReactiveProperty,
                IReadOnlyReactiveProperty<bool> bonusReadinessReactiveProperty,
                Action<BonusShopType> onBonusClickedAction)
            {
                View = view;
                BonusType = bonusType;
                BonusCountReactiveProperty = bonusCountReactiveProperty;
                BonusReadinessReactiveProperty = bonusReadinessReactiveProperty;
                OnBonusClickedAction = onBonusClickedAction;
            }
        }

        private readonly Context _context;
        private bool _isBonusAvailableOnLevel;
        private bool _isUnlimitedOnLevel;

        public BonusItemController(Context context)
        {
            _context = context;

            _context.BonusCountReactiveProperty.Subscribe((bonusCount) => Refresh());
            _context.BonusReadinessReactiveProperty.Subscribe((bonusReadiness) => Refresh());
            _context.View.UseBonusButton.onClick.AddListener(OnBonusClickedEventHandler);
        }

        public void Init(bool isBonusAvailableOnLevel, bool isUnlimitedOnLevel)
        {
            _isBonusAvailableOnLevel = isBonusAvailableOnLevel;
            _isUnlimitedOnLevel = isUnlimitedOnLevel;
            Refresh();
        }

        private void Refresh()
        {
            _context.View.BonusCountText.text = $"{_context.BonusCountReactiveProperty.Value}";
            _context.View.BonusCountText.gameObject.SetActive(_context.BonusCountReactiveProperty.Value > 0 && !_isUnlimitedOnLevel);
            bool isBlocked = !_isBonusAvailableOnLevel
                             || _context.BonusCountReactiveProperty.Value == 0 && !_isUnlimitedOnLevel
                             || !_context.BonusReadinessReactiveProperty.Value;
            _context.View.BlockerGo.SetActive(isBlocked);
            _context.View.UseBonusButton.enabled = !isBlocked;
        }

        private void OnBonusClickedEventHandler()
        {
            _context.OnBonusClickedAction(_context.BonusType);
            Refresh();
        }
    }
}