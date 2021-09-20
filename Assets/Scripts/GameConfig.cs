using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Game Config", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private bool showCheatPanel;
        [SerializeField] private LevelConfig[] levels;
        [SerializeField] private LevelConfig endlessLevel;
        [SerializeField] private TrackLevelParams[] washTrackLevels;
        [SerializeField] private TrackLevelParams[] laceTrackLevels;
        [SerializeField] private int levelToEnableEndlessMode;
        [Space]
        [Header("Bonuses")]
        [SerializeField] private BonusesParametersConfig regularModeBonusesConfig;
        [SerializeField] private BonusesParametersConfig endlessModeBonusesConfig;

        public bool ShowCheatPanel => showCheatPanel;
        public LevelConfig[] Levels => levels;
        public LevelConfig EndlessLevel => endlessLevel;
        public TrackLevelParams[] WashTrackLevels => washTrackLevels;
        public TrackLevelParams[] LaceTrackLevels => laceTrackLevels;
        public int LevelToEnableEndlessMode => levelToEnableEndlessMode;

        // bonuses
        public BonusesParametersConfig RegularModeBonusesConfig => regularModeBonusesConfig;
        public BonusesParametersConfig EndlessModeBonusesConfig => endlessModeBonusesConfig;
    }
}