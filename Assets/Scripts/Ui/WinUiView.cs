using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class WinUiView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelScoreText;
        [SerializeField] private Button continueButton;

        public TextMeshProUGUI LevelScoreText => levelScoreText;
        public Button ContinueButton => continueButton;
    }
}