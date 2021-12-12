using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class BonusShopItemView : MonoBehaviour
    {
        [SerializeField] private Button buyBonusButton;
        [SerializeField] private TextMeshProUGUI bonusCountText;
        [SerializeField] private TextMeshProUGUI priceText;
        
        public Button BuyBonusButton => buyBonusButton;
        public TextMeshProUGUI BonusCountText => bonusCountText;
        public TextMeshProUGUI PriceText => priceText;
    }
}