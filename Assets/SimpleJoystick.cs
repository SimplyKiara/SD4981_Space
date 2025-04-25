using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform joystick;
    public float speed = 5f;
    private Vector2 inputVector;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 movePosition = eventData.position - (Vector2)joystick.position;
        inputVector = movePosition.normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public Vector2 GetJoystickInput()
    {
        return inputVector;
    }
}
