using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("begin");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("onDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("end");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("point");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
