using UnityEngine;

namespace Sneakers
{
    public class ApplicationRoot : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        
        private SortingController _sortingController;
        
        private void Start()
        {
            SortingView sortingView = FindObjectOfType<SortingView>();
            SortingController.Context sortingControllerContext = new SortingController.Context(sortingView, gameConfig);
            _sortingController = new SortingController(sortingControllerContext);
        }
    }
}