using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class QuickFixBonusChoosingUiView : MonoBehaviour
    {
        [SerializeField] private Button chooseWashTrackButton;
        [SerializeField] private Image chooseWashTrackButtonBlocker;
        [SerializeField] private Button chooseLaceTrackButton;
        [SerializeField] private Image chooseLaceTrackButtonBlocker;
        
        public Button ChooseWashTrackButton => chooseWashTrackButton;
        public Image ChooseWashTrackButtonBlocker => chooseWashTrackButtonBlocker;
        public Button ChooseLaceTrackButton => chooseLaceTrackButton;
        public Image ChooseLaceTrackButtonBlocker => chooseLaceTrackButtonBlocker;
    }
}