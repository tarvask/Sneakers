using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Sneakers
{
    public class BonusesController
    {
        public struct Context
        {
            public GameModel GameModel { get; }
            public BonusesParameters BonusesParameters { get; }
            public Action<bool> SwitchFrozenStateAction { get; }
            public Action WashAllSneakersAction { get; }
            public Action LaceAllSneakersAction { get; }

            public Context(GameModel gameModel, BonusesParameters bonusesParameters,
                Action<bool> switchFrozenStateAction,
                Action washAllSneakersAction,
                Action laceAllSneakersAction)
            {
                GameModel = gameModel;
                BonusesParameters = bonusesParameters;
                SwitchFrozenStateAction = switchFrozenStateAction;
                WashAllSneakersAction = washAllSneakersAction;
                LaceAllSneakersAction = laceAllSneakersAction;
            }
        }

        private readonly Context _context;

        private bool _isFreezeTrackBonusActive;
        private bool _stopFreezeTrackBonus;
        private bool _isFreezeTrackBonusOnCooldown;

        private bool _isQuickFixWashBonusOnCooldown;
        private bool _isQuickFixLaceBonusOnCooldown;

        private readonly ReactiveProperty<bool> _isFreezeTrackBonusReady;
        private readonly ReactiveProperty<bool> _isQuickFixWashBonusReady;
        private readonly ReactiveProperty<bool> _isQuickFixLaceBonusReady;
        private readonly ReactiveProperty<bool> _isAutoUtilizationBonusReady;
        private readonly ReactiveProperty<bool> _isUndoBonusReady;

        public BonusesController(Context context)
        {
            _context = context;
        }

        public void Init(BonusLevelLimitations freezeTrackBonusLimitations)
        {
            // track freeze
            if (freezeTrackBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < freezeTrackBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusType.TrackFreeze);

            if (freezeTrackBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusType.TrackFreeze) > freezeTrackBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusType.TrackFreeze);
        }

        public void OuterUpdate(float frameLength)
        {
            if (_isFreezeTrackBonusActive && _stopFreezeTrackBonus)
            {
                _isFreezeTrackBonusActive = false;
                _context.SwitchFrozenStateAction(false);
            }

            if (!_isFreezeTrackBonusReady.Value && !_isFreezeTrackBonusOnCooldown)
                _isFreezeTrackBonusReady.Value = true;

            if (!_isQuickFixWashBonusReady.Value && !_isQuickFixWashBonusOnCooldown)
                _isQuickFixWashBonusReady.Value = true;
            
            if (!_isQuickFixLaceBonusReady.Value && !_isQuickFixLaceBonusOnCooldown)
                _isQuickFixLaceBonusReady.Value = true;
        }

        private int GetBonusCount(BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.TrackFreeze:
                    return _context.GameModel.TrackFreezeBonusCountReactiveProperty.Value;
                case BonusType.QuickFixWash:
                    return _context.GameModel.QuickFixWashBonusCountReactiveProperty.Value;
                case BonusType.QuickFixLace:
                    return _context.GameModel.QuickFixLaceBonusCountReactiveProperty.Value;
                case BonusType.AutoUtilization:
                    return _context.GameModel.AutoUtilizationBonusCountReactiveProperty.Value;
                case BonusType.Undo:
                    return _context.GameModel.UndoBonusCountReactiveProperty.Value;
            }

            throw new ArgumentException("Unknown bonus type");
        }
        
        private ReactiveProperty<bool> GetBonusReadyReactiveProperty(BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.TrackFreeze:
                    return _isFreezeTrackBonusReady;
                case BonusType.QuickFixWash:
                    return _isQuickFixWashBonusReady;
                case BonusType.QuickFixLace:
                    return _isQuickFixLaceBonusReady;
                case BonusType.AutoUtilization:
                    return _isAutoUtilizationBonusReady;
                case BonusType.Undo:
                    return _isUndoBonusReady;
            }

            throw new ArgumentException("Unknown bonus type");
        }

        public void ApplyBonus(BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.TrackFreeze:
                    FreezeTrack();
                    break;
                case BonusType.QuickFixWash:
                    WashAllSneakers();
                    break;
                case BonusType.QuickFixLace:
                    LaceAllSneakers();
                    break;
            }

            _context.GameModel.SpendBonus(bonusType);
        }

        private void FreezeTrack()
        {
            _context.SwitchFrozenStateAction(true);
            
            _isFreezeTrackBonusActive = true;

            // effect duration
            int effectTimerInMilliseconds =
                Mathf.RoundToInt(_context.BonusesParameters.FreezeTrackBonusParameters.BonusDuration * 1000);
            Task.Run(async () =>
            {
                await Task.Delay(effectTimerInMilliseconds);
                _stopFreezeTrackBonus = true;
            });
            
            // effect cooldown
            if (_context.BonusesParameters.FreezeTrackBonusParameters.BonusCooldown > 0)
            {
                _isFreezeTrackBonusOnCooldown = true;
                int cooldownTimerInMilliseconds =
                    Mathf.RoundToInt(_context.BonusesParameters.FreezeTrackBonusParameters.BonusCooldown * 1000);
                Task.Run(async () =>
                {
                    await Task.Delay(cooldownTimerInMilliseconds);
                    _isFreezeTrackBonusOnCooldown = false;
                });
            }
        }
        
        private void WashAllSneakers()
        {
            _context.WashAllSneakersAction();
            
            // effect cooldown
            if (_context.BonusesParameters.QuickFixWashBonusParameters.BonusCooldown > 0)
            {
                _isQuickFixWashBonusOnCooldown = true;
                int cooldownTimerInMilliseconds =
                    Mathf.RoundToInt(_context.BonusesParameters.QuickFixWashBonusParameters.BonusCooldown * 1000);
                Task.Run(async () =>
                {
                    await Task.Delay(cooldownTimerInMilliseconds);
                    _isQuickFixWashBonusOnCooldown = false;
                });
            }
        }
        
        private void LaceAllSneakers()
        {
            _context.LaceAllSneakersAction();
            
            // effect cooldown
            if (_context.BonusesParameters.QuickFixLaceBonusParameters.BonusCooldown > 0)
            {
                _isQuickFixLaceBonusOnCooldown = true;
                int cooldownTimerInMilliseconds =
                    Mathf.RoundToInt(_context.BonusesParameters.QuickFixLaceBonusParameters.BonusCooldown * 1000);
                Task.Run(async () =>
                {
                    await Task.Delay(cooldownTimerInMilliseconds);
                    _isQuickFixLaceBonusOnCooldown = false;
                });
            }
        }
    }
}