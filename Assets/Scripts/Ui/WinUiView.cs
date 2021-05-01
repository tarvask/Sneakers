using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class WinUiView : MonoBehaviour
    {
        [SerializeField] private Text levelScoreText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button mainMenuButton;

        public Text LevelScoreText => levelScoreText;
        public Button ContinueButton => continueButton;
        public Button MainMenuButton => mainMenuButton;
    }
}