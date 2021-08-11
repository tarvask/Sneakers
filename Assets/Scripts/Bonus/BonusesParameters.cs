using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public class BonusesParameters
    {
        [SerializeField] private BonusWithDurationParameters freezeTrackBonusParameters;
        
        public BonusWithDurationParameters FreezeTrackBonusParameters => freezeTrackBonusParameters;
    }
}