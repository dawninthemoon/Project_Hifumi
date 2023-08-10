using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CustomPhysics {
    public enum ColliderLayerMask {
        Default
    }
    public abstract class CustomCollider : MonoBehaviour, IQuadTreeObject {
        [SerializeField] ColliderLayerMask _colliderLayer = ColliderLayerMask.Default;
        public ColliderLayerMask Layer { 
            get { return _colliderLayer; }
            set {
                if (_colliderLayer == value) return;
                _colliderLayer = value;
                InitalizeLayerMask();
            }
        }
        public string Tag { get; set; }
        private readonly UnityEvent _onCollisionEvent = new UnityEvent();
        public UnityEvent OnCollisionEvent {
            get { return _onCollisionEvent; }
        }
        int _layerMask;
        [SerializeField] protected Color _gizmoColor = Color.red;

        protected virtual void Start() {
            CollisionManager.Instance.AddCollider(this);
            InitalizeLayerMask();
        }

        void InitalizeLayerMask() {
            switch (_colliderLayer) {
            case ColliderLayerMask.Default:
                _layerMask = 1;
                break;
            }
        }

        void AddBitMask(ColliderLayerMask targetMask) {
            _layerMask |= (1 << (int)targetMask);
        }
        void RemoveBitMask(ColliderLayerMask targetMask) {
            _layerMask &= ~(1 << (int)targetMask);
        }
        public bool CannotCollision(ColliderLayerMask other) {
            return (_layerMask & (1 << (int)other)) == 0;
        }
        public abstract bool IsCollision(CustomCollider collider);
        public void OnCollision(CustomCollider collider) {
            OnCollisionEvent.Invoke();
        }
        public abstract Rectangle GetBounds();
    }
}