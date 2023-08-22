using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class TruckMover : MonoBehaviour {
    [SerializeField] private Truck _truckObject = null;
    private bool _canSetDirection;
    private Vector2? _startPosition = null;
    private Vector2 _endPosition;

    private void Awake() {
        _canSetDirection = true;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _startPosition = MouseUtils.GetMouseWorldPosition();
        }

        if (Input.GetMouseButtonUp(0)) {
            _endPosition = MouseUtils.GetMouseWorldPosition();
            if (_canSetDirection) {
                //SetTruckMoveConfig();
            }

            //_canSetDirection = false;
        }

        if (_startPosition != null && _canSetDirection) {
            SetTruckMoveConfig();
            Debug.DrawLine(_startPosition.Value, MouseUtils.GetMouseWorldPosition(), Color.green);
        }
    }

    private void SetTruckMoveConfig() {
        _truckObject.transform.position = GetClosestPoint(_startPosition.Value);
    }

    public Vector2 GetClosestPoint(Vector2 selectedPosition) {
        Vector2 position = Vector2.zero;
        float width = 640f;
        float height = 360f;

        Vector2 closestPosition = Vector2.zero;
        if (selectedPosition.x < position.x + width * 0.5f)
            closestPosition.x = position.x + width * 0.5f;
        else if (selectedPosition.x > position.x - width * 0.5f)
            closestPosition.x = position.x - width * 0.5f;
        else
            closestPosition.x = selectedPosition.x;

        if (selectedPosition.y < position.y + height * 0.5f)
            closestPosition.y = position.y + height * 0.5f;
        else if (selectedPosition.y > position.y - height * 0.5f)
            closestPosition.y = position.y - height * 0.5f;
        else
            closestPosition.y = selectedPosition.y;

        return closestPosition;
    }
}
