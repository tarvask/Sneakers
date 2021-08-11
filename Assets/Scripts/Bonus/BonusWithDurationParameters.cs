using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public class BonusWithDurationParameters : BonusParameters
    {
        [SerializeField] private float bonusDuration;
        
        public float BonusDuration => bonusDuration;
    }
}