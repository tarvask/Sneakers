using UnityEngine;

namespace Sneakers
{
    [CreateAssetMenu(fileName = "New Sneaker Config", menuName = "Sneaker")]
    public class SneakerConfig : ScriptableObject
    {
        [SerializeField] private string model;
        [SerializeField] private int id;
        [SerializeField] private Sprite sprite;
        [SerializeField] private Sprite sprite_wash;
        [SerializeField] private Sprite sprite_shnur;
        [SerializeField] private Sprite sprite_broken;

        public string Model => model;
        public int Id => id;
    
        public Sprite Sprite => sprite;
        public Sprite Sprite_wash { get => sprite_wash; }
        public Sprite Sprite_shnur { get => sprite_shnur; }
        public Sprite Sprite_broken { get => sprite_broken; }
    }
}
