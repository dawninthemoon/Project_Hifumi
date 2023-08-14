#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics {
    [System.Serializable]
    public struct Circle {
        public Vector2 center;
        public float radius;
        public Circle(Vector2 center, float radius) {
            this.center = center;
            this.radius = radius;
        }

        public Rectangle GetBounds() {
            Vector2 pos = center;
            pos.y -= radius;
            Rectangle bounds = new Rectangle(pos, radius * 2f, radius * 2f);
            return bounds;
        }
    }

    public class CircleCollider : CustomCollider {
        [SerializeField] private Circle _circle;
        public Circle CircleShape { 
            get {
                Circle newCircleShape = new Circle((Vector2)transform.position + _circle.center, _circle.radius);
                return newCircleShape; 
            }
        }
        protected override void Awake() {
            base.Awake();
        }
        protected override void OnEnable() {
            base.OnEnable();
        }
        protected override void OnDisable() {
            base.OnDisable();
        }
        public override Rectangle GetBounds() {
            Circle circle = CircleShape;
            Vector2 pos = circle.center;
            pos.y -= circle.radius;
            Rectangle bounds = new Rectangle(pos, circle.radius * 2f, circle.radius * 2f);
            return bounds;
        }
        public override bool IsCollision(CustomCollider other) {
            if (other is CircleCollider) {
                return IsCollision(other as CircleCollider);
            }
            else if (other is RectCollider) {
                return IsCollision(other as RectCollider);
            }
            return false;
        }
        public bool IsCollision(RectCollider other) {
            return CollisionManager.Instance.IsCollision(other, this);
        }
        public bool IsCollision(CircleCollider other) {
            return CollisionManager.Instance.IsCollision(other, this);
        }
        void OnDrawGizmos() {
            Circle circle = CircleShape;
            Vector2 position = circle.center;
            Vector2 cur = position + circle.radius * new Vector2(Mathf.Cos(0f), Mathf.Sin(0f));
            Vector2 prev = cur;
            
            Gizmos.color = _gizmoColor;
            for (float theta = 0.1f; theta < Mathf.PI * 2f; theta += 0.1f) {
                cur = position + circle.radius * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
                Gizmos.DrawLine(cur, prev);
                prev = cur;
            }
            cur = position + circle.radius * new Vector2(Mathf.Cos(0f), Mathf.Sin(0f));
            Gizmos.DrawLine(cur, prev);
        }
    }
}