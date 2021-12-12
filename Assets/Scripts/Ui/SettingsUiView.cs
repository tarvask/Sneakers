using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class SettingsUiView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private SettingSwitchView musicSwitch;
        [SerializeField] private SettingSwitchView vibrationSwitch;

        public Button BackButton => backButton;
        public SettingSwitchView MusicSwitch => musicSwitch;
        public SettingSwitchView VibrationSwitch => vibrationSwitch;
    }
}