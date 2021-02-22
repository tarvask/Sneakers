using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sneaker", menuName = "Sneaker")]
public class Sneaker : ScriptableObject
{
    [SerializeField]
    private string model;
    [SerializeField]
    private int id;
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private Sprite sprite_wash;
    [SerializeField]
    private Sprite sprite_shnur;
    [SerializeField]
    private Sprite sprite_broken;
    public string Model
    {
        get
        {
            return model;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }
    }

    public Sprite Sprite
    {
        get
        {
            
            return sprite;
        }
    }

    public Sprite Sprite_wash { get => sprite_wash; }
    public Sprite Sprite_shnur { get => sprite_shnur; }
    public Sprite Sprite_broken { get => sprite_broken; }
}
