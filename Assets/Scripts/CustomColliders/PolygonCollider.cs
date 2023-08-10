using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics {
    [System.Serializable]
    public class Polygon {
        public Vector2[] points;
        public Vector2 offset;
        public Rectangle Bounds {
            get;
            private set;
        }

        public void CalculateMinMaxBounds() {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (Vector2 point in points) {
                if (point.x < minX) { minX = point.x; }
                if (point.y < minY) { minY = point.y; }
                if (point.x > maxX) { maxX = point.x; }
                if (point.y > maxY) { maxY = point.y; }
            }

            float width = maxX - minX;
            float height = maxY - minY;

            Bounds = new Rectangle(minX + width * 0.5f, minY, width, height);
        }
    }

    public class PolygonCollider : CustomCollider {
        [SerializeField] Polygon _polygon = null;
        public void Initalize(Vector2[] points) {
            if (_polygon == null)
                _polygon = new Polygon();

            _polygon.points = points.Clone() as Vector2[];
            _polygon.offset = Vector2.zero;
            _polygon.CalculateMinMaxBounds();
        }
        public Polygon GetPolygon() => _polygon;
        public override bool IsCollision(CustomCollider other) {
            return IsCollision(other);
        }
        public bool IsCollision(PolygonCollider other) {
            return CollisionManager.Instance.IsCollision(_polygon, transform.position, other.GetPolygon(), other.transform.position);
        }

        public bool IsCollision(CircleCollider other) => false;
        public bool IsCollision(RectCollider other) => false;
        public override Rectangle GetBounds() {
            return _polygon.Bounds;
        }

        void OnDrawGizmos() {
            if (_polygon == null) return;
            var points = _polygon.points;
            int pLength = points.Length;
            if (pLength < 2) return;

            Gizmos.color = _gizmoColor;
            Vector2 cur = transform.position;
            for (int i = 0; i < pLength; ++i) {
                int nextIdx = (i + 1) % pLength;
                Gizmos.DrawLine(points[i] + _polygon.offset + cur, points[nextIdx] + _polygon.offset + cur);
            }
        }
    }
}