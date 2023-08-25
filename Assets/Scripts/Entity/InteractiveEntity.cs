using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

[RequireComponent(typeof(Collider2D))]
public class InteractiveEntity : MonoBehaviour {
    public enum Type {
        Entity,
        UI,
        Reward
    }

    [SerializeField] private Type _type;
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
    public static Dictionary<InteractiveEntity.Type, bool> InteractiveDictionary { get; set; } = new Dictionary<Type, bool>();

    public static void SetInteractive(InteractiveEntity.Type type, bool interactive) {
        if (!InteractiveDictionary.TryGetValue(type, out bool isInteractive)) {
            InteractiveDictionary.Add(type, interactive);
        }
        else {
            InteractiveDictionary[type] = interactive;
        }
    }

    private void OnMouseOver() {
        if (CheckIsInteractive()) {
            _onMouseOverEvent?.Invoke();
        }
    }

    private void OnMouseDown() {
        if (CheckIsInteractive()) {
            _onMouseDownEvent?.Invoke();
        }
    }

    private void OnMouseDrag() {
        if (CheckIsInteractive()) {
            _onMouseDragEvent?.Invoke();
        }
    }

    private void OnMouseUp() {
        if (CheckIsInteractive()) {
            _onMouseUpEvent?.Invoke();
        }
    }

    private void OnMouseExit() {
        if (CheckIsInteractive()) {
            _onMouseExitEvent?.Invoke();
        }
    }

    private bool CheckIsInteractive() {
        if (!InteractiveDictionary.TryGetValue(_type, out bool isInteractive)) {
            isInteractive = true;
            InteractiveDictionary.Add(_type, isInteractive);
        }
        return isInteractive;
    }
}
