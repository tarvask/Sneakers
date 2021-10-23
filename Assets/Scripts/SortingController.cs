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
            public BonusesController BonusesController { get; }
            public Action<int> OnLegendarySneakerCollectedAction { get; }
            public Action<BonusShopType> OnBonusButtonClickedAction { get; }

            public Context(SortingView view, GameModel gameModel,
                BonusesController bonusesController,
                Action<int> onLegendarySneakerCollectedAction, Action<BonusShopType> onBonusButtonClickedAction)
            {
                View = view;
                GameModel = gameModel;
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

        private readonly BonusItemController _freezeTrackBonus;
        private readonly QuickFixBonusItemController _quickFixBonus;
        private readonly BonusItemController _autoUtilizationBonus;
        private readonly BonusItemController _undoBonus;

        private BadSortingInfo _lastBadSorting;
        
        private int _spawnedSneakersCount;
        private int _sortedSneakersCount;
        private int _score;
        private int _lives;
        private bool _isAutoUtilizationActive;
        
        public static SortingController instance;
        public Transform SneakersSpawnPoint => _context.View.SneakersSpawnPoint;
        public LevelConfig CurrentLevelConfig => _currentLevelConfig;

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
                _context.View.FreezeTrackBonus, BonusShopType.TrackFreeze,
                _context.GameModel.TrackFreezeBonusCountReactiveProperty,
                _context.BonusesController.GetBonusesReadiness(BonusShopType.TrackFreeze),
                _context.OnBonusButtonClickedAction);
            _freezeTrackBonus = new BonusItemController(freezeTrackBonusContext);
            
            QuickFixBonusItemController.Context quickFixBonusContext = new QuickFixBonusItemController.Context(
                _context.View.QuickFixBonus, BonusShopType.QuickFix,
                _context.GameModel.QuickFixBonusCountReactiveProperty,
                _context.BonusesController.GetBonusesReadiness(BonusShopType.QuickFix),
                _context.OnBonusButtonClickedAction);
            _quickFixBonus = new QuickFixBonusItemController(quickFixBonusContext);

            BonusItemController.Context autoUtilizationBonusContext = new BonusItemController.Context(
                _context.View.AutoUtilizationBonus, BonusShopType.AutoUtilization,
                _context.GameModel.AutoUtilizationBonusCountReactiveProperty,
                _context.BonusesController.GetBonusesReadiness(BonusShopType.AutoUtilization),
                _context.OnBonusButtonClickedAction);
            _autoUtilizationBonus = new BonusItemController(autoUtilizationBonusContext);
            
            BonusItemController.Context undoBonusContext = new BonusItemController.Context(
                _context.View.UndoBonus, BonusShopType.Undo,
                _context.GameModel.UndoBonusCountReactiveProperty,
                _context.BonusesController.GetBonusesReadiness(BonusShopType.Undo),
                _context.OnBonusButtonClickedAction);
            _undoBonus = new BonusItemController(undoBonusContext);
        }

        public void Init(LevelConfig levelConfig, TrackLevelParams washTrackLevel, TrackLevelParams laceTrackLevel,
            BonusesParameters bonusesParameters)
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
            _context.View.FirstModelBin.Init(this, true, _currentLevelConfig.FirstBinModelConfig);
            _context.View.SecondModelBin.Init(this, true, _currentLevelConfig.SecondBinModelConfig);
            
            _context.BonusesController.Init(bonusesParameters,
                _currentLevelConfig.FreezeTrackBonusLimitations,
                _currentLevelConfig.QuickFixBonusLimitations,
                _currentLevelConfig.QuickFixBonusLimitations,
                _currentLevelConfig.AutoUtilizationBonusLimitations,
                _currentLevelConfig.UndoBonusLimitations);
            _freezeTrackBonus.Init(_currentLevelConfig.FreezeTrackBonusLimitations.IsBonusAvailable,
                _currentLevelConfig.FreezeTrackBonusLimitations.IsUnlimited);
            _quickFixBonus.Init(_currentLevelConfig.QuickFixBonusLimitations.IsBonusAvailable,
                _currentLevelConfig.QuickFixBonusLimitations.IsUnlimited);
            _autoUtilizationBonus.Init(_currentLevelConfig.AutoUtilizationBonusLimitations.IsBonusAvailable,
                _currentLevelConfig.AutoUtilizationBonusLimitations.IsUnlimited);
            _undoBonus.Init(_currentLevelConfig.UndoBonusLimitations.IsBonusAvailable,
                _currentLevelConfig.UndoBonusLimitations.IsUnlimited);
            _lastBadSorting = null;
            
            _spawnedSneakersCount = 0;
            _sortedSneakersCount = 0;
            _lives = _currentLevelConfig.NumberOfLives;
            _context.View.LivesIndicator.SetLives(_lives);
            _score = 0;
            _context.View.TotalLabel.text = _score.ToString();
            _context.View.BestLabel.text = $"Best: {_context.GameModel.BestResultReactiveProperty.Value}";
            _context.View.BestLabel.gameObject.SetActive(_currentLevelConfig.NumberOfSneakers < 0);
            
            _context.View.StartCoroutine(Spawn());

            if (_currentLevelConfig.IsMainTrackSpeedingUp)
                _context.View.StartCoroutine(SpeedUpMainTrack());

            if (_currentLevelConfig.AreBinModelsSwitching)
                _context.View.StartCoroutine(SwitchBinsModels());
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
            bool randomSpawn = _currentLevelConfig.NumberOfSneakers == -1;

            while (_spawnedSneakersCount < _currentLevelConfig.NumberOfSneakers || randomSpawn)
            {
                SneakerController sneaker;
                
                if (randomSpawn)
                    sneaker = InstantiateRandomSneaker(_context.View.SneakersRoot, _context.View.SneakersSpawnPoint.localPosition);
                else
                    sneaker = InstantiateSneakerFromList(_context.View.SneakersRoot, _context.View.SneakersSpawnPoint.localPosition);
                
                if (_isAutoUtilizationActive && sneaker.State == SneakerState.Wasted)
                    OnSortSucceeded(sneaker);
                
                SendToMainTransporter(sneaker);
                _sneakers.Add(sneaker);
                float randomSpawnDelay = Random.Range(_currentLevelConfig.MainTrackMinSpawnDelay,
                    _currentLevelConfig.MainTrackMaxSpawnDelay);
                
                yield return new WaitForSeconds(randomSpawnDelay);
            }
        }
        
        private SneakerController InstantiateSneakerFromList(Transform spawnRoot, Vector3 position)
        {
            SneakerConfig sneakerConfig = _sneakersToSpawn[_spawnedSneakersCount].Config;
            SneakerState sneakerState = _sneakersToSpawn[_spawnedSneakersCount].State;
            
            return InstantiateSneaker(spawnRoot, position, sneakerConfig, sneakerState);
        }
        
        private SneakerController InstantiateRandomSneaker(Transform spawnRoot, Vector3 position)
        {
            // random sneaker, random state
            int sneakerToSpawnIndex = Random.Range(0, _currentLevelConfig.PossibleSneakers.Length);
            SneakerConfig sneakerConfig = _currentLevelConfig.PossibleSneakers[sneakerToSpawnIndex].Config;
            SneakerState sneakerState = (SneakerState)Random.Range((int)SneakerState.Normal, (int)SneakerState.Wasted + 1);
            
            return InstantiateSneaker(spawnRoot, position, sneakerConfig, sneakerState);
        }
        
        private SneakerController InstantiateSneaker(Transform spawnRoot, Vector3 position,
            SneakerConfig sneakerConfig, SneakerState state)
        {
            SneakerView newSneakerView = Object.Instantiate(sneakerConfig.Prefab, spawnRoot);
            newSneakerView.transform.localPosition = position;
            
            SneakerController.Context sneakerControllerContext =
                new SneakerController.Context(newSneakerView, sneakerConfig, _context.View.canvas, OnLegendarySneakerCollectedEventHandler);
            SneakerController sneakerController = new SneakerController(sneakerControllerContext, _spawnedSneakersCount);
            sneakerController.SetState(state);
            _spawnedSneakersCount++;
            
            return sneakerController;
        }

        private IEnumerator SpeedUpMainTrack()
        {
            while (true)
            {
                yield return new WaitForSeconds(_currentLevelConfig.MainTrackSpeedUpInterval);

                _context.View.MainTrack.SpeedUp(_currentLevelConfig.MainTrackSpeedUpDelta);
            }
        }

        private IEnumerator SwitchBinsModels()
        {
            while (true)
            {
                yield return new WaitForSeconds(_currentLevelConfig.BinModelsSwitchingInterval);

                SneakerConfig newFirstBinModelConfig = _currentLevelConfig
                    .PossibleSneakers[Random.Range(0, _currentLevelConfig.PossibleSneakers.Length)].Config;
                SneakerConfig newSecondBinModelConfig = _currentLevelConfig
                    .PossibleSneakers[Random.Range(0, _currentLevelConfig.PossibleSneakers.Length)].Config;
                _context.View.FirstModelBin.ChangeModel(newFirstBinModelConfig);
                _context.View.SecondModelBin.ChangeModel(newSecondBinModelConfig);
            }
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
            _lastBadSorting = null;
        }

        public void OnSortSucceeded(SneakerController sneaker)
        {
            _score += _currentLevelConfig.CoinsSuccessfulStepReward;
            _context.View.TotalLabel.text = _score.ToString();
            _sortedSneakersCount++;

            _sneakers.Remove(sneaker);
            CheckAndRemoveFromSpecialTracks(sneaker);
            sneaker.Dispose();
        }

        public void OnSortFailed(SneakerController sneaker)
        {
            // was not sorted to bin
            if (sneaker.Id == _context.View.FirstModelBin.ModelId
                || sneaker.Id == _context.View.SecondModelBin.ModelId
                // was not fixed in endless mode
                || sneaker.State != SneakerState.Normal)
            {
                _lives--;
                _context.View.LivesIndicator.SetLives(_lives);
            }
            else // could not be sorted, so count as success
            {
                _score += _currentLevelConfig.CoinsSuccessfulStepReward;
                _context.View.TotalLabel.text = _score.ToString();
            }

            _sortedSneakersCount++;
            _sneakers.Remove(sneaker);
            CheckAndRemoveFromSpecialTracks(sneaker);
            sneaker.Dispose();
            _lastBadSorting = null;
        }

        public void OnWasteFailed(SneakerController sneaker)
        {
            _lives--;
            _context.View.LivesIndicator.SetLives(_lives);
            _sortedSneakersCount++;
            
            // hide sneaker
            sneaker.View.gameObject.SetActive(false);
            
            // clear previous bad sorting
            if (_lastBadSorting != null)
            {
                _sneakers.Remove(sneaker);
                CheckAndRemoveFromSpecialTracks(sneaker);
                sneaker.Dispose();
            }
            
            // save current bad sorting
            _lastBadSorting = new BadSortingInfo(sneaker, Vector3.zero);
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
            _score += coinsReward + fineSize;
        }

        public void UpgradeWashTrack(TrackLevelParams trackLevelParams)
        {
            _context.View.WashTrack.Upgrade(trackLevelParams);
        }

        public void UpgradeLaceTrack(TrackLevelParams trackLevelParams)
        {
            _context.View.LaceTrack.Upgrade(trackLevelParams);
        }

        public void WashAllDirtySneakers()
        {
            foreach (SneakerController sneaker in _sneakers)
                if (sneaker.State == SneakerState.Dirty)
                    sneaker.SetState(SneakerState.Normal);
        }

        public void LaceAllUnlacedSneakers()
        {
            foreach (SneakerController sneaker in _sneakers)
                if (sneaker.State == SneakerState.Unlaced)
                    sneaker.SetState(SneakerState.Normal);
        }

        public void WasteAllWastedSneakers()
        {
            List<SneakerController> sneakersToWaste = new List<SneakerController>(_sneakers.Count);
            
            foreach (SneakerController sneaker in _sneakers)
                if (sneaker.State == SneakerState.Wasted)
                    sneakersToWaste.Add(sneaker);

            foreach (SneakerController wastedSneaker in sneakersToWaste)
                OnSortSucceeded(wastedSneaker);
            
            sneakersToWaste.Clear();
        }

        public void SetQuickWash(float processDuration)
        {
            _context.View.WashTrack.SetQuickFix(processDuration);
        }

        public void SetQuickLace(float processDuration)
        {
            _context.View.LaceTrack.SetQuickFix(processDuration);
        }
        
        public void SwitchAutoUtilization(bool isActive)
        {
            _isAutoUtilizationActive = isActive;
        }

        public void UndoBadSorting()
        {
            // give back life
            if (_lives < _currentLevelConfig.NumberOfLives)
            {
                _lives++;
                _context.View.LivesIndicator.SetLives(_lives);
            }

            if (_lastBadSorting == null)
                return;
            
            // reappear sneaker
            _lastBadSorting.Sneaker.View.gameObject.SetActive(true);
            // end drag state
            _lastBadSorting.Sneaker.DragDropItem.OnEndDrag(null);
            
            // back to position before drag
            if (!_lastBadSorting.Sneaker.DragDropItem.IsHold)
            {
                _lastBadSorting.Sneaker.SetPosition(_lastBadSorting.Sneaker.DragDropItem.vector);
                SendToMainTransporter(_lastBadSorting.Sneaker, _lastBadSorting.Sneaker.CurrentPoint);
            }
            // back to wait
            else
            {
                _lastBadSorting.Sneaker.SetPosition(_lastBadSorting.Sneaker.DragDropItem.vector);
            }
            
            // don't count this sneaker as sorted
            _sortedSneakersCount--;
        }
    }
}