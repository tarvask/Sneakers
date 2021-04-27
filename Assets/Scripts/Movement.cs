using System.Collections;
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
            
            StartCoroutine(Spawn());
        }
    
        private SneakerModel InstantiateSneaker(Transform spawnRoot, Vector3 position)
        {
            SneakerConfig sneakerConfig = sneakers[Random.Range(0, sneakers.Length)];
            SneakerState state = (SneakerState)Random.Range(1, 5);
            
            SneakerModel newSneaker = Instantiate(sneakerConfig.Prefab, spawnRoot);
            newSneaker.DragDropItem.canvas = canvas;
            
            newSneaker.SetState(state);
            newSneaker.Model = sneakerConfig.Model;
            newSneaker.Id = sneakerConfig.Id;
            newSneaker.transform.localPosition = position;
            newSneaker.name = sneakerConfig.Model + _countSneakers;
            _countSneakers++;
            
            return newSneaker;
        }
        
        private IEnumerator Spawn()
        {
            Transform spawnRoot = new GameObject("Sneakers").transform;
            spawnRoot.SetParent(sneakersSpawnPoint.parent.parent);
            spawnRoot.localPosition = Vector3.zero;

            while (_countSneakers < gameConfig.NumberOfSneakers)
            {
                SneakerModel sneaker = InstantiateSneaker(spawnRoot, sneakersSpawnPoint.localPosition);
                SendToMainTransporter(sneaker);
                
                yield return new WaitForSeconds(gameConfig.MainTrackSpawnDelay);
            }
        }

        public void SendToMainTransporter(SneakerModel sneaker, int mover = 0)
        {
            sneaker.route = sneaker.StartCoroutine(mainTrack.MainRoute(sneaker, mover));
        }
        
        public void SendToWashTransporter(SneakerModel sneaker, int mover = 2)
        {
            sneaker.route = sneaker.StartCoroutine(washTrack.WashRoute(sneaker, mover));
        }
        
        public void SendToLaceTransporter(SneakerModel sneaker, int mover = 2)
        {
            sneaker.route = sneaker.StartCoroutine(laceTrack.LaceRoute(sneaker, mover));
        }
        
        public void SendToWaitTransporter(SneakerModel sneaker, int mover)
        {
            sneaker.route = sneaker.StartCoroutine(waitTrack.WaitRoute(sneaker, mover));
        }

        public void OnSortError()
        {
            _lives--;
            livesLabel.text = _lives.ToString();
        }

        public void OnSortSucceeded(SneakerModel sneaker)
        {
            _score++;
            totalLabel.text = _score.ToString();
            
            sneaker.StopCoroutine(sneaker.route);
            Destroy(sneaker.gameObject);
        }

        public void OnSortFailed(SneakerModel sneaker)
        {
            _lives--;
            livesLabel.text = _lives.ToString();
            
            sneaker.StopCoroutine(sneaker.route);
            Destroy(sneaker.gameObject);
        }
    }
}
