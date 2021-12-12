using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class TutorialUiView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tutorialHeader;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private Button continueButton;

        public TextMeshProUGUI TutorialHeader => tutorialHeader;
        public TextMeshProUGUI TutorialText => tutorialText;
        public Button ContinueButton => continueButton;
    }
}