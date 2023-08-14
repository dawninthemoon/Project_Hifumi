#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

namespace CustomPhysics {
    [System.Serializable]
    public struct Rectangle {
        public Vector2 position;
        public float width;
        public float height;
        public float rotation;
        public Rectangle(float x, float y, float width, float height, float rotation = 0f) {
            this.position = new Vector2(x, y);
            this.width = width;
            this.height = height;
            this.rotation = rotation;
        }
        public Rectangle(Vector2 position, float width, float height, float rotation = 0f) {
            this.position = position;
            this.width = width;
            this.height = height;
            this.rotation = rotation;
        }
    }

    public class RectCollider : CustomCollider {
        [SerializeField] Rectangle _rect;
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
            Rectangle newRectangle = _rect;
            newRectangle.position += (Vector2)transform.position;
            newRectangle.width *= Mathf.Abs(transform.localScale.x);
            newRectangle.height *= Mathf.Abs(transform.localScale.y);
            newRectangle.rotation += transform.localRotation.eulerAngles.z;
            return newRectangle;
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
            return CollisionManager.Instance.IsCollision(this, other);
        }
        public bool IsCollision(CircleCollider other) {
            return CollisionManager.Instance.IsCollision(this, other);
        }
        public Vector2 GetWidthVector() {
            Vector2 ret;
            ret.x = _rect.width * Mathf.Abs(transform.localScale.x) * Mathf.Cos(_rect.rotation) * 0.5f;
            ret.y = -_rect.width * Mathf.Abs(transform.localScale.x) * Mathf.Sin(_rect.rotation) * 0.5f;
            return ret;
        }
        public Vector2 GetHeightVector() {
            Vector2 ret;
            ret.x = _rect.height * transform.localScale.y * Mathf.Cos(_rect.rotation) * 0.5f;
            ret.y = -_rect.height * transform.localScale.y * Mathf.Sin(_rect.rotation) * 0.5f;
            return ret;
        }
        public Vector2 GetClosestPoint(Vector2 point) {
            Rectangle bounds = GetBounds();
            float radian = bounds.rotation * Mathf.Deg2Rad;
            Vector2 rotatedPoint = ExMath.GetRotatedPos(bounds.position, point, radian);
            
            Vector2 rotatedClosest = Vector2.zero;
            if (rotatedPoint.x > bounds.position.x + bounds.width * 0.5f)
                rotatedClosest.x = bounds.position.x + bounds.width * 0.5f;
            else if (rotatedPoint.x < bounds.position.x - bounds.width * 0.5f)
                rotatedClosest.x = bounds.position.x - bounds.width * 0.5f;
            else
                rotatedClosest.x = rotatedPoint.x;

            if (rotatedPoint.y > bounds.position.y + bounds.height * 0.5f)
                rotatedClosest.y = bounds.position.y + bounds.height * 0.5f;
            else if (rotatedPoint.y < bounds.position.y - bounds.height * 0.5f)
                rotatedClosest.y = bounds.position.y - bounds.height * 0.5f;
            else
                rotatedClosest.y = rotatedPoint.y;

            Vector2 closestPosition = ExMath.GetRotatedPos(bounds.position, rotatedClosest, -radian);
            return closestPosition;
        }

        void OnDrawGizmos() {
            Color prevGizmoColor = Gizmos.color;

            Vector2 center = (Vector2)transform.position + _rect.position;
            float radian = (transform.eulerAngles.z + _rect.rotation) * Mathf.Deg2Rad;

            float width = _rect.width * Mathf.Abs(transform.localScale.x);
            Vector2 dir1 = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
            dir1 = dir1 * width * 0.5f;

            float height = _rect.height * Mathf.Abs(transform.localScale.y);
            Vector2 dir2 = new Vector2(Mathf.Cos(radian + Mathf.PI * 0.5f), Mathf.Sin(radian + Mathf.PI * 0.5f)).normalized;
            dir2 = dir2 * height * 0.5f;

            Gizmos.color = _gizmoColor;
            Gizmos.DrawLine(center + dir1 + dir2, center - dir1 + dir2);
            Gizmos.DrawLine(center - dir1 + dir2, center - dir1 - dir2);
            Gizmos.DrawLine(center - dir1 - dir2, center + dir1 - dir2);
            Gizmos.DrawLine(center + dir1 - dir2, center + dir1 + dir2);

            Gizmos.color = prevGizmoColor;
        }
    }
}