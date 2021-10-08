using System;
using System.Collections.Generic;
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
            
            public Action<bool> SwitchFrozenStateAction { get; }
            public Action WashAllSneakersAction { get; }
            public Action LaceAllSneakersAction { get; }
            public Action<float> SetQuickWashAction { get; }
            public Action<float> SetQuickLaceAction { get; }
            public Action<bool> SwitchAutoUtilizationAction { get; }
            public Action WasteAllWastedSneakersAction { get; }
            public Action UndoBadSortingAction { get; }

            public Context(GameModel gameModel,
                
                Action<bool> switchFrozenStateAction,
                Action washAllSneakersAction,
                Action laceAllSneakersAction,
                Action<float> setQuickWashAction,
                Action<float> setQuickLaceAction,
                Action<bool> switchAutoUtilizationAction,
                Action wasteAllWastedSneakersAction,
                Action undoBadSortingAction)
            {
                GameModel = gameModel;

                SwitchFrozenStateAction = switchFrozenStateAction;
                WashAllSneakersAction = washAllSneakersAction;
                LaceAllSneakersAction = laceAllSneakersAction;
                SetQuickWashAction = setQuickWashAction;
                SetQuickLaceAction = setQuickLaceAction;
                SwitchAutoUtilizationAction = switchAutoUtilizationAction;
                WasteAllWastedSneakersAction = wasteAllWastedSneakersAction;
                UndoBadSortingAction = undoBadSortingAction;
            }
        }

        private readonly Context _context;

        private BonusesParameters _currentBonusesParameters;
        
        private bool _isFreezeTrackBonusActive;
        private bool _stopFreezeTrackBonus;

        private bool _isAutoUtilizationBonusActive;
        private bool _stopAutoUtilizationBonus;

        private readonly Dictionary<BonusShopType, bool> _bonusesOnCooldown;
        private readonly Dictionary<BonusShopType, bool> _bonusesUnlimited;
        private readonly Dictionary<BonusShopType, ReactiveProperty<bool>> _bonusesReadiness;

        public IReadOnlyReactiveProperty<bool> GetBonusesReadiness(BonusShopType bonusType)
        {
            return _bonusesReadiness[bonusType];
        }

        public BonusesController(Context context)
        {
            _context = context;
            
            _bonusesOnCooldown = new Dictionary<BonusShopType, bool>
            {
                {BonusShopType.TrackFreeze, false},
                {BonusShopType.QuickFix, false},
                {BonusShopType.AutoUtilization, false},
                {BonusShopType.Undo, false}
            };
            
            _bonusesUnlimited = new Dictionary<BonusShopType, bool>
            {
                {BonusShopType.TrackFreeze, false},
                {BonusShopType.QuickFix, false},
                {BonusShopType.AutoUtilization, false},
                {BonusShopType.Undo, false}
            };
            
            _bonusesReadiness = new Dictionary<BonusShopType, ReactiveProperty<bool>>
            {
                {BonusShopType.TrackFreeze, new ReactiveProperty<bool>()},
                {BonusShopType.QuickFix, new ReactiveProperty<bool>()},
                {BonusShopType.AutoUtilization, new ReactiveProperty<bool>()},
                {BonusShopType.Undo, new ReactiveProperty<bool>()}
            };
        }

        public void Init(BonusesParameters bonusesParameters,
            BonusLevelLimitations freezeTrackBonusLimitations,
            BonusLevelLimitations quickFixWashBonusLimitations,
            BonusLevelLimitations quickFixLaceBonusLimitations,
            BonusLevelLimitations autoUtilizationBonusLimitations,
            BonusLevelLimitations undoBonusLimitations)
        {
            _currentBonusesParameters = bonusesParameters;
            
            // track freeze
            if (freezeTrackBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < freezeTrackBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusShopType.TrackFreeze);

            if (freezeTrackBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusShopType.TrackFreeze) > freezeTrackBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusShopType.TrackFreeze);
            
            // quick fix
            if (quickFixWashBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < quickFixWashBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusShopType.QuickFix);

            if (quickFixWashBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusShopType.QuickFix) > quickFixWashBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusShopType.QuickFix);

            // auto utilization
            if (autoUtilizationBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < autoUtilizationBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusShopType.AutoUtilization);

            if (autoUtilizationBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusShopType.AutoUtilization) > autoUtilizationBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusShopType.AutoUtilization);
            
            // undo
            if (undoBonusLimitations.BonusesToAddCount > 0)
                for (int i = 0; i < undoBonusLimitations.BonusesToAddCount; i++)
                    _context.GameModel.AddBonus(BonusShopType.Undo);
            
            if (undoBonusLimitations.BonusMaxCount != -1)
                while (GetBonusCount(BonusShopType.Undo) > undoBonusLimitations.BonusMaxCount)
                    _context.GameModel.SpendBonus(BonusShopType.Undo);

            // drop cooldown
            _bonusesOnCooldown[BonusShopType.TrackFreeze] = false;
            _bonusesOnCooldown[BonusShopType.QuickFix] = false;
            _bonusesOnCooldown[BonusShopType.AutoUtilization] = false;
            _bonusesOnCooldown[BonusShopType.Undo] = false;
            
            // set unlimited parameter
            _bonusesUnlimited[BonusShopType.TrackFreeze] = freezeTrackBonusLimitations.IsUnlimited;
            _bonusesUnlimited[BonusShopType.QuickFix] = quickFixLaceBonusLimitations.IsUnlimited;
            _bonusesUnlimited[BonusShopType.AutoUtilization] = autoUtilizationBonusLimitations.IsUnlimited;
            _bonusesUnlimited[BonusShopType.Undo] = undoBonusLimitations.IsUnlimited;
        }

        public void OuterUpdate(float frameLength)
        {
            // freeze track bonus
            if (_isFreezeTrackBonusActive && _stopFreezeTrackBonus)
            {
                _isFreezeTrackBonusActive = false;
                _context.SwitchFrozenStateAction(false);
            }
            
            if (!_bonusesReadiness[BonusShopType.TrackFreeze].Value && !_bonusesOnCooldown[BonusShopType.TrackFreeze])
                _bonusesReadiness[BonusShopType.TrackFreeze].Value = true;

            // quick fix bonus
            if (!_bonusesReadiness[BonusShopType.QuickFix].Value && !_bonusesOnCooldown[BonusShopType.QuickFix])
                _bonusesReadiness[BonusShopType.QuickFix].Value = true;
            
            // auto utilization
            if (_isAutoUtilizationBonusActive && _stopAutoUtilizationBonus)
            {
                _isAutoUtilizationBonusActive = false;
                _context.SwitchAutoUtilizationAction(false);
            }
            
            if (!_bonusesReadiness[BonusShopType.AutoUtilization].Value && !_bonusesOnCooldown[BonusShopType.AutoUtilization])
                _bonusesReadiness[BonusShopType.AutoUtilization].Value = true;
            
            // undo bonus
            if (!_bonusesReadiness[BonusShopType.Undo].Value && !_bonusesOnCooldown[BonusShopType.Undo])
                _bonusesReadiness[BonusShopType.Undo].Value = true;
        }

        private int GetBonusCount(BonusShopType bonusType)
        {
            switch (bonusType)
            {
                case BonusShopType.TrackFreeze:
                    return _context.GameModel.TrackFreezeBonusCountReactiveProperty.Value;
                case BonusShopType.QuickFix:
                    return _context.GameModel.QuickFixBonusCountReactiveProperty.Value;
                case BonusShopType.AutoUtilization:
                    return _context.GameModel.AutoUtilizationBonusCountReactiveProperty.Value;
                case BonusShopType.Undo:
                    return _context.GameModel.UndoBonusCountReactiveProperty.Value;
            }

            throw new ArgumentException("Unknown bonus type");
        }

        public void ApplyBonus(BonusShopType bonusType)
        {
            switch (bonusType)
            {
                case BonusShopType.TrackFreeze:
                    FreezeTrack();
                    break;
                case BonusShopType.QuickFix:
                    QuickFix();
                    break;
                case BonusShopType.AutoUtilization:
                    AutoUtilization();
                    break;
                case BonusShopType.Undo:
                    UndoBadSorting();
                    break;
            }
        }

        private void FreezeTrack()
        {
            _context.SwitchFrozenStateAction(true);
            
            _isFreezeTrackBonusActive = true;
            _stopFreezeTrackBonus = false;

            // effect duration
            int effectTimerInMilliseconds =
                Mathf.RoundToInt(_currentBonusesParameters.FreezeTrackBonusParameters.BonusDuration * 1000);
            Task.Run(async () =>
            {
                await Task.Delay(effectTimerInMilliseconds);
                _stopFreezeTrackBonus = true;
            });
            
            StartBonusCooldown(BonusShopType.TrackFreeze, _currentBonusesParameters.FreezeTrackBonusParameters.BonusCooldown);
            
            if (!_bonusesUnlimited[BonusShopType.TrackFreeze])
                _context.GameModel.SpendBonus(BonusShopType.TrackFreeze);
        }

        private void QuickFix()
        {
            SetQuickFixWash();
            SetQuickFixLace();
            
            if (!_bonusesUnlimited[BonusShopType.QuickFix])
                _context.GameModel.SpendBonus(BonusShopType.QuickFix);
        }

        private void SetQuickFixWash()
        {
            _context.SetQuickWashAction(_currentBonusesParameters.QuickFixBonusParameters.BonusDuration);

            // effect cooldown
            StartBonusCooldown(BonusShopType.QuickFix, _currentBonusesParameters.QuickFixBonusParameters.BonusCooldown);
        }

        private void SetQuickFixLace()
        {
            _context.SetQuickLaceAction(_currentBonusesParameters.QuickFixBonusParameters.BonusDuration);
            
            // effect cooldown
            StartBonusCooldown(BonusShopType.QuickFix, _currentBonusesParameters.QuickFixBonusParameters.BonusCooldown);
        }
        
        private void WashAllDirtySneakers()
        {
            _context.WashAllSneakersAction();
            
            // effect cooldown
            StartBonusCooldown(BonusShopType.QuickFix, _currentBonusesParameters.QuickFixBonusParameters.BonusCooldown);
        }
        
        private void LaceAllUnlacedSneakers()
        {
            _context.LaceAllSneakersAction();
            
            // effect cooldown
            StartBonusCooldown(BonusShopType.QuickFix, _currentBonusesParameters.QuickFixBonusParameters.BonusCooldown);
        }

        private void WasteAllWastedSneakers()
        {
            _context.WasteAllWastedSneakersAction();
        }

        private void AutoUtilization()
        {
            WasteAllWastedSneakers();
            _context.SwitchAutoUtilizationAction(true);
            
            _isAutoUtilizationBonusActive = true;

            // effect duration
            int effectTimerInMilliseconds =
                Mathf.RoundToInt(_currentBonusesParameters.AutoUtilizationBonusParameters.BonusDuration * 1000);
            Task.Run(async () =>
            {
                await Task.Delay(effectTimerInMilliseconds);
                _stopAutoUtilizationBonus = true;
            });
            
            StartBonusCooldown(BonusShopType.AutoUtilization, _currentBonusesParameters.AutoUtilizationBonusParameters.BonusCooldown);
            
            if (!_bonusesUnlimited[BonusShopType.AutoUtilization])
                _context.GameModel.SpendBonus(BonusShopType.AutoUtilization);
        }

        private void UndoBadSorting()
        {
            _context.UndoBadSortingAction();
            StartBonusCooldown(BonusShopType.Undo, _currentBonusesParameters.UndoBonusParameters.BonusCooldown);
            
            if (!_bonusesUnlimited[BonusShopType.Undo])
                _context.GameModel.SpendBonus(BonusShopType.Undo);
        }

        private void StartBonusCooldown(BonusShopType bonusType, float cooldownDuration)
        {
            if (cooldownDuration > 0)
            {
                _bonusesOnCooldown[bonusType] = true;
                _bonusesReadiness[bonusType].Value = false;
                int cooldownTimerInMilliseconds =
                    Mathf.RoundToInt(cooldownDuration * 1000);
                Task.Run(async () =>
                {
                    await Task.Delay(cooldownTimerInMilliseconds);
                    _bonusesOnCooldown[bonusType] = false;
                });
            }

            // set cooldown without timer, to make it available only once per level
            if (cooldownDuration < 0)
                _bonusesOnCooldown[bonusType] = true;
        }
    }
}