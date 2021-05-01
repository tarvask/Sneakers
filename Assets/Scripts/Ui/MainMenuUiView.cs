using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class MainMenuUiView : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Text currentLevelText;

        public Button PlayButton => playButton;
        public Text CurrentLevelText => currentLevelText;
    }
}