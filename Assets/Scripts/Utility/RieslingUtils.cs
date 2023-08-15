using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace RieslingUtils {
    public static class ExVector {
        public static Vector3 Jiggle(this Vector3 origin, float maxAmount) {
            float xAmount = Random.Range(-maxAmount, maxAmount);
            float yAmount = Random.Range(-maxAmount, maxAmount);

            Vector3 newVector = origin + new Vector3(xAmount, yAmount);
            return newVector;
        }

        public static Vector3 ChangeXPos(this Vector3 origin, float xValue) {
            Vector3 newVector = new Vector3(xValue, origin.y, origin.z);
            return newVector;
        }

        public static Vector3 ChangeYPos(this Vector3 origin, float yValue) {
            Vector3 newVector = new Vector3(origin.x, yValue, origin.z);
            return newVector;
        }

        public static Vector3 ChangeZPos(this Vector3 origin, float zValue) {
            Vector3 newVector = new Vector3(origin.x, origin.y, zValue);
            return newVector;
        }
    }

    public static class Bezier {
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
    }

    public static class ExMath {
        public static float GetDegreeBetween(Vector2 from, Vector2 to) {
            Vector2 diff = to - from;
            float radian = Mathf.Atan2(diff.y, diff.x);
            return radian * Mathf.Rad2Deg;
        }

        public static Vector2 GetRotatedPos(Vector2 origin, Vector2 point, float radian) {
            float rotatedX = (Mathf.Cos(radian) * (point.x - origin.x)) + (Mathf.Sin(radian) * (point.y - origin.y)) + origin.x;
            float rotatedY = (Mathf.Cos(radian) * (point.y - origin.y)) - (Mathf.Sin(radian) * (point.x - origin.x)) + origin.y;
            return new Vector2(rotatedX, rotatedY);
        }

        public static int AddBitMask(this int mask, int targetMask) {
            int newmMask = mask | (1 << targetMask);
            return newmMask;
        }

        public static bool BitmaskContains(this int mask, int other) {
            return ((mask & (1 << other)) != 0);
        }
    }

    public static class EnumUtil {
        public static T Parse<T>(string str) {
            T rarity = (T)System.Enum.Parse(typeof(T), str);
            return rarity;
        }
    }

    public static class StringUtils {
        private static readonly string RegexContainsPrefix = "^.*(";
        private static readonly string RegexContainsSuffix = ").*";
        public static bool Contains(string str, string pattern) {
            return Regex.Match(str, RegexContainsPrefix + pattern + RegexContainsSuffix).Success;
        }
    }

    public static class MouseUtils {
        public static Vector3 GetMouseWorldPosition() {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            return worldPosition;
        }

        public static bool IsMouseOverCollider(Collider2D collider) {
            Vector3 mousePosition = GetMouseWorldPosition();
            bool? isOverlaped = Physics2D.OverlapPoint(mousePosition)?.Equals(collider);
            return isOverlaped.HasValue ? isOverlaped.Value : false;
        }
    }

    public static class UnityUtils {
        public static void ToggleGameObject(this GameObject obj) {
            obj.SetActive(!obj.activeSelf);
        }
    }
}