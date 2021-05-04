using UnityEngine;

namespace Sneakers
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;

        [Space] [Header("Windows")]
        [SerializeField] private MainMenuUiView mainMenuUi;
        [SerializeField] private TutorialUiView tutorialUi;
        [SerializeField] private WinUiView winUi;
        [SerializeField] private LoseUiView loseUi;
        [SerializeField] private LegendUiView legendUi;

        public GameConfig GameConfig => gameConfig;
        
        // windows
        public MainMenuUiView MainMenuUi => mainMenuUi;
        public TutorialUiView TutorialUi => tutorialUi;
        public WinUiView WinUi => winUi;
        public LoseUiView LoseUi => loseUi;
        public LegendUiView LegendUi => legendUi;
    }
}