using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sneakers
{
    public class SortingController
    {
        public struct Context
        {
            public SortingView View { get; }

            public Context(SortingView view)
            {
                View = view;
            }
        }

        private readonly Context _context;
        private readonly List<SneakerController> _sneakers;
        private LevelConfig _currentLevelConfig;
        
        private int _spawnedSneakersCount;
        private int _sortedSneakersCount;
        private int _score;
        private int _lives;
        
        public static SortingController instance;
        public Transform SneakersSpawnPoint => _context.View.SneakersSpawnPoint;

        public bool AllSneakersAreSorted => _sortedSneakersCount == _currentLevelConfig.NumberOfSneakers;
        public int Score => _score;
        public int Lives => _lives;

        public SortingController(Context context)
        {
            _context = context;
            instance = this;
            _sneakers = new List<SneakerController>();
        }

        public void Init(LevelConfig levelConfig)
        {
            _currentLevelConfig = levelConfig;
            
            _context.View.MainTrack.Init(this, true, _currentLevelConfig.MainTrackMovementSpeed);
            _context.View.WashTrack.Init(this, _currentLevelConfig.IsWashTrackAvailable, _currentLevelConfig.WashTrackMovementSpeed, _currentLevelConfig.WashProcessDelay);
            _context.View.LaceTrack.Init(this, _currentLevelConfig.IsLaceTrackAvailable, _currentLevelConfig.LaceTrackMovementSpeed, _currentLevelConfig.LaceProcessDelay);
            _context.View.WasteTrack.Init(this, _currentLevelConfig.IsWasteTrackAvailable);
            _context.View.WaitTrack.Init(this, _currentLevelConfig.IsWaitTrackAvailable, _currentLevelConfig.WaitTrackMovementSpeed);
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
                sneaker.View.StopAllCoroutines();
                Object.Destroy(sneaker.View.gameObject);
            }
            
            _sneakers.Clear();
            _context.View.StopAllCoroutines();
        }
        
        private IEnumerator Spawn()
        {
            yield return new WaitForSeconds(1f);
            
            Transform spawnRoot = new GameObject("Sneakers").transform;
            spawnRoot.SetParent(_context.View.SneakersSpawnPoint.parent.parent);
            spawnRoot.localPosition = Vector3.zero;

            while (_spawnedSneakersCount < _currentLevelConfig.NumberOfSneakers)
            {
                SneakerController sneaker = InstantiateSneaker(spawnRoot, _context.View.SneakersSpawnPoint.localPosition);
                SendToMainTransporter(sneaker);
                _sneakers.Add(sneaker);
                
                yield return new WaitForSeconds(_currentLevelConfig.MainTrackSpawnDelay);
            }
        }
        
        private SneakerController InstantiateSneaker(Transform spawnRoot, Vector3 position)
        {
            SneakerConfig sneakerConfig = _currentLevelConfig.PossibleSneakers[Random.Range(0, _currentLevelConfig.PossibleSneakers.Length)];
            SneakerState state = (SneakerState)Random.Range(1, 5);
            
            SneakerView newSneakerView = Object.Instantiate(sneakerConfig.Prefab, spawnRoot);
            newSneakerView.DragDropItem.canvas = _context.View.canvas;
            newSneakerView.transform.localPosition = position;
            
            SneakerController.Context sneakerControllerContext = new SneakerController.Context(newSneakerView, sneakerConfig);
            SneakerController sneakerController = new SneakerController(sneakerControllerContext, _spawnedSneakersCount);
            sneakerController.SetState(state);
            _spawnedSneakersCount++;
            
            return sneakerController;
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
            _sortedSneakersCount++;
        }

        public void OnSortSucceeded(SneakerController sneaker)
        {
            _score += _currentLevelConfig.CoinsSuccessfulStepReward;
            _context.View.TotalLabel.text = _score.ToString();
            _sortedSneakersCount++;

            _sneakers.Remove(sneaker);
            sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
            Object.Destroy(sneaker.View.gameObject);
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
            sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
            Object.Destroy(sneaker.View.gameObject);
        }
    }
}