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
            public Action<bool> SwitchAutoUtilizationAction { get; }
            public Action UndoBadSortingAction { get; }

            public Context(GameModel gameModel, BonusesParameters bonusesParameters,
                Action<bool> switchFrozenStateAction,
                Action washAllSneakersAction,
                Action laceAllSneakersAction,
                Action<bool> switchAutoUtilizationAction,
                Action undoBadSortingAction)
            {
                GameModel = gameModel;
                BonusesParameters = bonusesParameters;
                SwitchFrozenStateAction = switchFrozenStateAction;
                WashAllSneakersAction = washAllSneakersAction;
                LaceAllSneakersAction = laceAllSneakersAction;
                SwitchAutoUtilizationAction = switchAutoUtilizationAction;
                UndoBadSortingAction = undoBadSortingAction;
            }
        }

        private readonly Context _context;

        private bool _isFreezeTrackBonusActive;
        private bool _stopFreezeTrackBonus;
        private bool _isFreezeTrackBonusOnCooldown;

        private bool _isQuickFixWashBonusOnCooldown;
        
        private bool _isQuickFixLaceBonusOnCooldown;

        private bool _isAutoUtilizationBonusActive;
        private bool _stopAutoUtilizationBonus;
        private bool _isAutoUtilizationBonusOnCooldown;
        
        private bool _isUndoBonusOnCooldown; 

        private readonly ReactiveProperty<bool> _isFreezeTrackBonusReady;
        private readonly ReactiveProperty<bool> _isQuickFixWashBonusReady;
        private readonly ReactiveProperty<bool> _isQuickFixLaceBonusReady;
        private readonly ReactiveProperty<bool> _isAutoUtilizationBonusReady;
        private readonly ReactiveProperty<bool> _isUndoBonusReady;

        public BonusesController(Context context)
        {
            _context = context;
            
            _isFreezeTrackBonusReady = new ReactiveProperty<bool>();
            _isQuickFixWashBonusReady = new ReactiveProperty<bool>();
            _isQuickFixLaceBonusReady = new ReactiveProperty<bool>();
            _isAutoUtilizationBonusReady = new ReactiveProperty<bool>();
            _isUndoBonusReady = new ReactiveProperty<bool>();
        }

        public void Init(BonusLevelLimitations freezeTrackBonusLimitations,
            BonusLevelLimitations quickFixWashBonusLimitations,
            BonusLevelLimitations quickFixLaceBonusLimitations,
            BonusLevelLimitations autoUtilizationBonusLimitations,
            BonusLevelLimitations undoBonusLimitations)
        {
            // track freeze
            if (freezeTrackBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < freezeTrackBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusType.TrackFreeze);

            if (freezeTrackBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusType.TrackFreeze) > freezeTrackBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusType.TrackFreeze);
            
            // quick fix wash
            if (quickFixWashBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < quickFixWashBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusType.QuickFixWash);

            if (quickFixWashBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusType.QuickFixWash) > quickFixWashBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusType.QuickFixWash);
            
            // quick fix lace
            if (quickFixLaceBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < quickFixLaceBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusType.QuickFixLace);

            if (quickFixLaceBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusType.QuickFixLace) > quickFixLaceBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusType.QuickFixLace);
            
            // auto utilization
            if (autoUtilizationBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < autoUtilizationBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusType.AutoUtilization);

            if (autoUtilizationBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusType.AutoUtilization) > autoUtilizationBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusType.AutoUtilization);
            
            // undo
            if (undoBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < undoBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusType.Undo);
            
            if (undoBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusType.Undo) > undoBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusType.Undo);
        }

        public void OuterUpdate(float frameLength)
        {
            // freeze track bonus
            if (_isFreezeTrackBonusActive && _stopFreezeTrackBonus)
            {
                _isFreezeTrackBonusActive = false;
                _context.SwitchFrozenStateAction(false);
            }
            
            if (!_isFreezeTrackBonusReady.Value && !_isFreezeTrackBonusOnCooldown)
                _isFreezeTrackBonusReady.Value = true;

            // quick fix bonus
            if (!_isQuickFixWashBonusReady.Value && !_isQuickFixWashBonusOnCooldown)
                _isQuickFixWashBonusReady.Value = true;
            
            if (!_isQuickFixLaceBonusReady.Value && !_isQuickFixLaceBonusOnCooldown)
                _isQuickFixLaceBonusReady.Value = true;
            
            // auto utilization
            if (_isAutoUtilizationBonusActive && _stopAutoUtilizationBonus)
            {
                _isAutoUtilizationBonusActive = false;
                _context.SwitchAutoUtilizationAction(false);
            }
            
            if (!_isAutoUtilizationBonusReady.Value && !_isAutoUtilizationBonusOnCooldown)
                _isAutoUtilizationBonusReady.Value = true;
            
            // undo bonus
            if (!_isUndoBonusReady.Value && !_isUndoBonusOnCooldown)
                _isUndoBonusReady.Value = true;
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
                case BonusType.AutoUtilization:
                    AutoUtilization();
                    break;
                case BonusType.Undo:
                    UndoBadSorting();
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

        private void AutoUtilization()
        {
            _context.SwitchAutoUtilizationAction(true);
            
            _isAutoUtilizationBonusActive = true;

            // effect duration
            int effectTimerInMilliseconds =
                Mathf.RoundToInt(_context.BonusesParameters.AutoUtilizationBonusParameters.BonusDuration * 1000);
            Task.Run(async () =>
            {
                await Task.Delay(effectTimerInMilliseconds);
                _stopAutoUtilizationBonus = true;
            });
            
            // effect cooldown
            if (_context.BonusesParameters.AutoUtilizationBonusParameters.BonusCooldown > 0)
            {
                _isAutoUtilizationBonusOnCooldown = true;
                int cooldownTimerInMilliseconds =
                    Mathf.RoundToInt(_context.BonusesParameters.AutoUtilizationBonusParameters.BonusCooldown * 1000);
                Task.Run(async () =>
                {
                    await Task.Delay(cooldownTimerInMilliseconds);
                    _isAutoUtilizationBonusOnCooldown = false;
                });
            }
        }

        private void UndoBadSorting()
        {
            _context.UndoBadSortingAction();
            
            // effect cooldown
            if (_context.BonusesParameters.UndoBonusParameters.BonusCooldown > 0)
            {
                _isUndoBonusOnCooldown = true;
                int cooldownTimerInMilliseconds =
                    Mathf.RoundToInt(_context.BonusesParameters.UndoBonusParameters.BonusCooldown * 1000);
                Task.Run(async () =>
                {
                    await Task.Delay(cooldownTimerInMilliseconds);
                    _isUndoBonusOnCooldown = false;
                });
            }
        }
    }
}