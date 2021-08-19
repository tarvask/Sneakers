using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Sneakers
{
    public class SortingController
    {
        public struct Context
        {
            public SortingView View { get; }
            public GameModel GameModel { get; }
            public BonusesParameters BonusesParameters { get; }
            public BonusesController BonusesController { get; }
            public Action<int> OnLegendarySneakerCollectedAction { get; }
            public Action<BonusType> OnBonusButtonClickedAction { get; }

            public Context(SortingView view, GameModel gameModel, BonusesParameters bonusesParameters,
                BonusesController bonusesController,
                Action<int> onLegendarySneakerCollectedAction, Action<BonusType> onBonusButtonClickedAction)
            {
                View = view;
                GameModel = gameModel;
                BonusesParameters = bonusesParameters;
                BonusesController = bonusesController;
                OnLegendarySneakerCollectedAction = onLegendarySneakerCollectedAction;
                OnBonusButtonClickedAction = onBonusButtonClickedAction;
            }
        }

        private readonly Context _context;
        private readonly List<SneakerController> _sneakers;
        private readonly Dictionary<int, int> _collectedLegendarySneakers;
        private List<PossibleSneakerParams> _sneakersToSpawn;
        private LevelConfig _currentLevelConfig;
        private Transform _spawnRoot;

        private readonly BonusItemController _freezeTrackBonus;
        private readonly BonusItemController _quickFixWashBonus;
        private readonly BonusItemController _quickFixLaceBonus;
        private readonly BonusItemController _autoUtilizationBonus;
        
        private int _spawnedSneakersCount;
        private int _sortedSneakersCount;
        private int _score;
        private int _lives;
        private bool _isAutoUtilizationActive;
        
        public static SortingController instance;
        public Transform SneakersSpawnPoint => _context.View.SneakersSpawnPoint;

        public Dictionary<int, int> CollectedLegendarySneakers => _collectedLegendarySneakers;
        public bool AllSneakersAreSorted => _sortedSneakersCount == _currentLevelConfig.NumberOfSneakers;
        public int Score => _score;
        public int Lives => _lives;

        public SortingController(Context context)
        {
            _context = context;
            instance = this;
            _sneakersToSpawn = new List<PossibleSneakerParams>();
            _sneakers = new List<SneakerController>();
            _collectedLegendarySneakers = new Dictionary<int, int>();
            
            // bonuses
            BonusItemController.Context freezeTrackBonusContext = new BonusItemController.Context(
                _context.View.FreezeTrackBonus, BonusType.TrackFreeze, _context.BonusesParameters.FreezeTrackBonusParameters,
                _context.GameModel.TrackFreezeBonusCountReactiveProperty,
                _context.GameModel.CoinsReactiveProperty, _context.OnBonusButtonClickedAction);
            _freezeTrackBonus = new BonusItemController(freezeTrackBonusContext);
            
            BonusItemController.Context quickFixWashBonusContext = new BonusItemController.Context(
                _context.View.QuickFixWashBonus, BonusType.QuickFixWash, _context.BonusesParameters.QuickFixWashBonusParameters,
                _context.GameModel.QuickFixWashBonusCountReactiveProperty,
                _context.GameModel.CoinsReactiveProperty, _context.OnBonusButtonClickedAction);
            _quickFixWashBonus = new BonusItemController(quickFixWashBonusContext);
            
            BonusItemController.Context quickFixLaceBonusContext = new BonusItemController.Context(
                _context.View.QuickFixLaceBonus, BonusType.QuickFixLace, _context.BonusesParameters.QuickFixLaceBonusParameters,
                _context.GameModel.QuickFixLaceBonusCountReactiveProperty,
                _context.GameModel.CoinsReactiveProperty, _context.OnBonusButtonClickedAction);
            _quickFixLaceBonus = new BonusItemController(quickFixLaceBonusContext);
            
            BonusItemController.Context autoUtilizationBonusContext = new BonusItemController.Context(
                _context.View.AutoUtilizationBonus, BonusType.AutoUtilization, _context.BonusesParameters.AutoUtilizationBonusParameters,
                _context.GameModel.AutoUtilizationBonusCountReactiveProperty,
                _context.GameModel.CoinsReactiveProperty, _context.OnBonusButtonClickedAction);
            _autoUtilizationBonus = new BonusItemController(autoUtilizationBonusContext);
        }

        public void Init(LevelConfig levelConfig, TrackLevelParams washTrackLevel, TrackLevelParams laceTrackLevel)
        {
            _currentLevelConfig = levelConfig;

            // prepare stuff for spawn
            foreach (PossibleSneakerParams possibleSneakerParams in _currentLevelConfig.PossibleSneakers)
            {
                for (int countIndex = 0; countIndex < possibleSneakerParams.Count; countIndex++)
                {
                    _sneakersToSpawn.Add(possibleSneakerParams);
                }
            }
            
            if (_currentLevelConfig.ShufflePossibleSneakers)
                CollectionsExtensions.ShuffleList(ref _sneakersToSpawn);
            
            _context.View.MainTrack.Init(this, true, _currentLevelConfig.MainTrackMovementSpeed);
            _context.View.WashTrack.Init(this, _currentLevelConfig.IsWashTrackAvailable, washTrackLevel);
            _context.View.LaceTrack.Init(this, _currentLevelConfig.IsLaceTrackAvailable, laceTrackLevel);
            _context.View.WasteTrack.Init(this, _currentLevelConfig.IsWasteTrackAvailable);
            _context.View.WaitTrack.Init(this, _currentLevelConfig.IsWaitTrackAvailable, _currentLevelConfig.WaitTrackMovementSpeed, _currentLevelConfig.IsWaitTrackMovingToWaste);
            _context.View.FirstModelBin.Init(this, true, _currentLevelConfig.FirstBinModelId);
            _context.View.SecondModelBin.Init(this, true, _currentLevelConfig.SecondBinModelId);
            
            _context.BonusesController.Init(_currentLevelConfig.FreezeTrackBonusLimitations,
                _currentLevelConfig.QuickFixWashBonusLimitations,
                _currentLevelConfig.QuickFixLaceBonusLimitations,
                _currentLevelConfig.AutoUtilizationBonusLimitations);
            _freezeTrackBonus.Init(_currentLevelConfig.FreezeTrackBonusLimitations.IsBonusAvailable);
            _quickFixWashBonus.Init(_currentLevelConfig.QuickFixWashBonusLimitations.IsBonusAvailable);
            _quickFixLaceBonus.Init(_currentLevelConfig.QuickFixLaceBonusLimitations.IsBonusAvailable);
            _autoUtilizationBonus.Init(_currentLevelConfig.AutoUtilizationBonusLimitations.IsBonusAvailable);
            
            _spawnedSneakersCount = 0;
            _sortedSneakersCount = 0;
            _lives = _currentLevelConfig.NumberOfLives;
            _context.View.LivesIndicator.SetLives(_lives);
            _score = 0;
            _context.View.TotalLabel.text = _score.ToString();
            
            _context.View.StartCoroutine(Spawn());
        }

        public void Clear()
        {
            foreach (SneakerController sneaker in _sneakers)
            {
                sneaker.Dispose();
            }

            _sneakersToSpawn.Clear();
            _sneakers.Clear();
            _collectedLegendarySneakers.Clear();
            _context.View.StopAllCoroutines();
        }
        
        private IEnumerator Spawn()
        {
            yield return new WaitForSeconds(1f);

            if (_spawnRoot == null)
            {
                _spawnRoot = new GameObject("Sneakers").transform;
                _spawnRoot.SetParent(_context.View.SneakersSpawnPoint.parent.parent);
                _spawnRoot.localPosition = Vector3.zero;
                _spawnRoot.localScale = Vector3.one;
            }

            while (_spawnedSneakersCount < _currentLevelConfig.NumberOfSneakers)
            {
                SneakerController sneaker = InstantiateSneaker(_spawnRoot, _context.View.SneakersSpawnPoint.localPosition);
                
                if (_isAutoUtilizationActive && sneaker.State == SneakerState.Wasted)
                    OnSortSucceeded(sneaker);
                
                SendToMainTransporter(sneaker);
                _sneakers.Add(sneaker);
                float randomSpawnDelay = Random.Range(_currentLevelConfig.MainTrackMinSpawnDelay,
                    _currentLevelConfig.MainTrackMaxSpawnDelay);
                
                yield return new WaitForSeconds(randomSpawnDelay);
            }
        }
        
        private SneakerController InstantiateSneaker(Transform spawnRoot, Vector3 position)
        {
            SneakerConfig sneakerConfig = _sneakersToSpawn[_spawnedSneakersCount].Config;
            SneakerState state = _sneakersToSpawn[_spawnedSneakersCount].State;
            
            SneakerView newSneakerView = Object.Instantiate(sneakerConfig.Prefab, spawnRoot);
            newSneakerView.transform.localPosition = position;
            
            SneakerController.Context sneakerControllerContext =
                new SneakerController.Context(newSneakerView, sneakerConfig, _context.View.canvas, OnLegendarySneakerCollectedEventHandler);
            SneakerController sneakerController = new SneakerController(sneakerControllerContext, _spawnedSneakersCount);
            sneakerController.SetState(state);
            _spawnedSneakersCount++;
            
            return sneakerController;
        }

        private void CheckAndRemoveFromSpecialTracks(SneakerController sneakerController, TransporterType trackToIgnore = TransporterType.Undefined)
        {
            if (trackToIgnore != TransporterType.Washing)
                _context.View.WashTrack.CheckAndRemoveFromTrack(sneakerController);
            
            if (trackToIgnore != TransporterType.Lacing)
                _context.View.LaceTrack.CheckAndRemoveFromTrack(sneakerController);
            
            if (trackToIgnore != TransporterType.Waiting)
                _context.View.WaitTrack.CheckAndRemoveFromTrack(sneakerController);
        }
        
        private void OnLegendarySneakerCollectedEventHandler(SneakerController sneaker)
        {
            if (_collectedLegendarySneakers.ContainsKey(sneaker.Id))
                _collectedLegendarySneakers[sneaker.Id]++;
            else
                _collectedLegendarySneakers.Add(sneaker.Id, 1);

            _context.OnLegendarySneakerCollectedAction.Invoke(sneaker.Id);
            OnSortSucceeded(sneaker);
        }
        
        public void SendToMainTransporter(SneakerController sneaker, int mover = 0, bool isImmediately = false)
        {
            CheckAndRemoveFromSpecialTracks(sneaker);
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(_context.View.MainTrack.MainRoute(sneaker, mover, isImmediately));
        }
        
        public void SendToWashTransporter(SneakerController sneaker, int mover = 2)
        {
            CheckAndRemoveFromSpecialTracks(sneaker, TransporterType.Washing);
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(_context.View.WashTrack.WashRoute(sneaker, mover));
        }
        
        public void SendToLaceTransporter(SneakerController sneaker, int mover = 2)
        {
            CheckAndRemoveFromSpecialTracks(sneaker, TransporterType.Lacing);
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(_context.View.LaceTrack.LaceRoute(sneaker, mover));
        }
        
        public void SendToWaitTransporter(SneakerController sneaker, int mover)
        {
            CheckAndRemoveFromSpecialTracks(sneaker, TransporterType.Waiting);
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(_context.View.WaitTrack.WaitRoute(sneaker, mover));
        }
        
        public void OnSortError()
        {
            _lives--;
            _context.View.LivesIndicator.SetLives(_lives);
        }

        public void OnSortSucceeded(SneakerController sneaker)
        {
            _context.View.TotalLabel.text = _score.ToString();
            _sortedSneakersCount++;

            _sneakers.Remove(sneaker);
            CheckAndRemoveFromSpecialTracks(sneaker);
            sneaker.Dispose();
        }

        public void OnSortFailed(SneakerController sneaker)
        {
            _lives--;
            _context.View.LivesIndicator.SetLives(_lives);
            _sortedSneakersCount++;
            
            _sneakers.Remove(sneaker);
            CheckAndRemoveFromSpecialTracks(sneaker);
            sneaker.Dispose();
        }

        public void OnSortLegendaryError(SneakerController sneaker)
        {
            _lives = 0;
            
            _sneakers.Remove(sneaker);
            CheckAndRemoveFromSpecialTracks(sneaker);
            sneaker.Dispose();
        }

        public void CountScore()
        {
            int coinsReward = _currentLevelConfig.CoinsSuccessfulLevelReward;
            int fineSize = (_currentLevelConfig.NumberOfLives - _lives) * _currentLevelConfig.CoinsWrongStepReward; 
            _score = coinsReward + fineSize;
        }

        public void UpgradeWashTrack(TrackLevelParams trackLevelParams)
        {
            _context.View.WashTrack.Upgrade(trackLevelParams);
        }

        public void UpgradeLaceTrack(TrackLevelParams trackLevelParams)
        {
            _context.View.LaceTrack.Upgrade(trackLevelParams);
        }

        public void WashAllSneakers()
        {
            foreach (SneakerController sneaker in _sneakers)
                if (sneaker.State == SneakerState.Dirty)
                    sneaker.SetState(SneakerState.Normal);
        }

        public void LaceAllSneakers()
        {
            foreach (SneakerController sneaker in _sneakers)
                if (sneaker.State == SneakerState.Unlaced)
                    sneaker.SetState(SneakerState.Normal);
        }
        
        public void SwitchAutoUtilization(bool isActive)
        {
            _isAutoUtilizationActive = isActive;
        }
    }
}