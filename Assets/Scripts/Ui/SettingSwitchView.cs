using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class SettingSwitchView : MonoBehaviour
    {
        [SerializeField] private Button switchButton;
        [SerializeField] private GameObject onSwitchVariantGo;
        [SerializeField] private GameObject offSwitchVariantGo;

        public Button SwitchButton => switchButton;
        public GameObject OnSwitchVariantGo => onSwitchVariantGo;
        public GameObject OffSwitchVariantGo => offSwitchVariantGo;
    }
}