using TMPro;
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
        [SerializeField] private Transform sneakersRoot;

        public Canvas canvas;
        [SerializeField] private Text totalLabel;
        [SerializeField] private Text bestLabel;
        [SerializeField] private TextMeshProUGUI levelNumberLabel;
        [SerializeField] private LivesIndicator livesIndicator;

        [Space]
        [Header("Bonuses")]
        [SerializeField] private BonusItemView freezeTrackBonus;
        [SerializeField] private BonusItemView quickFixBonus;
        [SerializeField] private BonusItemView autoUtilizationBonus;
        [SerializeField] private BonusItemView undoBonus;

        public Transform SneakersSpawnPoint => sneakersSpawnPoint;

        public MainTrack MainTrack => mainTrack; 
        public WashTrack WashTrack => washTrack;
        public LaceTrack LaceTrack => laceTrack;
        public WasteTrack WasteTrack => wasteTrack;
        public WaitTrack WaitTrack => waitTrack;
        public ModelBinTrack FirstModelBin => firstModelBin;
        public ModelBinTrack SecondModelBin => secondModelBin;
        public Transform SneakersRoot => sneakersRoot;

        public Text TotalLabel => totalLabel;
        public Text BestLabel => bestLabel;
        public TextMeshProUGUI LevelNumberLabel => levelNumberLabel;
        public LivesIndicator LivesIndicator => livesIndicator;

        // bonuses
        public BonusItemView FreezeTrackBonus => freezeTrackBonus;
        public BonusItemView QuickFixBonus => quickFixBonus;
        public BonusItemView AutoUtilizationBonus => autoUtilizationBonus;
        public BonusItemView UndoBonus => undoBonus;
    }
}
