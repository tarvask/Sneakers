using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class LegendUiView : MonoBehaviour
    {
        [SerializeField] private Image legendIcon;
        [SerializeField] private Text legendNameText;
        [SerializeField] private Button continueButton;

        public Image LegendIcon => legendIcon;
        public Text LegendNameText => legendNameText;
        public Button ContinueButton => continueButton;
    }
}