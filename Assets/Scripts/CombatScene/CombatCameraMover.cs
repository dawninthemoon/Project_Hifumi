using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCameraMover : MonoBehaviour {
    [SerializeField] private float _moveSpeed = 30f;
    private Truck _target;
    private Vector2 _direction = Vector2.zero;
    private Transform _cameraTransform;

    private void Awake() {
        _cameraTransform = Camera.main.transform;
    }

    public void SetCameraDirection(Truck target, Vector2 targetDirection) {
        _target = target;
        _direction = targetDirection;
    }

    private void Update() {
        if (_direction.Equals(Vector2.zero) || _target.MoveProgressEnd) {
            return;
        }

        Vector2 curr = transform.position;
        Vector2 to = _target.transform.position;

        Vector3 moveDirection = Vector3.zero;
        if ((_direction.x > 0f) && (to.x - curr.x > 0f)) {
            moveDirection.x = to.x - curr.x;
        }
        else if ((_direction.x < 0f) && (to.x - curr.x < 0f)) {
            moveDirection.x = to.x - curr.x;
        }

        if ((_direction.y > 0f) && (to.y - curr.y > 0f)) {
            moveDirection.y = (to.y - curr.y);
        }
        else if ((_direction.y < 0f) && (to.y - curr.y < 0f)) {
            moveDirection.y = (to.y - curr.y);
        }

        _cameraTransform.position += moveDirection.normalized * _moveSpeed * Time.deltaTime;
        CombatSceneHandler.SetMapView(_cameraTransform.position);
    }
}
