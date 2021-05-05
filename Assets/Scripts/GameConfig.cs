using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Game Config", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private LevelConfig[] levels;
        [SerializeField] private TrackLevelParams[] washTrackLevels;
        [SerializeField] private TrackLevelParams[] laceTrackLevels;

        public LevelConfig[] Levels => levels;
        public TrackLevelParams[] WashTrackLevels => washTrackLevels;
        public TrackLevelParams[] LaceTrackLevels => laceTrackLevels;
    }
}