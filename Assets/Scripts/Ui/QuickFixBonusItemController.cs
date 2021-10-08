using System;
using UniRx;

namespace Sneakers
{
    public class QuickFixBonusItemController
    {
        public struct Context
        {
            public BonusItemView View { get; }
            public BonusShopType BonusType { get; }
            public IReadOnlyReactiveProperty<int> QuickFixBonusCountReactiveProperty { get; }
            public IReadOnlyReactiveProperty<bool> BonusReadinessReactiveProperty { get; }
            public Action<BonusShopType> OnBonusClickedAction { get; }

            public Context(BonusItemView view, BonusShopType bonusType,
                IReadOnlyReactiveProperty<int> quickFixBonusCountReactiveProperty,
                IReadOnlyReactiveProperty<bool> bonusReadinessReactiveProperty,
                Action<BonusShopType> onBonusClickedAction)
            {
                View = view;
                BonusType = bonusType;
                QuickFixBonusCountReactiveProperty = quickFixBonusCountReactiveProperty;
                BonusReadinessReactiveProperty = bonusReadinessReactiveProperty;
                OnBonusClickedAction = onBonusClickedAction;
            }
        }

        private readonly Context _context;
        private bool _isBonusAvailableOnLevel;
        private bool _isBonusUnlimitedOnLevel;

        public QuickFixBonusItemController(Context context)
        {
            _context = context;

            _context.QuickFixBonusCountReactiveProperty.Subscribe((bonusCount) => Refresh());
            _context.BonusReadinessReactiveProperty.Subscribe((bonusReadiness) => Refresh());
            _context.View.UseBonusButton.onClick.AddListener(OnBonusClickedEventHandler);
        }

        public void Init(bool isBonusAvailableOnLevel, bool isBonusUnlimitedOnLevel)
        {
            _isBonusAvailableOnLevel = isBonusAvailableOnLevel;
            _isBonusUnlimitedOnLevel = isBonusUnlimitedOnLevel;
            Refresh();
        }

        private void Refresh()
        {
            _context.View.BonusCountText.text = $"{_context.QuickFixBonusCountReactiveProperty.Value}";
            _context.View.BonusCountText.gameObject.SetActive(_context.QuickFixBonusCountReactiveProperty.Value > 0 && !_isBonusUnlimitedOnLevel);
            bool isBlocked = !_isBonusAvailableOnLevel
                             || _context.QuickFixBonusCountReactiveProperty.Value == 0 && !_isBonusUnlimitedOnLevel
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