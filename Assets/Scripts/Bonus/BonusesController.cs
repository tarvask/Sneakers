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
            public BonusesParameters BonusesParameters { get; }
            public QuickFixBonusChoosingUiController QuickFixBonusChoosingUiController { get; }
            
            public Action<bool> SwitchFrozenStateAction { get; }
            public Action WashAllSneakersAction { get; }
            public Action LaceAllSneakersAction { get; }
            public Action<float> SetQuickWashAction { get; }
            public Action<float> SetQuickLaceAction { get; }
            public Action<bool> SwitchAutoUtilizationAction { get; }
            public Action WasteAllWastedSneakersAction { get; }
            public Action UndoBadSortingAction { get; }

            public Context(GameModel gameModel, BonusesParameters bonusesParameters,
                QuickFixBonusChoosingUiController quickFixBonusChoosingUiController,
                
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
                BonusesParameters = bonusesParameters;
                QuickFixBonusChoosingUiController = quickFixBonusChoosingUiController;
                
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

        private bool _isFreezeTrackBonusActive;
        private bool _stopFreezeTrackBonus;

        private bool _isAutoUtilizationBonusActive;
        private bool _stopAutoUtilizationBonus;

        private readonly Dictionary<BonusShopType, bool> _bonusesOnCooldown;

        private readonly ReactiveProperty<bool> _isFreezeTrackBonusReady;
        private readonly ReactiveProperty<bool> _isQuickFixWashBonusReady;
        private readonly ReactiveProperty<bool> _isQuickFixLaceBonusReady;
        private readonly ReactiveProperty<bool> _isAutoUtilizationBonusReady;
        private readonly ReactiveProperty<bool> _isUndoBonusReady;

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
            
            // quick fix
            // equalize counts
            if (GetBonusCount(BonusType.QuickFixWash) > GetBonusCount(BonusType.QuickFixLace))
                _context.GameModel.SpendBonus(BonusType.QuickFixWash);
            
            if (GetBonusCount(BonusType.QuickFixLace) > GetBonusCount(BonusType.QuickFixWash))
                _context.GameModel.SpendBonus(BonusType.QuickFixLace);
                
            // wash
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

            // drop cooldown
            _bonusesOnCooldown[BonusShopType.TrackFreeze] = false;
            _bonusesOnCooldown[BonusShopType.QuickFix] = false;
            _bonusesOnCooldown[BonusShopType.AutoUtilization] = false;
            _bonusesOnCooldown[BonusShopType.Undo] = false;
        }

        public void OuterUpdate(float frameLength)
        {
            // freeze track bonus
            if (_isFreezeTrackBonusActive && _stopFreezeTrackBonus)
            {
                _isFreezeTrackBonusActive = false;
                _context.SwitchFrozenStateAction(false);
            }
            
            if (!_isFreezeTrackBonusReady.Value && !_bonusesOnCooldown[BonusShopType.TrackFreeze])
                _isFreezeTrackBonusReady.Value = true;

            // quick fix bonus
            if (!_isQuickFixWashBonusReady.Value && !_bonusesOnCooldown[BonusShopType.QuickFix])
                _isQuickFixWashBonusReady.Value = true;
            
            if (!_isQuickFixLaceBonusReady.Value && !_bonusesOnCooldown[BonusShopType.QuickFix])
                _isQuickFixLaceBonusReady.Value = true;
            
            // auto utilization
            if (_isAutoUtilizationBonusActive && _stopAutoUtilizationBonus)
            {
                _isAutoUtilizationBonusActive = false;
                _context.SwitchAutoUtilizationAction(false);
            }
            
            if (!_isAutoUtilizationBonusReady.Value && !_bonusesOnCooldown[BonusShopType.AutoUtilization])
                _isAutoUtilizationBonusReady.Value = true;
            
            // undo bonus
            if (!_isUndoBonusReady.Value && !_bonusesOnCooldown[BonusShopType.Undo])
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
                Mathf.RoundToInt(_context.BonusesParameters.FreezeTrackBonusParameters.BonusDuration * 1000);
            Task.Run(async () =>
            {
                await Task.Delay(effectTimerInMilliseconds);
                _stopFreezeTrackBonus = true;
            });
            
            StartBonusCooldown(BonusShopType.QuickFix, _context.BonusesParameters.FreezeTrackBonusParameters.BonusCooldown);
            
            _context.GameModel.SpendBonus(BonusType.TrackFreeze);
        }

        private void QuickFix()
        {
            int quickFixWashBonusCount = GetBonusCount(BonusType.QuickFixWash);
            int quickFixLaceBonusCount = GetBonusCount(BonusType.QuickFixLace);
            bool isQuickWashingAvailable = quickFixWashBonusCount > 0 && quickFixWashBonusCount >= quickFixLaceBonusCount;
            bool isQuickLacingAvailable = quickFixLaceBonusCount > 0 && quickFixLaceBonusCount >= quickFixWashBonusCount;;
            _context.QuickFixBonusChoosingUiController.Show(isQuickWashingAvailable, SetQuickFixWash,
                isQuickLacingAvailable, SetQuickFixLace);
        }

        private void SetQuickFixWash()
        {
            _context.GameModel.SpendBonus(BonusType.QuickFixWash);
            _context.SetQuickWashAction(_context.BonusesParameters.QuickFixWashBonusParameters.BonusDuration);
            _context.QuickFixBonusChoosingUiController.Hide();
            
            // effect cooldown
            if (GetBonusCount(BonusType.QuickFixWash) != GetBonusCount(BonusType.QuickFixLace))
                return;
            
            StartBonusCooldown(BonusShopType.QuickFix, _context.BonusesParameters.QuickFixWashBonusParameters.BonusCooldown);
        }

        private void SetQuickFixLace()
        {
            _context.GameModel.SpendBonus(BonusType.QuickFixLace);
            _context.SetQuickLaceAction(_context.BonusesParameters.QuickFixLaceBonusParameters.BonusDuration);
            _context.QuickFixBonusChoosingUiController.Hide();
            
            // effect cooldown
            if (GetBonusCount(BonusType.QuickFixWash) != GetBonusCount(BonusType.QuickFixLace))
                return;
            
            StartBonusCooldown(BonusShopType.QuickFix, _context.BonusesParameters.QuickFixLaceBonusParameters.BonusCooldown);
        }
        
        private void WashAllDirtySneakers()
        {
            _context.WashAllSneakersAction();
            
            // effect cooldown
            if (GetBonusCount(BonusType.QuickFixWash) != GetBonusCount(BonusType.QuickFixLace))
                return;
            
            StartBonusCooldown(BonusShopType.QuickFix, _context.BonusesParameters.QuickFixWashBonusParameters.BonusCooldown);
        }
        
        private void LaceAllUnlacedSneakers()
        {
            _context.LaceAllSneakersAction();
            
            if (GetBonusCount(BonusType.QuickFixWash) != GetBonusCount(BonusType.QuickFixLace))
                return;
            
            StartBonusCooldown(BonusShopType.QuickFix, _context.BonusesParameters.QuickFixLaceBonusParameters.BonusCooldown);
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
                Mathf.RoundToInt(_context.BonusesParameters.AutoUtilizationBonusParameters.BonusDuration * 1000);
            Task.Run(async () =>
            {
                await Task.Delay(effectTimerInMilliseconds);
                _stopAutoUtilizationBonus = true;
            });
            
            StartBonusCooldown(BonusShopType.AutoUtilization, _context.BonusesParameters.AutoUtilizationBonusParameters.BonusCooldown);
            _context.GameModel.SpendBonus(BonusType.AutoUtilization);
        }

        private void UndoBadSorting()
        {
            _context.UndoBadSortingAction();
            StartBonusCooldown(BonusShopType.Undo, _context.BonusesParameters.UndoBonusParameters.BonusCooldown);
            _context.GameModel.SpendBonus(BonusType.Undo);
        }

        private void StartBonusCooldown(BonusShopType bonusType, float cooldownDuration)
        {
            if (cooldownDuration > 0)
            {
                _bonusesOnCooldown[bonusType] = true;
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