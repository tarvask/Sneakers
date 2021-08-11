using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class SortingView : MonoBehaviour
    {
        [SerializeField] private Transform sneakersSpawnPoint;

        [Space]
        [SerializeField] private MainTrack mainTrack;
        [SerializeField] private WashTrack washTrack;
        [SerializeField] private LaceTrack laceTrack;
        [SerializeField] private WasteTrack wasteTrack;
        [SerializeField] private WaitTrack waitTrack;
        [SerializeField] private ModelBinTrack firstModelBin;
        [SerializeField] private ModelBinTrack secondModelBin;

        public Canvas canvas;
        [SerializeField] private Text totalLabel;
        [SerializeField] private Text livesLabel;

        [Space]
        [Header("Bonuses")]
        [SerializeField] private BonusItemView freezeTrackBonus;
        [SerializeField] private BonusItemView quickFixWashBonus;
        [SerializeField] private BonusItemView quickFixLaceBonus;
        [SerializeField] private BonusItemView autoUtilizationBonus;

        public Transform SneakersSpawnPoint => sneakersSpawnPoint;

        public MainTrack MainTrack => mainTrack; 
        public WashTrack WashTrack => washTrack;
        public LaceTrack LaceTrack => laceTrack;
        public WasteTrack WasteTrack => wasteTrack;
        public WaitTrack WaitTrack => waitTrack;
        public ModelBinTrack FirstModelBin => firstModelBin;
        public ModelBinTrack SecondModelBin => secondModelBin;

        public Text TotalLabel => totalLabel;
        public Text LivesLabel => livesLabel;

        // bonuses
        public BonusItemView FreezeTrackBonus => freezeTrackBonus;
        public BonusItemView QuickFixWashBonus => quickFixWashBonus;
        public BonusItemView QuickFixLaceBonus => quickFixLaceBonus;
        public BonusItemView AutoUtilizationBonus => autoUtilizationBonus;
    }
}
