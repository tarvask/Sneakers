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
        
        public bool IsBonusAvailable => isBonusAvailable;
        public int BonusesToAddCount => bonusesToAddCount;
        public int BonusMaxCount => bonusMaxCount;
    }
}