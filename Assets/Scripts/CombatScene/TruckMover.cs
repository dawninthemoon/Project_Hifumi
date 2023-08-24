using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;
using UnityEngine.Events;

public class TruckMover : MonoBehaviour {
    [SerializeField] private UnityEvent _onTruckMoveEnd = null;
    [SerializeField] Collider2D[] _boarders = null;
    [SerializeField] private Truck _truckObject = null;
    [SerializeField] private RectTransform _truckDirectionArrow = null;
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
                _truckDirectionArrow.gameObject.SetActive(false);
                SetTruckMoveConfig();
            }

            _canSetDirection = false;
        }

        if (_startPosition != null && _canSetDirection) {
            _truckDirectionArrow.gameObject.SetActive(true);

            Vector2 start = _startPosition.Value;
            Vector2 end = MouseUtils.GetMouseWorldPosition();
            _truckDirectionArrow.eulerAngles = new Vector3(0f, 0f, ExVector.GetDegree(start, end));
            _truckDirectionArrow.position = start;

            float width = Vector2.Distance(start, end) * 2f;
            _truckDirectionArrow.sizeDelta =_truckDirectionArrow.sizeDelta.ChangeXPos(width);
        }
    }

    private void SetTruckMoveConfig() {
        if (_startPosition.Equals(_endPosition))
            return;

        float degree = ExVector.GetDegree(_startPosition.Value, _endPosition);
        Vector2 direction = (_endPosition - _startPosition.Value).normalized;

        Vector2 startPoint = GetStartPoint(_startPosition.Value, _endPosition, degree);
        startPoint = startPoint - direction * _truckObject.Width * 0.5f;

        Collider2D[] overlapedBoarders = Physics2D.OverlapCircleAll(startPoint, _truckObject.Width);

        foreach (Collider2D boarder in _boarders) {
            foreach (Collider2D overlapedBoarder in overlapedBoarders) {
                if (boarder.Equals(overlapedBoarder)) {
                    boarder.gameObject.SetActive(false);
                    break;
                }
            }
        }

        _truckObject.StartMove(startPoint, direction, degree, OnTruckMoveEnd);
    }
    
    private void OnTruckMoveEnd() {
        foreach (Collider2D boarder in _boarders) {
            boarder.gameObject.SetActive(false);
        }
        _onTruckMoveEnd.Invoke();
    }

    public Vector2 GetStartPoint(Vector2 start, Vector2 end, float degree) {
        var hit = Physics2D.Raycast(start, (start - end).normalized, 1000f, 1 << LayerMask.NameToLayer("Obstacle"));
        return hit.point;
    }
}
