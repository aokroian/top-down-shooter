using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler//, IDragHandler
{
    public RectTransform background = null;
    //public RectTransform handle = null;
    //public Vector2 input = Vector2.zero;

    //private Vector2 joyPosition;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //background.gameObject.SetActive(true);
        //OnDrag(eventData);
        //joyPosition = eventData.position;
        background.position = eventData.position;
        //handle.anchoredPosition = Vector2.zero; 
    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //    //Vector2 joyDirection = eventData.position - RectTransformUtility.WorldToScreenPoint(new Camera(), background.position);
    //    //input = (joyDirection.magnitude > background.sizeDelta.x / 2f) ? joyDirection.normalized :
    //    //    joyDirection / (background.sizeDelta.x / 2f);

    //    //handle.anchoredPosition = (input * background.sizeDelta.x / 2f);
    //}

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //background.gameObject.SetActive(false);
        //input = Vector2.zero;
        //handle.anchoredPosition = Vector2.zero;
    }
}
