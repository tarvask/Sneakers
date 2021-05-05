using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class UpgradeShopUiView : MonoBehaviour
    {
        [SerializeField] private Text washTrackUpgradePriceText;
        [SerializeField] private Button washTrackUpgradeButton;
        
        [SerializeField] private Text laceTrackUpgradePriceText;
        [SerializeField] private Button laceTrackUpgradeButton;

        [SerializeField] private Button continueButton;

        public Text WashTrackUpgradePriceText => washTrackUpgradePriceText;
        public Button WashTrackUpgradeButton => washTrackUpgradeButton;

        public Text LaceTrackUpgradePriceText => laceTrackUpgradePriceText;
        public Button LaceTrackUpgradeButton => laceTrackUpgradeButton;

        public Button ContinueButton => continueButton;
    }
}