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

        public int GetBonusPrice(BonusShopType bonusType)
        {
            switch (bonusType)
            {
                case BonusShopType.TrackFreeze:
                    return freezeTrackBonusParameters.BonusPrice;
                case BonusShopType.QuickFix:
                    // use quick fix wash, but lace can be used likewise
                    return quickFixWashBonusParameters.BonusPrice;
                    //return quickFixLaceBonusParameters.BonusPrice;
                case BonusShopType.AutoUtilization:
                    return autoUtilizationBonusParameters.BonusPrice;
                case BonusShopType.Undo:
                    return undoBonusParameters.BonusPrice;
                default:
                    throw new ArgumentException("Unknown bonus type");
            }
        }
    }
}