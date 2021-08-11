using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public class BonusParameters
    {
        [SerializeField] private float bonusCooldown;
        [SerializeField] private int bonusPrice;
        
        public float BonusCooldown => bonusCooldown;
        public int BonusPrice => bonusPrice;
    }
}