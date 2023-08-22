using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

[RequireComponent(typeof(Collider2D))]
public class InteractiveEntity : MonoBehaviour {
    [SerializeField] private UnityEvent _onMouseOverEvent = new UnityEvent();
    [SerializeField] private UnityEvent _onMouseDownEvent = new UnityEvent();
    [SerializeField] private UnityEvent _onMouseDragEvent = new UnityEvent();
    [SerializeField] private UnityEvent _onMouseUpEvent = new UnityEvent();
    [SerializeField] private UnityEvent _onMouseExitEvent = new UnityEvent();
    public UnityEvent OnMouseOverEvent { get { return _onMouseOverEvent; } }
    public UnityEvent OnMouseDownEvent { get { return _onMouseDownEvent; } }
    public UnityEvent OnMouseDragEvent { get { return _onMouseDragEvent; } }
    public UnityEvent OnMouseUpEvent { get { return _onMouseUpEvent; } }
    public UnityEvent OnMouseExitEvent { get { return _onMouseUpEvent; } }
    public static bool IsInteractive = true;

    private void OnMouseOver() {
        if (IsInteractive)
            _onMouseOverEvent?.Invoke();
    }

    private void OnMouseDown() {
        if (IsInteractive)
            _onMouseDownEvent?.Invoke();
    }

    private void OnMouseDrag() {
        if (IsInteractive)
            _onMouseDragEvent?.Invoke();
    }

    private void OnMouseUp() {
        if (IsInteractive)
            _onMouseUpEvent?.Invoke();
    }

    private void OnMouseExit() {
        if (IsInteractive)
            _onMouseExitEvent?.Invoke();
    }
}
