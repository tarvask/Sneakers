using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class CheatPanelUiView : MonoBehaviour
    {
        [SerializeField] private Button expandButton;
        [SerializeField] private GameObject panelGo;
        
        // cheat buttons
        [Space]
        [SerializeField] private Button winLevelButton;
        [SerializeField] private Button loseLevelButton;
        [SerializeField] private Button addCoinsButton;
        [SerializeField] private Button removeCoinsButton;
        [SerializeField] private Button dropProgressButton;

        public Button ExpandButton => expandButton;
        public GameObject PanelGo => panelGo;

        // cheat buttons
        public Button WinLevelButton => winLevelButton;
        public Button LoseLevelButton => loseLevelButton;
        public Button AddCoinsButton => addCoinsButton;
        public Button RemoveCoinsButton => removeCoinsButton;
        public Button DropProgressButton => dropProgressButton;
    }
}