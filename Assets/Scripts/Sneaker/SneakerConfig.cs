using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Sneaker Config", menuName = "Sneaker")]
    public class SneakerConfig : ScriptableObject
    {
        [SerializeField] private string model;
        [SerializeField] private int id;
        [SerializeField] private bool isLegendary;
        [SerializeField] private SneakerView prefab;
        [SerializeField] private Sprite boxSprite;

        public string Model => model;
        public int Id => id;
        public bool IsLegendary => isLegendary;
        public SneakerView Prefab => prefab;
        public Sprite BoxSprite => boxSprite;
    }
}
