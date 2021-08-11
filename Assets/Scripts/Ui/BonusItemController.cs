using System;
using UniRx;

namespace Sneakers
{
    public class BonusItemController
    {
        public struct Context
        {
            public BonusItemView View { get; }
            public BonusType BonusType { get; }
            public BonusParameters BonusParameters { get; }
            public IReadOnlyReactiveProperty<int> CountReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> CoinsCountReactiveProperty { get; }
            public Action<BonusType> OnBonusClickedAction { get; }

            public Context(BonusItemView view, BonusType bonusType, BonusParameters bonusParameters,
                IReadOnlyReactiveProperty<int> countReactiveProperty,
                IReadOnlyReactiveProperty<int> coinsCountReactiveProperty,
                Action<BonusType> onBonusClickedAction)
            {
                View = view;
                BonusType = bonusType;
                BonusParameters = bonusParameters;
                CountReactiveProperty = countReactiveProperty;
                CoinsCountReactiveProperty = coinsCountReactiveProperty;
                OnBonusClickedAction = onBonusClickedAction;
            }
        }

        private readonly Context _context;
        private bool _isBonusAvailableOnLevel;

        public BonusItemController(Context context)
        {
            _context = context;

            _context.View.PriceText.text = $"{_context.BonusParameters.BonusPrice}";
            _context.View.UseBonusButton.onClick.AddListener(OnBonusClickedEventHandler);
        }

        public void Init(bool isBonusAvailableOnLevel)
        {
            _isBonusAvailableOnLevel = isBonusAvailableOnLevel;
            Refresh();
        }

        private void Refresh()
        {
            _context.View.BonusCountText.text = $"{_context.CountReactiveProperty.Value}";
            _context.View.BonusCountText.gameObject.SetActive(_context.CountReactiveProperty.Value > 0);
            _context.View.PriceText.gameObject.SetActive(_context.CountReactiveProperty.Value == 0);
            bool isBlocked = !_isBonusAvailableOnLevel
                             ||
                             _context.CountReactiveProperty.Value == 0
                             && _context.CoinsCountReactiveProperty.Value < _context.BonusParameters.BonusPrice;
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