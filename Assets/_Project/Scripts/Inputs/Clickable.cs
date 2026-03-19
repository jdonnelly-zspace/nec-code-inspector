
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using zSpace.Core.EventSystems;
using zSpace.Core.Input;

namespace InputHandlers
{
    public class Clickable : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler, IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //#region Public Members
        public event Action<PointerEventData> OnPointerEnterEvent;
        public event Action<PointerEventData> OnPointerExitEvent;
        public event Action<PointerEventData> OnPointerDownEvent;
        public event Action<PointerEventData> OnPointerUpEvent;
        public event Action<PointerEventData> OnPointerClickEvent;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_grabbableParent != null)
            {
                _grabbableParent.OnPointerEnter(eventData);
            }
            if (OnPointerEnterEvent != null)
            {
                OnPointerEnterEvent(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_grabbableParent != null)
            {
                _grabbableParent.OnPointerExit(eventData);
            }
            if (OnPointerExitEvent != null)
            {
                OnPointerExitEvent(eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null ||
                pointerEventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (OnPointerDownEvent != null)
            {
                OnPointerDownEvent(eventData);
            }


            // Capture pointer events.
            pointerEventData.Pointer.CapturePointer(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null ||
                pointerEventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (OnPointerUpEvent != null)
            {
                OnPointerUpEvent(eventData);
            }

            // Capture pointer events.
            pointerEventData.Pointer.CapturePointer(null);
        }

        public void OnPointerClick(PointerEventData eventData)

        {
            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null ||
                pointerEventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (OnPointerClickEvent != null)
            {
                OnPointerClickEvent(eventData);
            }

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null) return;
            if (pointerEventData.button == PointerEventData.InputButton.Middle ||
                (pointerEventData.button == PointerEventData.InputButton.Left && pointerEventData.Pointer is ZMouse))
            {
                if (_grabbableParent != null)
                {
                    _grabbableParent.OnBeginDrag(eventData);
                }
                return;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null) return;
            if (pointerEventData.button == PointerEventData.InputButton.Middle ||
                (pointerEventData.button == PointerEventData.InputButton.Left && pointerEventData.Pointer is ZMouse))
            {
                if (_grabbableParent != null)
                {
                    _grabbableParent.OnDrag(eventData);
                }
                return;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null) return;
            if (pointerEventData.button == PointerEventData.InputButton.Middle ||
                (pointerEventData.button == PointerEventData.InputButton.Left && pointerEventData.Pointer is ZMouse))
            {
                if (_grabbableParent != null)
                {
                    _grabbableParent.OnEndDrag(eventData);
                }
                return;
            }
        }

        //#endregion

        //#region Private Members
        private Grabbable _grabbableParent;

        private void Start()
        {
            _grabbableParent = GetComponentInParent<Grabbable>();
        }

        //#endregion
    }
}