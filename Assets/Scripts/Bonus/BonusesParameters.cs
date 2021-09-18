using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public class BonusesParameters
    {
        [SerializeField] private BonusWithDurationParameters freezeTrackBonusParameters;
        [SerializeField] private BonusWithDurationParameters quickFixWashBonusParameters;
        [SerializeField] private BonusWithDurationParameters quickFixLaceBonusParameters;
        [SerializeField] private BonusWithDurationParameters autoUtilizationBonusParameters;
        [SerializeField] private BonusParameters undoBonusParameters;
        
        public BonusWithDurationParameters FreezeTrackBonusParameters => freezeTrackBonusParameters;
        public BonusWithDurationParameters QuickFixWashBonusParameters => quickFixWashBonusParameters;
        public BonusWithDurationParameters QuickFixLaceBonusParameters => quickFixLaceBonusParameters;
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