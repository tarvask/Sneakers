using System;
using UniRx;
using UnityEngine;

namespace Sneakers
{
    public class GameModel
    {
        private GameState _currentState;
        private int _currentLevel;
        
        private readonly ReactiveProperty<int> _coinsReactiveProperty;
        private readonly ReactiveProperty<int> _washTrackLevelReactiveProperty;
        private readonly ReactiveProperty<int> _laceTrackLevelReactiveProperty;
        private readonly ReactiveProperty<int> _bestResultReactiveProperty;
        // bonuses
        private readonly ReactiveProperty<int> _trackFreezeBonusCountReactiveProperty;
        private readonly ReactiveProperty<int> _quickFixBonusCountReactiveProperty;
        private readonly ReactiveProperty<int> _autoUtilizationBonusCountReactiveProperty;
        private readonly ReactiveProperty<int> _undoBonusCountReactiveProperty;

        public GameState CurrentState => _currentState;
        public int CurrentLevel => _currentLevel;

        public IReadOnlyReactiveProperty<int> CoinsReactiveProperty => _coinsReactiveProperty;
        public IReadOnlyReactiveProperty<int> WashTrackLevelReactiveProperty => _washTrackLevelReactiveProperty;
        public IReadOnlyReactiveProperty<int> LaceTrackLevelReactiveProperty => _laceTrackLevelReactiveProperty;
        public ReactiveProperty<int> BestResultReactiveProperty => _bestResultReactiveProperty;

        // bonuses
        public IReadOnlyReactiveProperty<int> TrackFreezeBonusCountReactiveProperty => _trackFreezeBonusCountReactiveProperty;
        public IReadOnlyReactiveProperty<int> QuickFixBonusCountReactiveProperty => _quickFixBonusCountReactiveProperty;
        public IReadOnlyReactiveProperty<int> AutoUtilizationBonusCountReactiveProperty => _autoUtilizationBonusCountReactiveProperty;
        public IReadOnlyReactiveProperty<int> UndoBonusCountReactiveProperty => _undoBonusCountReactiveProperty;

        public GameModel()
        {
            _currentLevel = PlayerPrefs.GetInt(GameConstants.CurrentLevelStorageName, 1);
            _coinsReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.CoinsStorageName, 0));
            _washTrackLevelReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.CurrentWashTrackLevelStorageName, 0));
            _laceTrackLevelReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.CurrentLaceTrackLevelStorageName, 0));
            _bestResultReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.BestResultStorageName, 0));
            
            _trackFreezeBonusCountReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.TrackFreezeBonusCountStorageName, 0));
            _quickFixBonusCountReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.QuickFixBonusCountStorageName, 0));
            _autoUtilizationBonusCountReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.AutoUtilizationBonusCountStorageName, 0));
            _undoBonusCountReactiveProperty = new ReactiveProperty<int>(PlayerPrefs.GetInt(GameConstants.UndoBonusCountStorageName, 0));
        }
        
        public void DropProgress()
        {
            // level
            _currentLevel = 1;
            PlayerPrefs.SetInt(GameConstants.CurrentLevelStorageName, _currentLevel);
            // tracks
            _washTrackLevelReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.CurrentWashTrackLevelStorageName, _washTrackLevelReactiveProperty.Value);
            _laceTrackLevelReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.CurrentLaceTrackLevelStorageName, _laceTrackLevelReactiveProperty.Value);
            // coins
            _coinsReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.CoinsStorageName, 0);
            // legends
            PlayerPrefs.SetString(GameConstants.CollectedLegendarySneakersStorageName, "");
            // best result
            _bestResultReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.BestResultStorageName, 0);
            // bonuses
            _trackFreezeBonusCountReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.TrackFreezeBonusCountStorageName, 0);
            _quickFixBonusCountReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.QuickFixBonusCountStorageName, 0);
            _autoUtilizationBonusCountReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.AutoUtilizationBonusCountStorageName, 0);
            _undoBonusCountReactiveProperty.Value = 0;
            PlayerPrefs.SetInt(GameConstants.UndoBonusCountStorageName, 0);
        }
        
        public bool SpendMoney(int coinsCount)
        {
            if (coinsCount > _coinsReactiveProperty.Value)
                return false;
            
            _coinsReactiveProperty.Value -= coinsCount;
            PlayerPrefs.SetInt(GameConstants.CoinsStorageName, _coinsReactiveProperty.Value);
            return true;
        }

        public void AddMoney(int coinsCount)
        {
            _coinsReactiveProperty.Value = Mathf.Clamp(_coinsReactiveProperty.Value + coinsCount, 0, Int32.MaxValue);
            PlayerPrefs.SetInt(GameConstants.CoinsStorageName, _coinsReactiveProperty.Value);
        }
        
        public void SpendBonus(BonusShopType bonusType)
        {
            switch (bonusType)
            {
                case BonusShopType.TrackFreeze:
                    _trackFreezeBonusCountReactiveProperty.Value--;
                    break;
                case BonusShopType.QuickFix:
                    _quickFixBonusCountReactiveProperty.Value--;
                    break;
                case BonusShopType.AutoUtilization:
                    _autoUtilizationBonusCountReactiveProperty.Value--;
                    break;
                case BonusShopType.Undo:
                    _undoBonusCountReactiveProperty.Value--;
                    break;
                default:
                    throw new ArgumentException("Unknown bonus type");
            }
        }
        
        public void AddBonus(BonusShopType bonusType)
        {
            switch (bonusType)
            {
                case BonusShopType.TrackFreeze:
                    _trackFreezeBonusCountReactiveProperty.Value++;
                    break;
                case BonusShopType.QuickFix:
                    _quickFixBonusCountReactiveProperty.Value++;
                    break;
                case BonusShopType.AutoUtilization:
                    _autoUtilizationBonusCountReactiveProperty.Value++;
                    break;
                case BonusShopType.Undo:
                    _undoBonusCountReactiveProperty.Value++;
                    break;
                default:
                    throw new ArgumentException("Unknown bonus type");
            }
        }

        public void NextLevel()
        {
            _currentLevel++;
            PlayerPrefs.SetInt(GameConstants.CurrentLevelStorageName, _currentLevel);
        }
        
        public void ChangeState(GameState newState)
        {
            _currentState = newState;
        }

        public bool IsPlayingState(GameState state)
        {
            return state == GameState.Playing;
        }

        public void UpgradeWashMachine()
        {
            _washTrackLevelReactiveProperty.Value++;
            PlayerPrefs.SetInt(GameConstants.CurrentWashTrackLevelStorageName, _washTrackLevelReactiveProperty.Value);
        }
        
        public void UpgradeLaceMachine()
        {
            _laceTrackLevelReactiveProperty.Value++;
            PlayerPrefs.SetInt(GameConstants.CurrentLaceTrackLevelStorageName, _laceTrackLevelReactiveProperty.Value);
        }

        public void SetBestResult(int newBest)
        {
            if (newBest <= _bestResultReactiveProperty.Value)
                return;
            
            _bestResultReactiveProperty.Value = newBest;
            PlayerPrefs.SetInt(GameConstants.BestResultStorageName, _bestResultReactiveProperty.Value);
        }
    }
}