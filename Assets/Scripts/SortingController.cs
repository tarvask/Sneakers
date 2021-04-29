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
            public GameConfig GameConfig { get; }

            public Context(SortingView view, GameConfig gameConfig)
            {
                View = view;
                GameConfig = gameConfig;
            }
        }

        private readonly Context _context;
        private readonly List<SneakerController> _sneakers;
        
        private int _countSneakers;
        private int _score;
        private int _lives;
        
        public static SortingController instance;
        public Transform SneakersSpawnPoint => _context.View.SneakersSpawnPoint;

        public SortingController(Context context)
        {
            _context = context;

            _lives = _context.GameConfig.NumberOfLives;
            
            instance = this;
            
            _context.View.MainTrack.Init(this, true, _context.GameConfig.MainTrackMovementSpeed);
            _context.View.WashTrack.Init(this, _context.GameConfig.IsWashTrackAvailable, _context.GameConfig.WashTrackMovementSpeed, _context.GameConfig.WashProcessDelay);
            _context.View.LaceTrack.Init(this, _context.GameConfig.IsLaceTrackAvailable, _context.GameConfig.LaceTrackMovementSpeed, _context.GameConfig.LaceProcessDelay);
            _context.View.WasteTrack.Init(this, _context.GameConfig.IsWasteTrackAvailable);
            _context.View.WaitTrack.Init(this, _context.GameConfig.IsWaitTrackAvailable, _context.GameConfig.WaitTrackMovementSpeed);
            _context.View.FirstModelBin.Init(this, true, _context.GameConfig.FirstBinModelId);
            _context.View.SecondModelBin.Init(this, true, _context.GameConfig.SecondBinModelId);
            
            _sneakers = new List<SneakerController>();
            
            _context.View.StartCoroutine(Spawn());
        }
        
        private IEnumerator Spawn()
        {
            Transform spawnRoot = new GameObject("Sneakers").transform;
            spawnRoot.SetParent(_context.View.SneakersSpawnPoint.parent.parent);
            spawnRoot.localPosition = Vector3.zero;

            while (_countSneakers < _context.GameConfig.NumberOfSneakers)
            {
                SneakerController sneaker = InstantiateSneaker(spawnRoot, _context.View.SneakersSpawnPoint.localPosition);
                SendToMainTransporter(sneaker);
                _sneakers.Add(sneaker);
                
                yield return new WaitForSeconds(_context.GameConfig.MainTrackSpawnDelay);
            }
        }
        
        private SneakerController InstantiateSneaker(Transform spawnRoot, Vector3 position)
        {
            SneakerConfig sneakerConfig = _context.GameConfig.PossibleSneakers[Random.Range(0, _context.GameConfig.PossibleSneakers.Length)];
            SneakerState state = (SneakerState)Random.Range(1, 5);
            
            SneakerView newSneakerView = Object.Instantiate(sneakerConfig.Prefab, spawnRoot);
            newSneakerView.DragDropItem.canvas = _context.View.canvas;
            newSneakerView.transform.localPosition = position;
            
            SneakerController.Context sneakerControllerContext = new SneakerController.Context(newSneakerView, sneakerConfig);
            SneakerController sneakerController = new SneakerController(sneakerControllerContext, _countSneakers);
            sneakerController.SetState(state);
            _countSneakers++;
            
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
            _score = Mathf.Max(0, _score + _context.GameConfig.CoinsWrongStepReward);
            _context.View.TotalLabel.text = _score.ToString();
            _lives--;
            _context.View.LivesLabel.text = _lives.ToString();
        }

        public void OnSortSucceeded(SneakerController sneaker)
        {
            _score += _context.GameConfig.CoinsSuccessfulStepReward;
            _context.View.TotalLabel.text = _score.ToString();

            _sneakers.Remove(sneaker);
            sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
            Object.Destroy(sneaker.View.gameObject);
        }

        public void OnSortFailed(SneakerController sneaker)
        {
            // do not allow negative score
            _score = Mathf.Max(0, _score + _context.GameConfig.CoinsWrongStepReward);
            _context.View.TotalLabel.text = _score.ToString();
            _lives--;
            _context.View.LivesLabel.text = _lives.ToString();
            
            _sneakers.Remove(sneaker);
            sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
            Object.Destroy(sneaker.View.gameObject);
        }
    }
}