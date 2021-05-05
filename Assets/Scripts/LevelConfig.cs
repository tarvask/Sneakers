using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Level Config", menuName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private int numberOfSneakers;
        [SerializeField] private int numberOfLives;
        [SerializeField] private int coinsSuccessfulStepReward;
        [SerializeField] private int coinsWrongStepReward;

        [Space]
        [Header("Possible sneakers")]
        [SerializeField] private bool shufflePossibleSneakers;
        [SerializeField] private PossibleSneakerParams[] possibleSneakers;
        
        [Header("Main track")]
        [SerializeField] private float mainTrackMovementSpeed = 5f;
        [SerializeField] private float mainTrackMinSpawnDelay = 2f;
        [SerializeField] private float mainTrackMaxSpawnDelay = 2f;

        [Header("Wash track")]
        [SerializeField] private bool isWashTrackAvailable;

        [Header("Lace track")]
        [SerializeField] private bool isLaceTrackAvailable;
        
        [Header("Wait track")]
        [SerializeField] private bool isWaitTrackAvailable;
        [SerializeField] private float waitTrackMovementSpeed = 5f;
        [SerializeField] private bool isWaitTrackMovingToWaste;

        [Header("Waste track")]
        [SerializeField] private bool isWasteTrackAvailable;
        
        [Header("Model bin track")]
        [SerializeField] private int firstBinModelId;
        [SerializeField] private int secondBinModelId;
        
        [Space]
        [Header("Tutorial")]
        [TextArea]
        [SerializeField] private string tutorialText;

        [Space]
        [Header("Upgrade shop")]
        [SerializeField] private bool showUpgradeShop;

        public int NumberOfSneakers => numberOfSneakers;
        public int NumberOfLives => numberOfLives;
        public int CoinsSuccessfulStepReward => coinsSuccessfulStepReward;
        public int CoinsWrongStepReward => coinsWrongStepReward;

        public bool ShufflePossibleSneakers => shufflePossibleSneakers;
        public PossibleSneakerParams[] PossibleSneakers => possibleSneakers;

        // main track
        public float MainTrackMovementSpeed => mainTrackMovementSpeed;
        public float MainTrackMinSpawnDelay => mainTrackMinSpawnDelay;
        public float MainTrackMaxSpawnDelay => mainTrackMaxSpawnDelay;
        
        // wash track
        public bool IsWashTrackAvailable => isWashTrackAvailable;
        
        // lace track
        public bool IsLaceTrackAvailable => isLaceTrackAvailable;
        
        // wait track
        public bool IsWaitTrackAvailable => isWaitTrackAvailable;
        public float WaitTrackMovementSpeed => waitTrackMovementSpeed;
        
        // waste track
        public bool IsWasteTrackAvailable => isWasteTrackAvailable;
        public bool IsWaitTrackMovingToWaste => isWaitTrackMovingToWaste;
        
        // model bins
        public int FirstBinModelId => firstBinModelId;
        public int SecondBinModelId => secondBinModelId;
        
        // tutorial
        public string TutorialText => tutorialText;
        
        // upgrade shop
        public bool ShowUpgradeShop => showUpgradeShop;

        [ContextMenu("Validate number of sneakers")]
        public void ValidateNumberOfSneakers()
        {
            int sneakersToSort = 0;

            foreach (PossibleSneakerParams possibleSneakerItem in possibleSneakers)
            {
                sneakersToSort += possibleSneakerItem.Count;
            }

            numberOfSneakers = sneakersToSort;
        }
    }
}