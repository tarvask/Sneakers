using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Game Config", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private int numberOfSneakers;
        [SerializeField] private int coinsSuccessfulStepReward;
        [SerializeField] private int coinsWrongStepReward;
        
        [Header("Main track")]
        [SerializeField] private float mainTrackMovementSpeed = 5f;
        [SerializeField] private float mainTrackSpawnDelay = 2f;

        [Header("Wash track")]
        [SerializeField] private bool isWashTrackAvailable;
        [SerializeField] private float washTrackMovementSpeed = 5f;
        [SerializeField] private float washProcessDelay = 5f;

        [Header("Lace track")]
        [SerializeField] private bool isLaceTrackAvailable;
        [SerializeField] private float laceTrackMovementSpeed = 5f;
        [SerializeField] private float laceProcessDelay = 5f;
        
        [Header("Wait track")]
        [SerializeField] private bool isWaitTrackAvailable;
        [SerializeField] private float waitTrackMovementSpeed = 5f;

        [Header("Waste track")]
        [SerializeField] private bool isWasteTrackAvailable;
        
        [Header("Model bin track")]
        [SerializeField] private int firstBinModelId;
        [SerializeField] private int secondBinModelId;

        public int NumberOfSneakers => numberOfSneakers;
        public int CoinsSuccessfulStepReward => coinsSuccessfulStepReward;
        public int CoinsWrongStepReward => coinsWrongStepReward;

        // main track
        public float MainTrackMovementSpeed => mainTrackMovementSpeed;
        public float MainTrackSpawnDelay => mainTrackSpawnDelay;
        
        // wash track
        public bool IsWashTrackAvailable => isWashTrackAvailable;
        public float WashTrackMovementSpeed => washTrackMovementSpeed;
        public float WashProcessDelay => washProcessDelay;
        
        // lace track
        public bool IsLaceTrackAvailable => isLaceTrackAvailable;
        public float LaceTrackMovementSpeed => laceTrackMovementSpeed;
        public float LaceProcessDelay => laceProcessDelay;
        
        // wait track
        public bool IsWaitTrackAvailable => isWaitTrackAvailable;
        public float WaitTrackMovementSpeed => waitTrackMovementSpeed;
        
        // waste track
        public bool IsWasteTrackAvailable => isWasteTrackAvailable;
        
        // model bins
        public int FirstBinModelId => firstBinModelId;
        public int SecondBinModelId => secondBinModelId;
    }
}