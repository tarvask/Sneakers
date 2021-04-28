using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private Transform sneakersSpawnPoint;

        [Space]
        [SerializeField] private MainTrack mainTrack;
        [SerializeField] private WashTrack washTrack;
        [SerializeField] private LaceTrack laceTrack;
        [SerializeField] private WasteTrack wasteTrack;
        [SerializeField] private WaitTrack waitTrack;
        [SerializeField] private ModelBinTrack firstModelBin;
        [SerializeField] private ModelBinTrack secondModelBin;

        [SerializeField] private SneakerConfig[] sneakers;
        public Canvas canvas;
        [SerializeField] private Text totalLabel;
        [SerializeField] private Text livesLabel;

        public Transform SneakersSpawnPoint => sneakersSpawnPoint;
        
        public static Movement instance;
        
        private int _countSneakers;
        private int _score;
        private int _lives;

        private List<SneakerController> _sneakers;

        // Start is called before the first frame update
        private void Awake()
        {
            instance = this;
        }
        
        private void Start()
        {
            mainTrack.Init(this, true, gameConfig.MainTrackMovementSpeed);
            washTrack.Init(this, gameConfig.IsWashTrackAvailable, gameConfig.WashTrackMovementSpeed, gameConfig.WashProcessDelay);
            laceTrack.Init(this, gameConfig.IsLaceTrackAvailable, gameConfig.LaceTrackMovementSpeed, gameConfig.LaceProcessDelay);
            wasteTrack.Init(this, gameConfig.IsWasteTrackAvailable);
            waitTrack.Init(this, gameConfig.IsWaitTrackAvailable, gameConfig.WaitTrackMovementSpeed);
            firstModelBin.Init(this, true, gameConfig.FirstBinModelId);
            secondModelBin.Init(this, true, gameConfig.SecondBinModelId);
            
            _sneakers = new List<SneakerController>();
            
            StartCoroutine(Spawn());
        }
    
        private SneakerController InstantiateSneaker(Transform spawnRoot, Vector3 position)
        {
            SneakerConfig sneakerConfig = sneakers[Random.Range(0, sneakers.Length)];
            SneakerState state = (SneakerState)Random.Range(1, 5);
            
            SneakerView newSneakerView = Instantiate(sneakerConfig.Prefab, spawnRoot);
            newSneakerView.DragDropItem.canvas = canvas;
            newSneakerView.transform.localPosition = position;
            
            SneakerController.Context sneakerControllerContext = new SneakerController.Context(newSneakerView, sneakerConfig);
            SneakerController sneakerController = new SneakerController(sneakerControllerContext, _countSneakers);
            sneakerController.SetState(state);
            _countSneakers++;
            
            return sneakerController;
        }
        
        private IEnumerator Spawn()
        {
            Transform spawnRoot = new GameObject("Sneakers").transform;
            spawnRoot.SetParent(sneakersSpawnPoint.parent.parent);
            spawnRoot.localPosition = Vector3.zero;

            while (_countSneakers < gameConfig.NumberOfSneakers)
            {
                SneakerController sneaker = InstantiateSneaker(spawnRoot, sneakersSpawnPoint.localPosition);
                SendToMainTransporter(sneaker);
                _sneakers.Add(sneaker);
                
                yield return new WaitForSeconds(gameConfig.MainTrackSpawnDelay);
            }
        }

        public void SendToMainTransporter(SneakerController sneaker, int mover = 0)
        {
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(mainTrack.MainRoute(sneaker, mover));
        }
        
        public void SendToWashTransporter(SneakerController sneaker, int mover = 2)
        {
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(washTrack.WashRoute(sneaker, mover));
        }
        
        public void SendToLaceTransporter(SneakerController sneaker, int mover = 2)
        {
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(laceTrack.LaceRoute(sneaker, mover));
        }
        
        public void SendToWaitTransporter(SneakerController sneaker, int mover)
        {
            sneaker.CurrentCoroutine = sneaker.View.StartCoroutine(waitTrack.WaitRoute(sneaker, mover));
        }

        public void OnSortError()
        {
            _lives--;
            livesLabel.text = _lives.ToString();
        }

        public void OnSortSucceeded(SneakerController sneaker)
        {
            _score += gameConfig.CoinsSuccessfulStepReward;
            totalLabel.text = _score.ToString();

            _sneakers.Remove(sneaker);
            sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
            Destroy(sneaker.View.gameObject);
        }

        public void OnSortFailed(SneakerController sneaker)
        {
            // do not allow negative score
            _score = Mathf.Max(0, _score + gameConfig.CoinsWrongStepReward);
            totalLabel.text = _score.ToString();
            _lives--;
            livesLabel.text = _lives.ToString();
            
            _sneakers.Remove(sneaker);
            sneaker.View.StopCoroutine(sneaker.CurrentCoroutine);
            Destroy(sneaker.View.gameObject);
        }
    }
}
