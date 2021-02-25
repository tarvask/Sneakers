using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        
        public Transform[] points;

        [SerializeField] private Transform sneakersSpawnPoint;

        [Space]
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
        
        private int _currentPoint;
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
            washTrack.Init(this, gameConfig.WashTrackMovementSpeed, gameConfig.WashProcessDelay);
            laceTrack.Init(this, gameConfig.LaceTrackMovementSpeed, gameConfig.LaceProcessDelay);
            wasteTrack.Init(this);
            waitTrack.Init(this, gameConfig.WaitTrackMovementSpeed);
            firstModelBin.Init(this, gameConfig.FirstBinModelId);
            secondModelBin.Init(this, gameConfig.SecondBinModelId);
            
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

            while (true)
            {
                SneakerModel sneaker = InstantiateSneaker(spawnRoot, sneakersSpawnPoint.localPosition);
                SendToMainTransporter(sneaker);
                
                yield return new WaitForSeconds(gameConfig.MainTrackSpawnDelay);
            }
        }
    
        private IEnumerator MainRoute(SneakerModel sneaker, int mover)
        {
            //sneaker.GetComponent<DragDropItem>().isDropped = false;
            sneaker.SetTransporterType(TransporterType.Main);
            while (mover == 0 || mover == 1 || mover == 2)
            {
                sneaker.currentPoint = mover;
                while (points[sneaker.currentPoint + 1].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position, points[sneaker.currentPoint + 1].position, gameConfig.MainTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 3)
            {
                sneaker.currentPoint = 3;
                OnSortFailed(sneaker);
            }
            if (mover == 4)
            {
                while (points[3].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position, points[5].position, gameConfig.MainTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 5)
            {
                while (points[3].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position, points[6].position, gameConfig.MainTrackMovementSpeed);
                    yield return null;
                }
                mover++;
            }
            if (mover == 7)
            {
                while (points[3].position != sneaker.transform.position)
                {
                    sneaker.transform.position = Vector3.MoveTowards(sneaker.transform.position, points[6].position, gameConfig.MainTrackMovementSpeed);
                    yield return null;
                }
                //mover++;
            }
        }

        public void SendToMainTransporter(SneakerModel sneaker, int mover = 0)
        {
            sneaker.route = sneaker.StartCoroutine(MainRoute(sneaker, mover));
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
