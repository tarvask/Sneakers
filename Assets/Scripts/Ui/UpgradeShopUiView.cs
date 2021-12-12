using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class UpgradeShopUiView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsCountText;
        
        [Header("Background")]
        [SerializeField] private Image background;

        [SerializeField] private Sprite backgroundNoBonusesSprite;
        [SerializeField] private Sprite backgroundWithBonusesSprite;

        [SerializeField] private float backgroundNoBonusesHeight;
        [SerializeField] private float backgroundWithBonusesHeight;

        [Header("Washing")]
        [SerializeField] private TextMeshProUGUI washTrackUpgradePriceText;
        [SerializeField] private Button washTrackUpgradeButton;
        [SerializeField] private GameObject washTrackButtonBlockerGo;
        [SerializeField] private GameObject washTrackMaxPriceLabelGo;
        [SerializeField] private Image[] washTrackUpgradeChecks;
        
        [Header("Lacing")]
        [SerializeField] private TextMeshProUGUI laceTrackUpgradePriceText;
        [SerializeField] private Button laceTrackUpgradeButton;
        [SerializeField] private GameObject laceTrackButtonBlockerGo;
        [SerializeField] private GameObject laceTrackMaxPriceLabelGo;
        [SerializeField] private Image[] laceTrackUpgradeChecks;
        
        [Header("Bonuses")]
        [SerializeField] private BonusShopItemView freezeTrackBonus;
        [SerializeField] private BonusShopItemView quickFixBonus;
        [SerializeField] private BonusShopItemView autoUtilizationBonus;
        [SerializeField] private BonusShopItemView undoBonus;

        [Space]
        [SerializeField] private Button continueButton;

        public TextMeshProUGUI CoinsCountText => coinsCountText;
        
        // background
        public Image Background => background;

        public Sprite BackgroundNoBonusesSprite => backgroundNoBonusesSprite;
        public Sprite BackgroundWithBonusesSprite => backgroundWithBonusesSprite;

        public float BackgroundNoBonusesHeight => backgroundNoBonusesHeight;
        public float BackgroundWithBonusesHeight => backgroundWithBonusesHeight;

        // washing
        public TextMeshProUGUI WashTrackUpgradePriceText => washTrackUpgradePriceText;
        public Button WashTrackUpgradeButton => washTrackUpgradeButton;
        public GameObject WashTrackButtonBlockerGo => washTrackButtonBlockerGo;
        public GameObject WashTrackMaxPriceLabelGo => washTrackMaxPriceLabelGo;
        public Image[] WashTrackUpgradeChecks => washTrackUpgradeChecks;

        // lacing
        public TextMeshProUGUI LaceTrackUpgradePriceText => laceTrackUpgradePriceText;
        public Button LaceTrackUpgradeButton => laceTrackUpgradeButton;
        public GameObject LaceTrackButtonBlockerGo => laceTrackButtonBlockerGo;
        public GameObject LaceTrackMaxPriceLabelGo => laceTrackMaxPriceLabelGo;
        public Image[] LaceTrackUpgradeChecks => laceTrackUpgradeChecks;
        
        // bonuses
        public BonusShopItemView FreezeTrackBonus => freezeTrackBonus;
        public BonusShopItemView QuickFixBonus => quickFixBonus;
        public BonusShopItemView AutoUtilizationBonus => autoUtilizationBonus;
        public BonusShopItemView UndoBonus => undoBonus;

        public Button ContinueButton => continueButton;
    }
}