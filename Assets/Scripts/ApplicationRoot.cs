using UnityEngine;

namespace Sneakers
{
    public class ApplicationRoot : MonoBehaviour
    {
        private GameController _gameController;
        
        private void Start()
        {
            Application.targetFrameRate = 60;
            _gameController = new GameController();
        }
        
        private void Update()
        {
            _gameController.OuterUpdate(Time.deltaTime);
        }
    }
}