using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class MainMenuUiView : MonoBehaviour
    {
        [SerializeField] private Button regularModeButton;
        [SerializeField] private Text currentLevelText;
        [SerializeField] private Button endlessModeButton;
        [SerializeField] private Button legendaryInventoryButton;
        [SerializeField] private Button settingsButton;

        public Button RegularModeButton => regularModeButton;
        public Text CurrentLevelText => currentLevelText;
        public Button EndlessModeButton => endlessModeButton;
        public Button LegendaryInventoryButton => legendaryInventoryButton;
        public Button SettingsButton => settingsButton;
    }
}