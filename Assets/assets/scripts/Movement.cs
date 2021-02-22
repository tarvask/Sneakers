using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public static Movement instance;
    public float movementSpeed = 5F;
    public Transform[] points;
    public Sneaker[] sneakers;
    private int currentPoint = 0;
    private float delay = 2F;
    private int countSneakers = 0;
    public Canvas canvas;
    public Text total;
    
    // Start is called before the first frame update
    private void Awake()
    {
            instance = this;
    }
    void Start()
    {
        
        StartCoroutine("Spawn");
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    GameObject InstantiateSneaker()
    {
        int i = Random.Range(0, 2);
        var obj = new GameObject();
        int state = Random.Range(1, 5);
        obj.AddComponent<CanvasGroup>();
        obj.transform.SetParent(points[0]);
        SneakerScript sneaker = obj.AddComponent<SneakerScript>();
        Image objSprite = obj.AddComponent<Image>();
        var drag_drop = obj.AddComponent<DragDropItem>();
        drag_drop.canvas = canvas;
        obj.name = sneakers[i].Model + countSneakers.ToString();
        countSneakers++;
        sneaker.State = state;
        if(state == 1)
        {
            sneaker.Sprite = sneakers[i].Sprite;
            objSprite.sprite = sneakers[i].Sprite;
        }
        if(state == 2)
        {
            sneaker.Sprite = sneakers[i].Sprite_wash;
            objSprite.sprite = sneakers[i].Sprite_wash;
        }
        if(state == 3)
        {
            sneaker.Sprite = sneakers[i].Sprite_shnur;
            objSprite.sprite = sneakers[i].Sprite_shnur;
        }
        if(state == 4)
        {
            sneaker.Sprite = sneakers[i].Sprite_broken;
            objSprite.sprite = sneakers[i].Sprite_broken;
        }
        sneaker.Model = sneakers[i].Model;
        sneaker.Id = sneakers[i].Id;
        obj.transform.localScale = new Vector3(2F, 1.5F, 1);
        obj.transform.position = points[0].position;
        return obj;
    }
    IEnumerator Spawn()
    {
        while(true)
        {
            var obj = InstantiateSneaker();
            if(obj != null)
            {
               obj.GetComponent<SneakerScript>().route = StartCoroutine(MainRoute(obj, 0));
               
            }
            yield return new WaitForSeconds(delay);
        }
    }
    
    public IEnumerator MainRoute(GameObject obj, int mover)
    {
        obj.GetComponent<DragDropItem>().isDropped = false;
        obj.GetComponent<SneakerScript>().route_index = 0;
        if (mover == 0)
        {
            obj.GetComponent<SneakerScript>().currentPoint = 0;
            while (points[1].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[1].position, movementSpeed);
                yield return new WaitForFixedUpdate();
                
            }
            mover++;
        }
        if (mover == 1)
        {
            while (points[2].position != obj.transform.position)
            {
                obj.GetComponent<SneakerScript>().currentPoint = 1;
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[2].position, movementSpeed);
                yield return new WaitForFixedUpdate();
                
            }
            mover++;
        }
        if (mover == 2)
        {
            obj.GetComponent<SneakerScript>().currentPoint = 2;
            while (points[3].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[3].position, movementSpeed);
                yield return new WaitForFixedUpdate();

            }
            mover++;
        }
        if(mover == 3)
        {
            obj.GetComponent<SneakerScript>().currentPoint = 3;
            if(obj.GetComponent<SneakerScript>().state == 4)
            {
                total.text = (int.Parse(total.text) + 1).ToString();
            }
            else
            {
                total.text = (int.Parse(total.text) - 1).ToString();
            }
            Destroy(obj);
        }
        if (mover == 4)
        {
            while (points[3].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[5].position, movementSpeed);
                yield return new WaitForFixedUpdate();

            }
            mover++;
        }
        if (mover == 5)
        {
            while (points[3].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[6].position, movementSpeed);
                yield return new WaitForFixedUpdate();

            }
            mover++;
        }
        if (mover == 7)
        {
            while (points[3].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[6].position, movementSpeed);
                yield return new WaitForFixedUpdate();

            }
            mover++;
        }
    }
    public IEnumerator WashRoute(GameObject obj, int mover)
    {
        obj.GetComponent<SneakerScript>().route_index = 1;
        obj.GetComponent<DragDropItem>().isDropped = false;
        if (mover == 2)
        {
            obj.GetComponent<SneakerScript>().currentPoint = 2;
            while (points[5].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[5].position, movementSpeed);
                yield return new WaitForFixedUpdate();


            }
            for(int i = 0; i < 5 ; i++){
                obj.SetActive(false);
                yield return new WaitForSeconds(1F);
            }
            obj.SetActive(true);
            mover++;
            
        }
        if (mover == 3)
        {
            
            obj.GetComponent<SneakerScript>().state = 1;
            obj.GetComponent<SneakerScript>().Sprite = sneakers[obj.GetComponent<SneakerScript>().Id].Sprite;
            obj.GetComponent<Image>().sprite = sneakers[obj.GetComponent<SneakerScript>().Id].Sprite;
            obj.GetComponent<SneakerScript>().currentPoint = 3;
            while (points[6].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[6].position, movementSpeed);
                yield return new WaitForFixedUpdate();
                
            }
            mover++;
        }
        if (mover == 4)
        {
            obj.GetComponent<SneakerScript>().currentPoint = 4;
            while (points[0].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[0].position, movementSpeed);
                yield return new WaitForFixedUpdate();

            }
            mover++;
        }
        if (mover == 5)
        {
            obj.GetComponent<SneakerScript>().currentPoint = 5;
            obj.GetComponent<SneakerScript>().route = StartCoroutine(MainRoute(obj, 0));
        }
    }
    public IEnumerator ShnurRoute(GameObject obj, int mover)
    {
        obj.GetComponent<DragDropItem>().isDropped = false;
        obj.GetComponent<SneakerScript>().route_index = 2;
        if (mover == 2)
        {
            
            for (int i = 0; i < 50; i++)
            {
                obj.SetActive(false) ;
                yield return new WaitForSeconds(0.1F);
            }
            obj.GetComponent<SneakerScript>().state = 1;
            obj.GetComponent<SneakerScript>().Sprite = sneakers[obj.GetComponent<SneakerScript>().Id].Sprite;
            obj.GetComponent<Image>().sprite = sneakers[obj.GetComponent<SneakerScript>().Id].Sprite;
            obj.GetComponent<SneakerScript>().currentPoint = 2;
            obj.GetComponent<CanvasGroup>().alpha = 1F;
            obj.GetComponent<CanvasGroup>().blocksRaycasts = true;
            obj.SetActive(enabled);
            while (points[8].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[8].position, movementSpeed);
                yield return new WaitForFixedUpdate();

            }
            mover++;
        }
        if (mover == 3)
        {
            obj.GetComponent<SneakerScript>().currentPoint = 3;
            while (points[0].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[0].position, movementSpeed);
                yield return new WaitForFixedUpdate();

            }
            mover++;
        }
        if (mover == 4)
        {
            obj.GetComponent<SneakerScript>().currentPoint = 4;
            obj.GetComponent<SneakerScript>().route = StartCoroutine(MainRoute(obj, 0));

        }
    }
    public IEnumerator WaitRoute(GameObject obj, int mover)
    {
        if (mover == 1)
        {
            while (points[10].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[10].position, movementSpeed);
                yield return new WaitForFixedUpdate();
            }
            StopCoroutine(obj.GetComponent<SneakerScript>().route);
            Debug.Log("stopped");
        }
        if (mover == 2)
        {

            while (points[11].position != obj.transform.position)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, points[11].position, movementSpeed);
                yield return new WaitForFixedUpdate();

            }
            StopCoroutine(obj.GetComponent<SneakerScript>().route);
            Debug.Log("stopped");
        }
    }
}
