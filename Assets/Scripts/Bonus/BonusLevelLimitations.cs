using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public class BonusLevelLimitations
    {
        [SerializeField] private bool isBonusAvailable;
        [SerializeField] private int bonusesToAddCount;
        [SerializeField] private int bonusMaxCount;
        [SerializeField] private bool isUnlimited;
        
        public bool IsBonusAvailable => isBonusAvailable;
        public int BonusesToAddCount => bonusesToAddCount;
        public int BonusMaxCount => bonusMaxCount;
        public bool IsUnlimited => isUnlimited;
    }
}