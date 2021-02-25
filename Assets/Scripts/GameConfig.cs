using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Game Config", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Main track")]
        [SerializeField] private float mainTrackMovementSpeed = 5f;
        [SerializeField] private float mainTrackSpawnDelay = 2f;
        
        [Header("Wash track")]
        [SerializeField] private float washTrackMovementSpeed = 5f;
        [SerializeField] private float washProcessDelay = 5f;

        [Header("Lace track")]
        [SerializeField] private float laceTrackMovementSpeed = 5f;
        [SerializeField] private float laceProcessDelay = 5f;
        
        [Header("Wait track")]
        [SerializeField] private float waitTrackMovementSpeed = 5f;
        
        [Header("Model bin track")]
        [SerializeField] private int firstBinModelId;
        [SerializeField] private int secondBinModelId;

        public float MainTrackMovementSpeed => mainTrackMovementSpeed;
        public float MainTrackSpawnDelay => mainTrackSpawnDelay;
        public float WashTrackMovementSpeed => washTrackMovementSpeed;
        public float WashProcessDelay => washProcessDelay;
        public float LaceTrackMovementSpeed => laceTrackMovementSpeed;
        public float LaceProcessDelay => laceProcessDelay;
        public float WaitTrackMovementSpeed => waitTrackMovementSpeed;
        public int FirstBinModelId => firstBinModelId;
        public int SecondBinModelId => secondBinModelId;
    }
}