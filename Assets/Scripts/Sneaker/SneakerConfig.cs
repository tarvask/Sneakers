using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Sneaker Config", menuName = "Sneaker")]
    public class SneakerConfig : ScriptableObject
    {
        [SerializeField] private string model;
        [SerializeField] private int id;
        [SerializeField] private SneakerModel prefab;

        public string Model => model;
        public int Id => id;
        public SneakerModel Prefab => prefab;
    }
}
