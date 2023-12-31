using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

namespace CustomPhysics {
    public struct RaycastInfo {
        public CustomCollider collider;
        public Vector2 point;
        public Vector2 normal;
        public float distance;
        public RaycastInfo(CustomCollider collider, Vector2 point, Vector2 normal, float distance) {
            this.collider = collider;
            this.point = point;
            this.normal = normal;
            this.distance = distance;
        }
    }

    public class CollisionManager : SingletonWithMonoBehaviour<CollisionManager> {
        private static Vector2[] _cachedVectorArr;
        private List<CustomCollider> _colliders;
        private QuadTree<CustomCollider> _quadTree;
        private List<CustomCollider> _adjustObjectsList;
        private List<CustomCollider> _cachedListForOverlapCircle;
        private List<CustomCollider> _cachedOverlapedColliderList;
        private List<UICollider> _uiColliders;

        private void Awake() {
            Initalize();
        }
        
        public void Initalize() {
            _quadTree = new QuadTree<CustomCollider>(0, new Rectangle(-650f, -650f, 650f, 650f));
            _cachedVectorArr = new Vector2[4];
            _colliders = new List<CustomCollider>();
            _adjustObjectsList = new List<CustomCollider>();
            _cachedListForOverlapCircle = new List<CustomCollider>();
            _cachedOverlapedColliderList = new List<CustomCollider>();
            _uiColliders = new List<UICollider>();
        }

        public void Update() {
            CheckUICollisions();

            _quadTree.Clear();

            int numOfColliders = _colliders.Count;
            for (int i = 0; i < numOfColliders; ++i) {
                _quadTree.Insert(_colliders[i]);
            }

            for (int i = 0; i < numOfColliders; ++i) {
                _adjustObjectsList.Clear();
                _quadTree.GetObjects(_adjustObjectsList, _colliders[i].GetBounds());

                int numOfAdjustObjects = _adjustObjectsList.Count;
                for (int j = 0; j < numOfAdjustObjects; ++j) {
                    if (_colliders[i].Equals(_adjustObjectsList[j]) || _colliders[i].CannotCollision(_adjustObjectsList[j].Layer)) continue;
                    if (_colliders[i].IsCollision(_adjustObjectsList[j])) {
                        _colliders[i].OnCollision(_adjustObjectsList[j]);
                    }
                }
            }
        }
        public void AddCollider(CustomCollider collider) {
            _colliders.Add(collider);
        }
        public void AddUICollider(UICollider collider) {
            _uiColliders.Add(collider);
        }
        public void RemoveCollider(CustomCollider collider) {
            _colliders.Remove(collider);
        }
        public void RemoveUICollider(UICollider collider) {
            _uiColliders.Remove(collider);
        }
        public CustomCollider OverlapCircle(Vector2 point, float radius, ColliderLayerMask layerMask) {
            _cachedListForOverlapCircle.Clear();

            Circle collisionArea = new Circle(point, radius);
            _quadTree.GetObjects(_cachedListForOverlapCircle, collisionArea.GetBounds());
            foreach (CustomCollider collider in _cachedListForOverlapCircle) {
                if (!collider.Layer.Equals(layerMask)) continue;
                if (collider is CircleCollider) {
                    if (IsCollision((collider as CircleCollider).CircleShape, collisionArea)) {
                        return collider;
                    }
                }
                else if (collider is RectCollider) {
                    if (IsCollision((collider as RectCollider), collisionArea)) {
                        return collider;
                    }
                }
            }
            return null;
        }
        public CustomCollider[] OverlapCircleAll(Vector2 point, float radius, ColliderLayerMask layerMask) {
            _cachedListForOverlapCircle.Clear();
            _cachedOverlapedColliderList.Clear();

            Circle collisionArea = new Circle(point, radius);
            _quadTree.GetObjects(_cachedListForOverlapCircle, collisionArea.GetBounds());
            foreach (CustomCollider collider in _cachedListForOverlapCircle) {
                if (!collider.Layer.Equals(layerMask)) continue;
                if (collider is CircleCollider) {
                    if (IsCollision((collider as CircleCollider).CircleShape, collisionArea)) {
                        _cachedOverlapedColliderList.Add(collider);
                    }
                }
                else if (collider is RectCollider) {
                    if (IsCollision((collider as RectCollider), collisionArea)) {
                        _cachedOverlapedColliderList.Add(collider);
                    }
                }
            }
            return _cachedOverlapedColliderList.ToArray();
        }
        public bool IsCollision(Polygon p1, Vector2 p1Pos, Polygon p2, Vector2 p2Pos) {
            int p1Length = p1.points.Length;
            int p2Length = p2.points.Length;

            for (int i = 0; i < p1Length; ++i)
                p1.points[i] += p1Pos + p1.offset;
            for (int i = 0; i < p2Length; ++i)
                p2.points[i] += p2Pos + p2.offset;

            bool isCollision = CheckProjection(p1, p2);

            for (int i = 0; i < p1Length; ++i)
                p1.points[i] -= (p1Pos + p1.offset);
            for (int i = 0; i < p2Length; ++i)
                p2.points[i] -= (p2Pos + p2.offset);
            
            return isCollision;
        }
        private bool CheckProjection(Polygon p1, Polygon p2) {
            int p1Length = p1.points.Length;
            int p2Length = p2.points.Length;

            if (p1Length < 3 || p2Length < 3) return false;

            float tmpRadian;
            float p1X1;
            float p1X2;
            float p2X1;
            float p2X2;

            float lastP1X1 = 0f;
            float lastP1X2 = 0f;
            float lastP2X1 = 0f;
            float lastP2X2 = 0f;

            float tmpMaxLen;

            for (int i = 0; i < p1Length ; ++i) {
                int nextIdx = (i + 1) % p1Length;

                if (p1.points[nextIdx].x > p1.points[i].x) {
                    tmpRadian = Mathf.Atan2(
                                    p1.points[nextIdx].y - p1.points[i].y,
                                    p1.points[nextIdx].x - p1.points[i].x);
                }
                else {
                    tmpRadian = Mathf.Atan2(
                                    p1.points[i].y - p1.points[nextIdx].y,
                                    p1.points[i].x - p1.points[nextIdx].x);
                }
                tmpRadian += Mathf.PI / 2f;
                tmpRadian = -tmpRadian;

                tmpMaxLen = 0f;
                lastP1X1 = 0f;
                lastP1X2 = 0f;

                for (int j = 0; j < p1Length ; ++j) {
                    for (int s = (j + 1) % p1Length; s < p1Length; ++s) {
                        p1X1 = p1.points[j].x * Mathf.Cos(tmpRadian) - p1.points[j].y * Mathf.Sin(tmpRadian);
                        p1X2 = p1.points[s].x * Mathf.Cos(tmpRadian) - p1.points[s].y * Mathf.Sin(tmpRadian);

                        if (p1X1 > p1X2) {
                            float tmpChangeValue = p1X1;
                            p1X1 = p1X2;
                            p2X1 = tmpChangeValue;
                        }

                        float tmpLen = p1X2 - p1X1;
                        if (tmpLen > tmpMaxLen) {
                            tmpMaxLen = tmpLen;
                            lastP1X1 = p1X1;
                            lastP1X2 = p1X2;
                        }
                        if (s == 0) break;
                    }
                }

                tmpMaxLen = 0f;
                lastP2X1 = 0f;
                lastP2X2 = 0f;

                for (int j = 0; j < p2Length; ++j) {
                    for (int s = (j + 1) % p2Length; s < p2Length; ++s) {
                        p2X1 = p2.points[j].x * Mathf.Cos(tmpRadian) - p2.points[j].y * Mathf.Sin(tmpRadian);
                        p2X2 = p2.points[s].x * Mathf.Cos(tmpRadian) - p2.points[s].y * Mathf.Sin(tmpRadian);

                        if (p2X1 > p2X2) {
                            float tmpChangeValue = p2X1;
                            p2X1 = p2X2;
                            p2X2 = tmpChangeValue;
                        }

                        float tmpLen = p2X2 - p2X1;
                        if (tmpLen > tmpMaxLen) {
                            tmpMaxLen = tmpLen;
                            lastP2X1 = p2X1;
                            lastP2X2 = p2X2;
                        }

                        if (s == 0) break;
                    }
                }

                if (lastP1X1 > lastP2X2 || lastP1X2 < lastP2X1) return false;
            }
            return true;
        }
        public bool IsCollision(RectCollider c1, RectCollider c2) {
            Rectangle rect1 = c1.GetBounds(), rect2 = c2.GetBounds();
			Vector2 dist = rect1.position - rect2.position;
            _cachedVectorArr[0] = c1.GetHeightVector();
            _cachedVectorArr[1] = c2.GetHeightVector();
            _cachedVectorArr[2] = c1.GetWidthVector();
            _cachedVectorArr[3] = c2.GetWidthVector();

            Vector2 unit;
            for (int i = 0; i < 4; i++) {
                double sum = 0;
                unit = GetUnitVector(_cachedVectorArr[i]);
                for (int j = 0; j < 4; j++) {
                    sum += Mathf.Abs(_cachedVectorArr[j].x * unit.x + _cachedVectorArr[j].y * unit.y);
                }
                if (Mathf.Abs(dist.x * unit.x + dist.y * unit.y) > sum) {
                    return false;
                }
            }
            return true;
        }
        public bool IsCollision(CircleCollider c1, CircleCollider c2) {
            return IsCollision(c1.CircleShape, c2.CircleShape);
        }
        public bool IsCollision(Circle circle1, Circle circle2) {
            float xPow = (circle1.center.x - circle2.center.x) * (circle1.center.x - circle2.center.x);
            float yPow = (circle1.center.y - circle2.center.y) * (circle1.center.y - circle2.center.y);
            float radiusPow = (circle1.radius + circle2.radius) * (circle1.radius + circle2.radius);

            return (xPow + yPow) < radiusPow;
        }
        public bool IsCollision(RectCollider c1, CircleCollider c2) {
            Circle circle = c2.CircleShape;
            return IsCollision(c1, circle);
        }

        public bool IsCollision(RectCollider c1, Circle circle) {
            Rectangle rect = c1.GetBounds();
            circle.center = ExMath.GetRotatedPos(rect.position, circle.center, rect.rotation * Mathf.Deg2Rad);

            Vector2 closestPos;

            if (circle.center.x < rect.position.x - rect.width * 0.5f)
                closestPos.x = rect.position.x - rect.width * 0.5f;
            else if (circle.center.x > rect.position.x + rect.width * 0.5f)
                closestPos.x = rect.position.x + rect.width * 0.5f;
            else
                closestPos.x = circle.center.x;
            
            if (circle.center.y < rect.position.y - rect.height * 0.5f)
                closestPos.y = rect.position.y - rect.height * 0.5f;
            else if (circle.center.y > rect.position.y + rect.height * 0.5f)
                closestPos.y = rect.position.y + rect.height * 0.5f;
            else
                closestPos.y = circle.center.y;

            float distX = Mathf.Abs(circle.center.x - closestPos.x);
            float distY = Mathf.Abs(circle.center.y - closestPos.y);
            float distanceSqr = distX * distX + distY * distY;

            return distanceSqr < circle.radius * circle.radius;
        }
        
        private Vector2 GetUnitVector(Vector2 vec) {
            Vector2 ret;
            float size = Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y);
            ret.x = vec.x / size;
            ret.y = vec.y / size;
            return ret;
        }
        public bool Raycast(Vector2 origin, Vector2 dir, float length, out RaycastInfo info) {
            int numOfColliders = _colliders.Count;
            for (int i = 0; i < numOfColliders; ++i) {
                if (_colliders[i] is PolygonCollider) {
                    if (Raycast(origin, dir, _colliders[i] as PolygonCollider, out info)) {
                        if (length < info.distance) continue;
                        return true;
                    }
                }
                else if (_colliders[i] is RectCollider) {
                    Rectangle rect = (_colliders[i] as RectCollider).GetBounds();
                    PolygonCollider pc = new GameObject().AddComponent<PolygonCollider>();
                    Vector2[] points = new Vector2[4] { 
                        new Vector2(rect.position.x - rect.width / 2, rect.position.x + rect.height / 2),
                        new Vector2(rect.position.x + rect.width / 2, rect.position.y + rect.height / 2),
                        new Vector2(rect.position.x + rect.width / 2, rect.position.y - rect.height / 2),
                        new Vector2(rect.position.x - rect.width / 2, rect.position.y - rect.height / 2)
                    };
                    pc.Initalize(points);
                    if (Raycast(origin, dir, pc, out info)) {
                        Destroy(pc.gameObject);
                        if (length < info.distance) continue;
                        return true;
                    }
                    else {
                        Destroy(pc.gameObject);
                    }
                }
                else if (_colliders[i] is CircleCollider) {
                    if (Raycast(origin, dir * length, _colliders[i] as CircleCollider, out info)) {
                        return true;
                    }
                }
            }
            info = new RaycastInfo();
            return false;
        }

        public bool Raycast(Vector2 origin, Vector2 dir, CircleCollider collider, out RaycastInfo info) {
            Circle circle = collider.CircleShape;
            Vector2 f = origin - circle.center;

            float a = Vector2.Dot(dir, dir);
            float b = Vector2.Dot(f, dir) * 2f;
            float c = Vector2.Dot(f, f) - circle.radius * circle.radius;

            float discriminant = b*b-4*a*c;
            if (discriminant < 0) {
                info = new RaycastInfo();
            }
            else {
                discriminant = Mathf.Sqrt(discriminant);

                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                if (t1 >= 0 && t1 <= 1) {
                    info = new RaycastInfo();
                    info.collider = collider;
                    return true;
                }

                if (t2 >= 0 && t2 <= 1) {
                    info = new RaycastInfo();
                    info.collider = collider;
                    return true;
                }
            }

            info = new RaycastInfo();
            return false;
        }

        public bool Raycast(Vector2 origin, Vector2 dir, PolygonCollider collider, out RaycastInfo info) {
            var polygon = collider.GetPolygon();
            var points = polygon.points;
            int length = points.Length;

            info = new RaycastInfo();
            if (length < 3) return false;

            Vector2 colliderPos = collider.transform.position;
            info = new RaycastInfo(collider, Vector2.zero, Vector2.zero, Mathf.Infinity);
            Vector2 c = Vector2.zero;

            for (int i = 0; i < length; ++i) {
                Vector2 p1 = points[i] + colliderPos + polygon.offset;
                Vector2 p2 = points[(i + 1) % length] + colliderPos + polygon.offset;

                bool intersects = RayLineIntersection(origin, origin + dir, p1, p2, ref c);
                if (intersects) {
                    UpdateRaycastInfo(origin, p1, p2, c, ref info);
                }
            }

            return (info.distance < Mathf.Infinity);
        }
        private void UpdateRaycastInfo(Vector2 origin, Vector2 p1, Vector2 p2, Vector2 c, ref RaycastInfo info) {
            float dist = Vector2.Distance(origin, c);
            if (dist < info.distance) {
                float ibl = 1f / Vector2.Distance(p1, p2);
                float nx = -(p2.y - p1.y) * ibl;
                float ny =  (p2.x - p1.x) * ibl;
                info.normal = new Vector2(nx, ny);
                info.distance = dist;
            }
        }
        private bool RayLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, ref Vector2 c) {
            var dax = (a1.x - a2.x);
            var dbx = (b1.x - b2.x);
            var day = (a1.y - a2.y);
            var dby = (b1.y - b2.y);

            var denominator = dax * dby - day * dbx;
            if (denominator == 0) return false;
            
            var a = (a1.x * a2.y - a1.y * a2.x);
            var b = (b1.x * b2.y - b1.y * b2.x);
            
            var i = c;
            var iDen = 1 / denominator;
            i.x = (a * dbx - dax * b) * iDen;
            i.y = (a * dby - day * b) * iDen;
            c = i;
            
            if (!IsInRect(i, b1, b2)) return false;
            if ((day > 0f && i.y > a1.y) || (day < 0f && i.y < a1.y)) return false;
            if ((dax > 0f && i.x > a1.x) || (dax < 0f && i.x < a1.x)) return false; 
            return true;
	    }
        private bool IsInRect(Vector2 a, Vector2 b, Vector2 c) {
            float minX = Mathf.Min(b.x, c.x), maxX = Mathf.Max(b.x, c.x);
            float minY = Mathf.Min(b.y, c.y), maxY = Mathf.Max(b.y, c.y);
            
            if	(minX == maxX) return (minY <= a.y && a.y <= maxY);
            if	(minY == maxY) return (minX <= a.x && a.x <= maxX);
            
            return (minX <= a.x + 1e-10 && a.x - 1e-10 <= maxX && minY <= a.y + 1e-10 && a.y - 1e-10 <= maxY) ;		
        }
        private void CheckUICollisions() {
            Vector2 mousePosition = RieslingUtils.MouseUtils.GetMouseWorldPosition();
            Circle mouseBounds = new Circle(mousePosition, 1f);
            for (int i = 0; i < _uiColliders.Count; ++i) {
                UICollider uiCollider = _uiColliders[i];
                if (IsCollision(uiCollider, mouseBounds)) {
                    uiCollider.OnMouseOver?.Invoke();
                    if (Input.GetMouseButtonDown(0)) {
                        uiCollider.OnMouseDown?.Invoke();
                    }
                    if (Input.GetMouseButtonUp(0)) {
                        uiCollider.OnMouseUp?.Invoke();
                    }
                }
                else if (uiCollider.MouseOverlapedAtLastFrame) {
                    uiCollider.OnMouseExit?.Invoke();
                }
            }
        }
/*
        static bool Contains(Rectangle rect, Vector2 point) {
            return 
                (point.x >= rect.x - rect.width * 0.5f) &&
                (point.y >= rect.y - rect.height * 0.5f) &&
                (point.x <= rect.x + rect.width * 0.5f) &&
                (point.y <= rect.y + rect.height * 0.5f);
        }
        */
    }
}