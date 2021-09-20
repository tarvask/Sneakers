using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class MainMenuUiView : MonoBehaviour
    {
        [SerializeField] private Button regularModeButton;
        [SerializeField] private Text currentLevelText;
        [SerializeField] private Button endlessModeButton;

        public Button RegularModeButton => regularModeButton;
        public Text CurrentLevelText => currentLevelText;
        public Button EndlessModeButton => endlessModeButton;
    }
}