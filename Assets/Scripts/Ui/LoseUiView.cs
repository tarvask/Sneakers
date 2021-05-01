using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class LoseUiView : MonoBehaviour
    {
        [SerializeField] private Button continueWithAdsButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button mainMenuButton;

        public Button ContinueWithAdsButton => continueWithAdsButton;
        public Button ReplayButton => replayButton;
        public Button MainMenuButton => mainMenuButton;
    }
}