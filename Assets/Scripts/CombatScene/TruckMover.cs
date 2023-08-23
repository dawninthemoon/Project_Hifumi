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
                SetTruckMoveConfig();
            }

            //_canSetDirection = false;
        }

        if (_startPosition != null && _canSetDirection) {
            Debug.DrawLine(_startPosition.Value, MouseUtils.GetMouseWorldPosition(), Color.green);
        }
    }

    private void SetTruckMoveConfig() {
        float degree = GetAngle(_startPosition.Value, _endPosition);
        _truckObject.transform.eulerAngles = new Vector3(0f, 0f, degree);
        _truckObject.transform.position = GetStartPoint(_startPosition.Value, _endPosition, degree);
    }

    public float GetAngle(Vector2 start, Vector2 end) {
        Vector2 diff = end - start;
        float radian = Mathf.Atan2(diff.y, diff.x);
        return radian * Mathf.Rad2Deg;
    }

    public Vector2 GetStartPoint(Vector2 start, Vector2 end, float degree) {
        float slope = (end - start).y / (end - start).x;
        float yIntercept = start.y - slope * start.x;

        float xPos = 0f;
        float yPos = 0f;

        if (degree < 0f) {
            degree = 360f + degree;
        }

        if (degree >= 315f || degree < 45f) {
            xPos = GameTest.StageMinSize.x;
            yPos = slope * xPos + yIntercept;
        }
        else if (degree < 135f) {
            yPos = GameTest.StageMinSize.y;
            xPos = (yPos - yIntercept) / slope;
        }
        else if (degree < 225f) {
            xPos = GameTest.StageMaxSize.x;
            yPos = slope * xPos + yIntercept;
        }
        else {
            yPos = GameTest.StageMaxSize.y;
            xPos = (yPos - yIntercept) / slope;
        }

        return new Vector2(xPos, yPos);
    }
}
