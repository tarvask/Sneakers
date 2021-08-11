using System;
using System.Threading.Tasks;
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

            public Context(GameModel gameModel, BonusesParameters bonusesParameters,
                Action<bool> switchFrozenStateAction)
            {
                GameModel = gameModel;
                BonusesParameters = bonusesParameters;
                SwitchFrozenStateAction = switchFrozenStateAction;
            }
        }

        private readonly Context _context;

        private bool _isFreezeTrackBonusActive;
        private bool _stopFreezeTrackBonus;

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
        }

        public int GetBonusCount(BonusType bonusType)
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

        public void ApplyBonus(BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.TrackFreeze:
                    FreezeTrack();
                    break;
            }

            _context.GameModel.SpendBonus(bonusType);
        }

        private void FreezeTrack()
        {
            _context.SwitchFrozenStateAction(true);
            _isFreezeTrackBonusActive = true;
            int effectTimerInMilliseconds =
                Mathf.RoundToInt(_context.BonusesParameters.FreezeTrackBonusParameters.BonusDuration * 1000);

            Task.Run(async () =>
            {
                await Task.Delay(effectTimerInMilliseconds);

                _stopFreezeTrackBonus = true;
            });
        }
    }
}