using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class Truck : EntityBase, ITargetable, IResetable {
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _height = 1f;
    [SerializeField] private float _acceleration = 1.2f;
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _knockbackForce = 20f;
    [SerializeField] private float _knockbackMin = 20f;
    public float Width {
        get { return _width; }
    }
    public float Height {
        get { return _height; }
    }
    public Vector3 Position {
        get { return transform.position; }
    }

    private Vector3 _direction;
    private float _currentSpeed;
    private int _collisionCount;
    public bool MoveProgressEnd { get; private set; }

    public void StartMove(Vector3 position, Vector3 direction, float angle, System.Action onTruckmoveEnd) {
        transform.eulerAngles = new Vector3(0f, 0f, angle);
        transform.position = position;
        _collisionCount = 0;
        _acceleration = Mathf.Abs(_acceleration);
        _direction = direction;
        
        StopAllCoroutines();
        StartCoroutine(MoveProgress(direction, onTruckmoveEnd));
    }

    public void Reset() {
        MoveProgressEnd = false;
    }

    private IEnumerator MoveProgress(Vector3 direction, System.Action onTruckmoveEnd) {
        float initialSpeed = _currentSpeed = _speed;
        float acc = _acceleration;
        float timeAgo = 0f;
        while (_currentSpeed > 0f) {
            _currentSpeed = initialSpeed + acc * timeAgo;
            if (_collisionCount > 0) {
                acc = -Mathf.Abs(_acceleration * 2f);
            }

            Vector3 velocity = direction * _currentSpeed;
            transform.position += velocity * Time.deltaTime;

            timeAgo += Time.deltaTime;
            
            yield return null;
        }

        MoveProgressEnd = true;
        transform.localRotation = Quaternion.identity;
        onTruckmoveEnd();
        StartCoroutine(MoveCamera(transform.position));
    }

    private IEnumerator MoveCamera(Vector3 targetPosition) {
        Transform cameraTransform = Camera.main.transform;
        float duration = 0.5f;
        float timeAgo = 0f;

        Vector3 startPosition = cameraTransform.position;
        while (timeAgo < duration) {
            timeAgo += Time.deltaTime;

            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, timeAgo / duration).ChangeZPos(-10f);

            yield return null;
        }

        CombatSceneHandler.SetMapView(targetPosition);
    }

    private void Awake() {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (MoveProgressEnd) {
            return;
        }

        ++_collisionCount;
        if (other.gameObject.tag.Equals("Enemy")) {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            float speed = _currentSpeed > 0f ? _currentSpeed : _speed / 10f;
            other.GetComponent<HitEffect>().ApplyKnockback(direction, _knockbackMin + speed * _knockbackForce);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (MoveProgressEnd) {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            other.transform.position += direction * _knockbackForce * 200f * Time.deltaTime;
        }
    }
}
