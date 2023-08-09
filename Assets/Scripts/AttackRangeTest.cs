using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeTest : MonoBehaviour {
    private Vector2 _size;
    private float _radian;
    private float _lifeTime;
    public void Initialize(Vector2 position, Vector2 size, float radian, float duration) {
        _size = size;
        _lifeTime = duration;
        _radian = radian;
        transform.position = position;
    }

    private void Update() {
        DrawRect(transform.position, _size, _radian, Color.white);
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0f) {
            Destroy(gameObject);
        }
    }

    private void DrawRect(Vector2 center, Vector2 size, float radian, Color color) {
        Vector2 dir1 = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
        dir1 = dir1 * size.x;

        Vector2 dir2 = new Vector2(Mathf.Cos(radian + Mathf.PI * 0.5f), Mathf.Sin(radian + Mathf.PI * 0.5f)).normalized;
        dir2 = dir2 * size.y;

        Debug.DrawLine(center + dir1 + dir2, center - dir1 + dir2, color);
        Debug.DrawLine(center - dir1 + dir2, center - dir1 - dir2, color);
        Debug.DrawLine(center - dir1 - dir2, center + dir1 - dir2, color);
        Debug.DrawLine(center + dir1 - dir2, center + dir1 + dir2, color);
    }
}
