using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class BonusItemView : MonoBehaviour
    {
        [SerializeField] private Button useBonusButton;
        [SerializeField] private GameObject blockerGo;
        [SerializeField] private TextMeshProUGUI bonusCountText;
        [SerializeField] private TextMeshProUGUI priceText;
        
        public Button UseBonusButton => useBonusButton;
        public GameObject BlockerGo => blockerGo;
        public TextMeshProUGUI BonusCountText => bonusCountText;
        public TextMeshProUGUI PriceText => priceText;
    }
}