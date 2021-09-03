using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform background = null;

    public CustomScreenStick stick;
    public GameObject fakeStick;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);
        fakeStick.SetActive(false);

        background.position = eventData.position;
        stick.OnPointerDown(eventData); 
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        stick.OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        stick.OnPointerUp(eventData);
        background.gameObject.SetActive(false);
        fakeStick.SetActive(true);
    }
}
