using System;
using UniRx;
using UnityEngine;

namespace Sneakers
{
    public class QuickFixBonusItemController
    {
        public struct Context
        {
            public BonusItemView View { get; }
            public BonusShopType BonusType { get; }
            public IReadOnlyReactiveProperty<int> QuickFixWashBonusCountReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> QuickFixLaceBonusCountReactiveProperty { get; }
            public Action<BonusShopType> OnBonusClickedAction { get; }

            public Context(BonusItemView view, BonusShopType bonusType,
                IReadOnlyReactiveProperty<int> quickFixWashBonusCountReactiveProperty,
                IReadOnlyReactiveProperty<int> quickFixLaceBonusCountReactiveProperty,
                Action<BonusShopType> onBonusClickedAction)
            {
                View = view;
                BonusType = bonusType;
                QuickFixWashBonusCountReactiveProperty = quickFixWashBonusCountReactiveProperty;
                QuickFixLaceBonusCountReactiveProperty = quickFixLaceBonusCountReactiveProperty;
                OnBonusClickedAction = onBonusClickedAction;
            }
        }

        private readonly Context _context;
        private bool _isBonusAvailableOnLevel;

        public QuickFixBonusItemController(Context context)
        {
            _context = context;

            _context.QuickFixWashBonusCountReactiveProperty.Subscribe((bonusCount) => Refresh());
            _context.QuickFixLaceBonusCountReactiveProperty.Subscribe((bonusCount) => Refresh());
            _context.View.UseBonusButton.onClick.AddListener(OnBonusClickedEventHandler);
        }

        public void Init(bool isBonusAvailableOnLevel)
        {
            _isBonusAvailableOnLevel = isBonusAvailableOnLevel;
            Refresh();
        }

        private void Refresh()
        {
            _context.View.BonusCountText.text = $"{Mathf.Min(_context.QuickFixWashBonusCountReactiveProperty.Value, _context.QuickFixLaceBonusCountReactiveProperty.Value)}";
            _context.View.BonusCountText.gameObject.SetActive(_context.QuickFixWashBonusCountReactiveProperty.Value > 0 || _context.QuickFixLaceBonusCountReactiveProperty.Value > 0);
            bool isBlocked = !_isBonusAvailableOnLevel
                             || _context.QuickFixWashBonusCountReactiveProperty.Value == 0 && _context.QuickFixLaceBonusCountReactiveProperty.Value == 0;
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