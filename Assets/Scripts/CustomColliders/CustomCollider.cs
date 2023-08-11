using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CustomPhysics {
    public enum ColliderLayerMask {
        None,
        AllyBody,
        EnemyBody,
        AllyBodyCheck,
        EnemyBodyCheck,
        Obstacle,
    }

    public abstract class CustomCollider : MonoBehaviour, IQuadTreeObject {
        [SerializeField] ColliderLayerMask _colliderLayer = ColliderLayerMask.None;
        public ColliderLayerMask Layer { 
            get { 
                return _colliderLayer;
            }
            set {
                if (_colliderLayer.Equals(value)) return;
                _colliderLayer = value;
                InitalizeLayerMask();
            }
        }
        public string Tag { get; set; }
        private readonly UnityEvent<CustomCollider, CustomCollider> _onCollisionEvent = new UnityEvent<CustomCollider, CustomCollider>();
        public UnityEvent<CustomCollider, CustomCollider> OnCollisionEvent {
            get { return _onCollisionEvent; }
        }
        private int _layerMask;
        [SerializeField] protected Color _gizmoColor = Color.red;

        protected virtual void Awake() {
            InitalizeLayerMask();
        }

        protected virtual void OnEnable() {
            CollisionManager.Instance.AddCollider(this);
        }

        protected virtual void OnDisable() {
            CollisionManager.Instance.RemoveCollider(this);
        }

        void InitalizeLayerMask() {
            _layerMask = 0;
            switch (_colliderLayer) {
            case ColliderLayerMask.None:
                _layerMask = 0;
                break;
            case ColliderLayerMask.AllyBody:
            case ColliderLayerMask.EnemyBody:
                AddBitMask(ColliderLayerMask.AllyBody);
                AddBitMask(ColliderLayerMask.EnemyBody);
                AddBitMask(ColliderLayerMask.Obstacle);
                break;
            case ColliderLayerMask.AllyBodyCheck:
                AddBitMask(ColliderLayerMask.AllyBody);
                break;
            case ColliderLayerMask.EnemyBodyCheck:
                AddBitMask(ColliderLayerMask.EnemyBody);
                break;
            case ColliderLayerMask.Obstacle:
                AddBitMask(ColliderLayerMask.AllyBody);
                AddBitMask(ColliderLayerMask.EnemyBody);
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
            OnCollisionEvent?.Invoke(this, collider);
        }
        public abstract Rectangle GetBounds();
    }
}