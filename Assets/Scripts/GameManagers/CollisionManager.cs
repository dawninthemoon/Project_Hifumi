using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        static Vector2[] _cachedVectorArr;
        List<CustomCollider> _colliders;
        QuadTree<CustomCollider> _quadTree;
        List<CustomCollider> _adjustObjectsList;

        private void Awake() {
            Initalize();
        }
        
        public void Initalize() {
            _quadTree = new QuadTree<CustomCollider>(0, new Rectangle(-650f, -650f, 650f, 650f));
            _cachedVectorArr = new Vector2[4];
            _colliders = new List<CustomCollider>();
            _adjustObjectsList = new List<CustomCollider>();
        }

        public void Update() {
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
                    if (_colliders[i].CannotCollision(_adjustObjectsList[j].Layer)) continue;
                    if (_colliders[i].IsCollision(_adjustObjectsList[j])) {
                        _colliders[i].OnCollision(_adjustObjectsList[j]);
                    }
                }
            }
        }
        public void AddCollider(CustomCollider collider) {
            _colliders.Add(collider);
        }
        public void RemoveCollider(CustomCollider collider) {
            _colliders.Remove(collider);
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
        bool CheckProjection(Polygon p1, Polygon p2) {
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
            Circle circle1 = c1.CircleShape;
            Circle circle2 = c2.CircleShape;

            float xPow = (circle1.center.x - circle2.center.x) * (circle1.center.x - circle2.center.x);
            float yPow = (circle1.center.y - circle2.center.y) * (circle1.center.y - circle2.center.y);
            float radiusPow = (circle1.radius + circle2.radius) * (circle1.radius + circle2.radius);

            return (xPow + yPow) < radiusPow;
        }
        public bool IsCollision(RectCollider c1, CircleCollider c2) {
            Rectangle rect = c1.GetBounds();
            Circle circle = c2.CircleShape;

            float max = -Mathf.Infinity;
            Vector2 boxToCircle = circle.center - rect.position;
            float boxToCircleMagnitude = boxToCircle.magnitude;
            Vector2 normalizedBoxToCircle = boxToCircle.normalized;

            _cachedVectorArr[0] = rect.GetP00();
            _cachedVectorArr[1] = rect.GetP01();
            _cachedVectorArr[2] = rect.GetP11();
            _cachedVectorArr[3] = rect.GetP10();

            for (int i = 0; i < 4; ++i) {
                float currentProjection = Vector2.Dot(_cachedVectorArr[i], normalizedBoxToCircle);
                max = Mathf.Max(max, currentProjection);
            }

            return !((boxToCircleMagnitude - max - circle.radius > 0f) && (boxToCircleMagnitude > 0f));
        }
        public bool IsCollision(RectCollider c1, Circle circle) {
            Rectangle rect = c1.GetBounds();

            float max = -Mathf.Infinity;
            Vector2 boxToCircle = circle.center - rect.position;
            float boxToCircleMagnitude = boxToCircle.magnitude;
            Vector2 normalizedBoxToCircle = boxToCircle.normalized;

            _cachedVectorArr[0] = rect.GetP00();
            _cachedVectorArr[1] = rect.GetP01();
            _cachedVectorArr[2] = rect.GetP11();
            _cachedVectorArr[3] = rect.GetP10();

            for (int i = 0; i < 4; ++i) {
                float currentProjection = Vector2.Dot(_cachedVectorArr[i], normalizedBoxToCircle);
                max = Mathf.Max(max, currentProjection);
            }

            return !((boxToCircleMagnitude - max - circle.radius > 0f) && (boxToCircleMagnitude > 0f));
        }
        Vector2 GetUnitVector(Vector2 vec) {
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
        void UpdateRaycastInfo(Vector2 origin, Vector2 p1, Vector2 p2, Vector2 c, ref RaycastInfo info) {
            float dist = Vector2.Distance(origin, c);
            if (dist < info.distance) {
                float ibl = 1f / Vector2.Distance(p1, p2);
                float nx = -(p2.y - p1.y) * ibl;
                float ny =  (p2.x - p1.x) * ibl;
                info.normal = new Vector2(nx, ny);
                info.distance = dist;
            }
        }
        bool RayLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, ref Vector2 c) {
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
        bool IsInRect(Vector2 a, Vector2 b, Vector2 c) {
            float minX = Mathf.Min(b.x, c.x), maxX = Mathf.Max(b.x, c.x);
            float minY = Mathf.Min(b.y, c.y), maxY = Mathf.Max(b.y, c.y);
            
            if	(minX == maxX) return (minY <= a.y && a.y <= maxY);
            if	(minY == maxY) return (minX <= a.x && a.x <= maxX);
            
            return (minX <= a.x + 1e-10 && a.x - 1e-10 <= maxX && minY <= a.y + 1e-10 && a.y - 1e-10 <= maxY) ;		
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