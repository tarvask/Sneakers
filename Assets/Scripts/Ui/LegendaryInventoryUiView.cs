using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class LegendaryInventoryUiView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Image[] sneakers;
        [SerializeField] private SneakerConfig[] legendarySneakers;

        public Button BackButton => backButton;
        public Image[] Sneakers => sneakers;
        public SneakerConfig[] LegendarySneakers => legendarySneakers;
    }
}