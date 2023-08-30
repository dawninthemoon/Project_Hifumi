using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCameraMover : MonoBehaviour {
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

    private void LateUpdate() {
        if (_direction.Equals(Vector2.zero) || _target.MoveProgressEnd) {
            return;
        }

        Vector2 curr = _cameraTransform.position;
        Vector2 to = _target.transform.position;

        Vector3 nextPosition = _cameraTransform.position;
        nextPosition.z = -10f;

        if ((_direction.x > 0f) && (to.x - curr.x > 0f)) {
            nextPosition.x = to.x;
        }
        else if ((_direction.x < 0f) && (to.x - curr.x < 0f)) {
            nextPosition.x = to.x;
        }

        if ((_direction.y > 0f) && (to.y - curr.y > 0f)) {
            nextPosition.y = to.y;
        }
        else if ((_direction.y < 0f) && (to.y - curr.y < 0f)) {
            nextPosition.y = to.y;
        }

        _cameraTransform.position = nextPosition;
        CombatSceneHandler.SetMapView(_cameraTransform.position);
    }
}
