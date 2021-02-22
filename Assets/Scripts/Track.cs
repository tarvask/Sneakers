using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sneakers
{
    public class Track : MonoBehaviour, IDropHandler
    {
        public int id;
        public Transform[] points;
        public Movement movement;
        List<GameObject> list = new List<GameObject>();
        
        public void Start()
        {
            movement = Movement.instance;
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            eventData.pointerDrag.GetComponent<DragDropItem>().isDropped = true;
            if (eventData.pointerDrag != null)
            {
                switch (id)
                {
                    case 1:
                    case 2:
                    case 3:
                        if (!eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                            eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.MainRoute(eventData.pointerDrag.gameObject, eventData.pointerDrag.GetComponent<SneakerModel>().currentPoint));
                        }
                        if (eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                        }
                        break;
                    case 4:
                        if (!eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (eventData.pointerDrag.GetComponent<SneakerModel>().state == 2)
                            {

                                var x1 = movement.points[5].position.x;
                                var x2 = movement.points[4].position.x;
                                var y1 = movement.points[5].position.y;
                                var y2 = movement.points[4].position.y;
                                var x = eventData.pointerDrag.transform.position.x;
                                var y = ((x1 * y2 - x2 * y1) + x * (y1 - y2)) / (x1 - x2);
                                eventData.pointerDrag.transform.position = new Vector2(x, y);
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.WashRoute(eventData.pointerDrag.gameObject, 2));
                            }
                            else
                            {
                                eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.MainRoute(eventData.pointerDrag.gameObject, eventData.pointerDrag.GetComponent<SneakerModel>().currentPoint));
                            }
                        }
                        if (eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (eventData.pointerDrag.GetComponent<SneakerModel>().state == 2)
                            {
                                var x1 = movement.points[5].position.x;
                                var x2 = movement.points[4].position.x;
                                var y1 = movement.points[5].position.y;
                                var y2 = movement.points[4].position.y;
                                var x = eventData.pointerDrag.transform.position.x;
                                var y = ((x1 * y2 - x2 * y1) + x * (y1 - y2)) / (x1 - x2);
                                StopCoroutine(eventData.pointerDrag.GetComponent<SneakerModel>().route);
                                eventData.pointerDrag.GetComponent<DragDropItem>().isHold = false;
                                eventData.pointerDrag.GetComponent<DragDropItem>().isDropped = false;
                                eventData.pointerDrag.transform.position = new Vector2(x, y);
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.WashRoute(eventData.pointerDrag.gameObject, 2));
                            }
                            else
                            {
                                eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                            }
                        }
                        break;
                    case 5:
                        if (!eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (eventData.pointerDrag.GetComponent<SneakerModel>().state == 3)
                            {
                                eventData.pointerDrag.transform.position = movement.points[7].position;
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.ShnurRoute(eventData.pointerDrag.gameObject, 2));
                            }
                            else
                            {
                                eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.MainRoute(eventData.pointerDrag.gameObject, eventData.pointerDrag.GetComponent<SneakerModel>().currentPoint));
                            }
                        }
                        if (eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (eventData.pointerDrag.GetComponent<SneakerModel>().state == 3)
                            {
                                StopCoroutine(eventData.pointerDrag.GetComponent<SneakerModel>().route);
                                eventData.pointerDrag.GetComponent<DragDropItem>().isHold = false;
                                eventData.pointerDrag.GetComponent<DragDropItem>().isDropped = false;
                                eventData.pointerDrag.transform.position = movement.points[7].position;
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.ShnurRoute(eventData.pointerDrag.gameObject, 2));
                            }
                            else
                            {
                                eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                            }
                        }
                        break;
                    case 6:
                        if (eventData.pointerDrag.GetComponent<SneakerModel>().state == 4)
                        {
                            movement.total.text = (int.Parse(movement.total.text) + 1).ToString();
                        }
                        else
                        {
                            movement.total.text = (int.Parse(movement.total.text) - 1).ToString();
                        }
                        Destroy(eventData.pointerDrag.gameObject);
                        break;
                    case 7:
                        if (eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                        }
                        if (!eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (list.Count != 0)
                            {
                                var obj = list[list.Count - 1];
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.WaitRoute(obj, 2));
                            }
                            list.Add(eventData.pointerDrag.gameObject);
                            eventData.pointerDrag.transform.position = movement.points[9].position;
                            eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.WaitRoute(eventData.pointerDrag.gameObject, 1));
                            eventData.pointerDrag.GetComponent<DragDropItem>().isHold = true;
                        }
                    
                        break;
                    case 8:
                        if (!eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (eventData.pointerDrag.GetComponent<SneakerModel>().Id == 0 && eventData.pointerDrag.GetComponent<SneakerModel>().state == 1)
                            {
                                movement.total.text = (int.Parse(movement.total.text) + 1).ToString();
                                Destroy(eventData.pointerDrag.gameObject);
                            }
                            else
                            {
                                eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.MainRoute(eventData.pointerDrag.gameObject, eventData.pointerDrag.GetComponent<SneakerModel>().currentPoint));
                            }
                        }
                        if (eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (eventData.pointerDrag.GetComponent<SneakerModel>().Id == 0 && eventData.pointerDrag.GetComponent<SneakerModel>().state == 1)
                            {
                                movement.total.text = (int.Parse(movement.total.text) + 1).ToString();
                                Destroy(eventData.pointerDrag.gameObject);
                            }
                            else
                            {
                                eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                            }
                        }
                        break;
                    case 9:
                        if (!eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (eventData.pointerDrag.GetComponent<SneakerModel>().Id == 1 && eventData.pointerDrag.GetComponent<SneakerModel>().state == 1)
                            {
                                movement.total.text = (int.Parse(movement.total.text) + 1).ToString();
                                Destroy(eventData.pointerDrag.gameObject);
                            }
                            else
                            {
                                eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                                eventData.pointerDrag.GetComponent<SneakerModel>().route = StartCoroutine(movement.MainRoute(eventData.pointerDrag.gameObject, eventData.pointerDrag.GetComponent<SneakerModel>().currentPoint));

                            }
                        }
                        if (eventData.pointerDrag.GetComponent<DragDropItem>().isHold)
                        {
                            if (eventData.pointerDrag.GetComponent<SneakerModel>().Id == 1 && eventData.pointerDrag.GetComponent<SneakerModel>().state == 1)
                            {
                                movement.total.text = (int.Parse(movement.total.text) + 1).ToString();
                                Destroy(eventData.pointerDrag.gameObject);
                            }
                            else
                            {
                                eventData.pointerDrag.transform.position = eventData.pointerDrag.GetComponent<DragDropItem>().vector;
                            }
                        }
                        break;
                }
                /*
           if(id == 1)
            {
                var x1 = movement.points[1].position.x;
                var x2 = movement.points[0].position.x;
                var y1 = movement.points[1].position.y;
                var y2 = movement.points[0].position.y;
                var x = eventData.pointerDrag.transform.position.x;
                var y = ((x1 * y2 - x2 * y1) + x * (y1 - y2)) / (x1 - x2);
                eventData.pointerDrag.transform.position = new Vector2(x, y);
                eventData.pointerDrag.GetComponent<SneakerScript>().route = StartCoroutine(movement.MainRoute(eventData.pointerDrag.gameObject, 0));
            }
            if (id == 2)
            {
                var x1 = movement.points[2].position.x;
                var x2 = movement.points[1].position.x;
                var y1 = movement.points[2].position.y;
                var y2 = movement.points[1].position.y;
                var x = eventData.pointerDrag.transform.position.x;
                var y = ((x1 * y2 - x2 * y1) + x * (y1 - y2)) / (x1 - x2);
                eventData.pointerDrag.transform.position = new Vector2(x, y);
                eventData.pointerDrag.GetComponent<SneakerScript>().route = StartCoroutine(movement.MainRoute(eventData.pointerDrag.gameObject, 1));
            }
            if (id == 3)
            {
                var x1 = movement.points[3].position.x;
                var x2 = movement.points[2].position.x;
                var y1 = movement.points[3].position.y;
                var y2 = movement.points[2].position.y;
                var x = eventData.pointerDrag.transform.position.x;
                var y = ((x1 * y2 - x2 * y1) + x * (y1 - y2)) / (x1 - x2);
                eventData.pointerDrag.transform.position = new Vector2(x, y);
                eventData.pointerDrag.GetComponent<SneakerScript>().route = StartCoroutine(movement.MainRoute(eventData.pointerDrag.gameObject, 2));
            }
            */
           

       

            }
         
        }
    }
}
