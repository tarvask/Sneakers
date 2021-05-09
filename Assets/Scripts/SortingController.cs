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
            public Action<int> OnLegendarySneakerCollectedAction { get; }

            public Context(SortingView view, Action<int> onLegendarySneakerCollectedAction)
            {
                View = view;
                OnLegendarySneakerCollectedAction = onLegendarySneakerCollectedAction;
            }
        }

        private readonly Context _context;
        private readonly List<SneakerController> _sneakers;
        private readonly Dictionary<int, int> _collectedLegendarySneakers;
        private List<PossibleSneakerParams> _sneakersToSpawn;
        private LevelConfig _currentLevelConfig;
        private Transform _spawnRoot;
        
        private int _spawnedSneakersCount;
        private int _sortedSneakersCount;
        private int _score;
        private int _lives;
        
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
            
            _spawnedSneakersCount = 0;
            _sortedSneakersCount = 0;
            _lives = _currentLevelConfig.NumberOfLives;
            _context.View.LivesLabel.text = _lives.ToString();
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
            newSneakerView.DragDropItem.canvas = _context.View.canvas;
            newSneakerView.transform.localPosition = position;
            
            SneakerController.Context sneakerControllerContext = new SneakerController.Context(newSneakerView, sneakerConfig, OnLegendarySneakerCollectedEventHandler);
            SneakerController sneakerController = new SneakerController(sneakerControllerContext, _spawnedSneakersCount);
            sneakerController.SetState(state);
            _spawnedSneakersCount++;
            
            return sneakerController;
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
        
        public void SendToMainTransporter(SneakerController sneaker, int mover = 0)
        {
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(_context.View.MainTrack.MainRoute(sneaker, mover));
        }
        
        public void SendToWashTransporter(SneakerController sneaker, int mover = 2)
        {
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(_context.View.WashTrack.WashRoute(sneaker, mover));
        }
        
        public void SendToLaceTransporter(SneakerController sneaker, int mover = 2)
        {
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(_context.View.LaceTrack.LaceRoute(sneaker, mover));
        }
        
        public void SendToWaitTransporter(SneakerController sneaker, int mover)
        {
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(_context.View.WaitTrack.WaitRoute(sneaker, mover));
        }
        
        public void OnSortError()
        {
            // do not allow negative score
            _score = Mathf.Max(0, _score + _currentLevelConfig.CoinsWrongStepReward);
            _context.View.TotalLabel.text = _score.ToString();
            _lives--;
            _context.View.LivesLabel.text = _lives.ToString();
            //_sortedSneakersCount++;
        }

        public void OnSortSucceeded(SneakerController sneaker)
        {
            _score += _currentLevelConfig.CoinsSuccessfulStepReward;
            _context.View.TotalLabel.text = _score.ToString();
            _sortedSneakersCount++;

            _sneakers.Remove(sneaker);

            sneaker.Dispose();
        }

        public void OnSortFailed(SneakerController sneaker)
        {
            // do not allow negative score
            _score = Mathf.Max(0, _score + _currentLevelConfig.CoinsWrongStepReward);
            _context.View.TotalLabel.text = _score.ToString();
            _lives--;
            _context.View.LivesLabel.text = _lives.ToString();
            _sortedSneakersCount++;
            
            _sneakers.Remove(sneaker);
            sneaker.Dispose();
        }

        public void OnSortLegendaryError(SneakerController sneaker)
        {
            _lives = 0;
            
            _sneakers.Remove(sneaker);
            sneaker.Dispose();
        }

        public void UpgradeWashTrack(TrackLevelParams trackLevelParams)
        {
            _context.View.WashTrack.Upgrade(trackLevelParams);
        }

        public void UpgradeLaceTrack(TrackLevelParams trackLevelParams)
        {
            _context.View.LaceTrack.Upgrade(trackLevelParams);
        }
    }
}