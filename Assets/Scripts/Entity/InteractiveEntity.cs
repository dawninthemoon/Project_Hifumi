using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

[RequireComponent(typeof(Collider2D))]
public class InteractiveEntity : MonoBehaviour {
    [SerializeField] private UnityEvent _onMouseOverEvent;
    [SerializeField] private UnityEvent _onMouseDownEvent;
    [SerializeField] private UnityEvent _onMouseUpEvent;
    [SerializeField] private UnityEvent _onMouseExitEvent;
    public UnityEvent OnMouseOverEvent { get { return _onMouseOverEvent; } }
    public UnityEvent OnMouseDownEvent { get { return _onMouseDownEvent; } }
    public UnityEvent OnMouseUpEvent { get { return _onMouseUpEvent; } }
    public UnityEvent OnMouseExitEvent { get { return _onMouseUpEvent; } }

    private void Awake() {
        _onMouseOverEvent = new UnityEvent();
        _onMouseDownEvent = new UnityEvent();
        _onMouseUpEvent = new UnityEvent();
        _onMouseExitEvent = new UnityEvent();
    }

    private void OnMouseOver() {
        _onMouseOverEvent?.Invoke();
    }

    private void OnMouseDown() {
        _onMouseDownEvent?.Invoke();
    }

    private void OnMouseUp() {
        _onMouseUpEvent?.Invoke();
    }

    private void OnMouseExit() {
        _onMouseExitEvent?.Invoke();
    }
}
