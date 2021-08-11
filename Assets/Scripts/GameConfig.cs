using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Game Config", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private bool showCheatPanel;
        [SerializeField] private LevelConfig[] levels;
        [SerializeField] private TrackLevelParams[] washTrackLevels;
        [SerializeField] private TrackLevelParams[] laceTrackLevels;
        [Space]
        [Header("Bonuses")]
        [SerializeField] private BonusesParameters bonusesParameters;

        public bool ShowCheatPanel => showCheatPanel;
        public LevelConfig[] Levels => levels;
        public TrackLevelParams[] WashTrackLevels => washTrackLevels;
        public TrackLevelParams[] LaceTrackLevels => laceTrackLevels;

        // bonuses
        public BonusesParameters BonusesParameters => bonusesParameters;
    }
}