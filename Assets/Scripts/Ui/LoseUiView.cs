using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class LoseUiView : MonoBehaviour
    {
        [SerializeField] private Button continueWithAdsButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TextMeshProUGUI countdownTimerText;
        [SerializeField] private TextMeshProUGUI noThanksText;

        public Button ContinueWithAdsButton => continueWithAdsButton;
        public Button ReplayButton => replayButton;
        public Button MainMenuButton => mainMenuButton;
        public TextMeshProUGUI CountdownTimerText => countdownTimerText;
        public TextMeshProUGUI NoThanksText => noThanksText;
    }
}