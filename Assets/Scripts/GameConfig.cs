using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Game Config", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private LevelConfig[] levels;

        public LevelConfig[] Levels => levels;
    }
}