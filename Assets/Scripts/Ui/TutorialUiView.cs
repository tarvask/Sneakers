using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class TutorialUiView : MonoBehaviour
    {
        [SerializeField] private Text tutorialText;
        [SerializeField] private Button continueButton;

        public Text TutorialText => tutorialText;
        public Button ContinueButton => continueButton;
    }
}