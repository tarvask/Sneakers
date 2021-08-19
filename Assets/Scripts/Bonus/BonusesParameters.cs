using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public class BonusesParameters
    {
        [SerializeField] private BonusWithDurationParameters freezeTrackBonusParameters;
        [SerializeField] private BonusParameters quickFixWashBonusParameters;
        [SerializeField] private BonusParameters quickFixLaceBonusParameters;
        [SerializeField] private BonusWithDurationParameters autoUtilizationBonusParameters;
        [SerializeField] private BonusParameters undoBonusParameters;
        
        public BonusWithDurationParameters FreezeTrackBonusParameters => freezeTrackBonusParameters;
        public BonusParameters QuickFixWashBonusParameters => quickFixWashBonusParameters;
        public BonusParameters QuickFixLaceBonusParameters => quickFixLaceBonusParameters;
        public BonusWithDurationParameters AutoUtilizationBonusParameters => autoUtilizationBonusParameters;
        public BonusParameters UndoBonusParameters => undoBonusParameters;
    }
}