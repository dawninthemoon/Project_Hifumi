using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CustomPhysics {
    public class UICollider : RectCollider {
        [SerializeField] private UnityEvent _onMouseOver = new UnityEvent();
        [SerializeField] private UnityEvent _onMouseDown = new UnityEvent();
        [SerializeField] private UnityEvent _onMouseUp = new UnityEvent();
        [SerializeField] private UnityEvent _onMouseExit = new UnityEvent();
        public UnityEvent OnMouseOver { get { return _onMouseOver; }}
        public UnityEvent OnMouseDown { get { return _onMouseDown; } }
        public UnityEvent OnMouseUp { get { return _onMouseUp; } }
        public UnityEvent OnMouseExit { get { return _onMouseExit; } }
        public bool MouseOverlapedAtLastFrame { get; private set; }
        protected override void Awake() {
            _onMouseOver.AddListener(() => MouseOverlapedAtLastFrame = true);
            _onMouseExit.AddListener(() => MouseOverlapedAtLastFrame = false);
        }

        protected override void OnEnable() {
            CollisionManager.Instance.AddUICollider(this);
        }

        protected override void OnDisable() {
            CollisionManager.Instance.RemoveUICollider(this);
        }
    }
}
