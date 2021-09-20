using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Level Config", menuName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private int numberOfSneakers;
        [SerializeField] private int numberOfLives;
        [SerializeField] private int coinsSuccessfulLevelReward;
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
        [SerializeField] private bool isMainTrackSpeedingUp;
        [SerializeField] private float mainTrackSpeedUpInterval;
        [SerializeField] private float mainTrackSpeedUpDelta;

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
        [SerializeField] private bool areBinModelsSwitching;
        [SerializeField] private float binModelsSwitchingInterval;
        
        [Space]
        [Header("Tutorial")]
        [TextArea]
        [SerializeField] private string tutorialText;

        [Space]
        [Header("Upgrade shop")]
        [SerializeField] private bool showUpgradeShop;

        [Space]
        [Header("Bonuses")]
        [SerializeField] private BonusLevelLimitations freezeTrackBonusLimitations;
        [SerializeField] private BonusLevelLimitations quickFixBonusLimitations;
        [SerializeField] private BonusLevelLimitations autoUtilizationBonusLimitations;
        [SerializeField] private BonusLevelLimitations undoBonusLimitations;

        public int NumberOfSneakers => numberOfSneakers;
        public int NumberOfLives => numberOfLives;
        public int CoinsSuccessfulLevelReward => coinsSuccessfulLevelReward;
        public int CoinsSuccessfulStepReward => coinsSuccessfulStepReward;
        public int CoinsWrongStepReward => coinsWrongStepReward;

        public bool ShufflePossibleSneakers => shufflePossibleSneakers;
        public PossibleSneakerParams[] PossibleSneakers => possibleSneakers;

        // main track
        public float MainTrackMovementSpeed => mainTrackMovementSpeed;
        public float MainTrackMinSpawnDelay => mainTrackMinSpawnDelay;
        public float MainTrackMaxSpawnDelay => mainTrackMaxSpawnDelay;
        public bool IsMainTrackSpeedingUp => isMainTrackSpeedingUp;
        public float MainTrackSpeedUpInterval => mainTrackSpeedUpInterval;
        public float MainTrackSpeedUpDelta => mainTrackSpeedUpDelta;

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
        public bool AreBinModelsSwitching => areBinModelsSwitching;
        public float BinModelsSwitchingInterval => binModelsSwitchingInterval;
        
        // tutorial
        public string TutorialText => tutorialText;
        
        // upgrade shop
        public bool ShowUpgradeShop => showUpgradeShop;
        
        // bonuses
        public BonusLevelLimitations FreezeTrackBonusLimitations => freezeTrackBonusLimitations;
        public BonusLevelLimitations QuickFixBonusLimitations => quickFixBonusLimitations;
        public BonusLevelLimitations AutoUtilizationBonusLimitations => autoUtilizationBonusLimitations;
        public BonusLevelLimitations UndoBonusLimitations => undoBonusLimitations;

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