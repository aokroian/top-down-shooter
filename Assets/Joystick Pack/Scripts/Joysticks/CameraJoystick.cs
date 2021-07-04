using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraJoystick : Joystick
{
    public override void OnPointerUp(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = SetAlpha(0.1f);
        handle.GetComponent<Image>().color = SetAlpha(0.1f);
    }
}
