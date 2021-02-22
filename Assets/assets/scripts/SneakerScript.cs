using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SneakerScript : MonoBehaviour
{
    private string model;
    private int id;
    private Sprite sprite;
    public Coroutine route;
    public int route_index;
    public int currentPoint;
    public int state;
    public string Model { get => model; set => model = value; }
    public int Id { get => id; set => id = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
    public int State { get => state; set => state = value; }
}
