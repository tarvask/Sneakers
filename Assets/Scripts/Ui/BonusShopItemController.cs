using System;
using UniRx;

namespace Sneakers
{
    public class BonusShopItemController
    {
        public struct Context
        {
            public BonusShopItemView View { get; }
            public BonusShopType BonusType { get; }
            public BonusParameters BonusParameters { get; }
            public IReadOnlyReactiveProperty<int> BonusCountReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> CoinsCountReactiveProperty { get; }

            public Context(BonusShopItemView view, BonusShopType bonusType, BonusParameters bonusParameters,
                IReadOnlyReactiveProperty<int> bonusCountReactiveProperty,
                IReadOnlyReactiveProperty<int> coinsCountReactiveProperty)
            {
                View = view;
                BonusType = bonusType;
                BonusParameters = bonusParameters;
                BonusCountReactiveProperty = bonusCountReactiveProperty;
                CoinsCountReactiveProperty = coinsCountReactiveProperty;
            }
        }

        private readonly Context _context;
        private bool _isBonusAvailableOnLevel;
        private Action<BonusShopType> _onBonusClickedAction;
        
        public BonusShopItemController(Context context)
        {
            _context = context;

            _context.View.PriceText.text = $"{_context.BonusParameters.BonusPrice}";
            _context.View.BuyBonusButton.onClick.AddListener(OnBonusClickedEventHandler);
            _context.CoinsCountReactiveProperty.Subscribe((coinsCount) => Refresh());
        }

        public void Init(bool isBonusAvailableOnLevel, Action<BonusShopType> onBonusClickedAction)
        {
            _isBonusAvailableOnLevel = isBonusAvailableOnLevel;
            _onBonusClickedAction = onBonusClickedAction;
            Refresh();
        }

        private void Refresh()
        {
            _context.View.BonusCountText.text = $"{_context.BonusCountReactiveProperty.Value}";
            bool isBlocked = !_isBonusAvailableOnLevel
                             ||
                             /*_context.BonusCountReactiveProperty.Value == 0
                             &&*/ _context.CoinsCountReactiveProperty.Value < _context.BonusParameters.BonusPrice;
            _context.View.BlockerGo.SetActive(isBlocked);
            _context.View.BuyBonusButton.enabled = !isBlocked;
        }

        private void OnBonusClickedEventHandler()
        {
            _onBonusClickedAction(_context.BonusType);
            Refresh();
        }
    }
}