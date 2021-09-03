using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class UpgradeShopUiView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsCountText;
        
        [SerializeField] private Text washTrackUpgradePriceText;
        [SerializeField] private Button washTrackUpgradeButton;
        
        [SerializeField] private Text laceTrackUpgradePriceText;
        [SerializeField] private Button laceTrackUpgradeButton;
        
        [Header("Bonuses")]
        [SerializeField] private BonusShopItemView freezeTrackBonus;
        [SerializeField] private BonusShopItemView quickFixBonus;
        [SerializeField] private BonusShopItemView autoUtilizationBonus;
        [SerializeField] private BonusShopItemView undoBonus;

        [Space]
        [SerializeField] private Button continueButton;

        public TextMeshProUGUI CoinsCountText => coinsCountText;

        public Text WashTrackUpgradePriceText => washTrackUpgradePriceText;
        public Button WashTrackUpgradeButton => washTrackUpgradeButton;

        public Text LaceTrackUpgradePriceText => laceTrackUpgradePriceText;
        public Button LaceTrackUpgradeButton => laceTrackUpgradeButton;
        
        // bonuses
        public BonusShopItemView FreezeTrackBonus => freezeTrackBonus;
        public BonusShopItemView QuickFixBonus => quickFixBonus;
        public BonusShopItemView AutoUtilizationBonus => autoUtilizationBonus;
        public BonusShopItemView UndoBonus => undoBonus;

        public Button ContinueButton => continueButton;
    }
}