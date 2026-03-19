using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ForwardPointerToParentButton :
    MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    Button parentButton;

    void Awake() => parentButton = GetComponentInParent<Button>();

    void Send<T>(PointerEventData e, ExecuteEvents.EventFunction<T> fn) where T : IEventSystemHandler
    {
        if (parentButton) ExecuteEvents.Execute(parentButton.gameObject, e, fn);
    }

    public void OnPointerClick(PointerEventData e) => Send(e, ExecuteEvents.pointerClickHandler);
    public void OnPointerDown(PointerEventData e) => Send(e, ExecuteEvents.pointerDownHandler);
    public void OnPointerUp(PointerEventData e) => Send(e, ExecuteEvents.pointerUpHandler);
    public void OnPointerEnter(PointerEventData e) => Send(e, ExecuteEvents.pointerEnterHandler);
    public void OnPointerExit(PointerEventData e) => Send(e, ExecuteEvents.pointerExitHandler);
}