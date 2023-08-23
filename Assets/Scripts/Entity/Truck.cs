using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class Truck : EntityBase, ITargetable {
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _height = 1f;
    [SerializeField] private float _acceleration = 1.2f;
    [SerializeField] private float _speed = 20f;
    public float Width {
        get { return _width; }
    }
    public float Height {
        get { return _height; }
    }
    public Vector3 Position {
        get { return transform.position; }
    }
    public List<Vector2> GetScentTrail() {
        return new List<Vector2>();
    }

    private bool _collisionWithEnemy;
    private Vector3 _direction;
    private float _currentSpeed;

    public void StartMove(Vector3 position, Vector3 direction, float angle) {
        transform.eulerAngles = new Vector3(0f, 0f, angle);
        transform.position = position;
        _collisionWithEnemy = false;
        _acceleration = Mathf.Abs(_acceleration);
        _direction = direction;
        
        StopAllCoroutines();
        StartCoroutine(MoveProgress(direction));
    }

    private IEnumerator MoveProgress(Vector3 direction) {
        float initialSpeed = _currentSpeed = _speed;
        float timeAgo = 0f;
        while (_currentSpeed > 0f) {
            _currentSpeed = initialSpeed + _acceleration * timeAgo;
            if (_collisionWithEnemy && _acceleration > 0f) {
                _acceleration = -_acceleration;
            }

            Vector3 velocity = direction * _currentSpeed;
            transform.position += velocity * Time.deltaTime;

            timeAgo += Time.deltaTime;

            yield return null;
        }

        GameTest.SetMapView(transform.position);
    }

    private void LateUpdate() {
        if (_collisionWithEnemy) {
            Camera.main.transform.position = transform.position.ChangeZPos(-10f);
        }
    }

    private void Awake() {
        
    }
}
