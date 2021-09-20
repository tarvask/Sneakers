using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Bonuses Config", menuName = "BonusesConfig")]
    public class BonusesParametersConfig : ScriptableObject
    {
        [SerializeField] private BonusesParameters bonusesParameters;

        public BonusesParameters BonusesParameters => bonusesParameters;
    }
}