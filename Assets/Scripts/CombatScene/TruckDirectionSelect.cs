using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;
using UnityEngine.Events;

public class TruckDirectionSelect : MonoBehaviour, IResetable {
    [SerializeField] private UnityEvent _onTruckMoveEnd = null;
    [SerializeField] Collider2D[] _boarders = null;
    [SerializeField] private Truck _truckObject = null;
    [SerializeField] private CombatCameraMover _cameraMover = null;
    [SerializeField] private RectTransform _truckDirectionArrow = null;
    private Vector3 _initialTruckPosition;
    private bool _canSetDirection;
    private Vector2? _startPosition = null;
    private bool _canShootTruck;

    private void Awake() {
        _initialTruckPosition = _truckObject.transform.position;
        Reset();
    }

    public void Reset() {
        _truckObject.transform.position = _initialTruckPosition;
        _canSetDirection = true;
        _canShootTruck = false;
        _startPosition = null;
        _truckObject.Reset();

        for (int i = 0; i < _boarders.Length; ++i) {
            _boarders[i].gameObject.SetActive(true);
        }

        _cameraMover.SetCameraDirection(null, Vector2.zero);
    }

    private void Update() {
        Vector2 curr = MouseUtils.GetMouseWorldPosition();
        if (Input.GetMouseButtonDown(0)) {
            _startPosition = curr;
        }

        if (_startPosition != null && _canSetDirection) {
            Vector2 start = _startPosition.Value;
            float width = Vector2.Distance(start, curr) * 2f;
            if (width > 100f) {
                _canShootTruck = true;
                _canSetDirection = false;
                _startPosition = GetStartPoint(start, curr);
                _truckDirectionArrow.position = _startPosition.Value;
                _truckDirectionArrow.gameObject.SetActive(true);
            }
        }
        if (_canShootTruck) {
            float degree = ExVector.GetDegree(_startPosition.Value, curr);
            _truckDirectionArrow.eulerAngles = new Vector3(0f, 0f, degree);
        }

        if (_startPosition != null && Input.GetMouseButtonUp(0)) {
            _truckDirectionArrow.gameObject.SetActive(false);
            if (_canShootTruck) {
                _canShootTruck = false;
                SetTruckMoveConfig(curr);
            }
            else {
                _startPosition = null;
            }
        }
    }

    private void SetTruckMoveConfig(Vector2 endPosition) {
        if (_startPosition.Equals(endPosition))
            return;

        float degree = ExVector.GetDegree(_startPosition.Value, endPosition);
        Vector2 startPoint = GetStartPoint(_startPosition.Value, endPosition);
        Vector2 direction = (endPosition - startPoint).normalized;

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
        _cameraMover.SetCameraDirection(_truckObject, direction);
    }
    
    private void OnTruckMoveEnd() {
        foreach (Collider2D boarder in _boarders) {
            boarder.gameObject.SetActive(false);
        }
        _onTruckMoveEnd.Invoke();
    }

    public Vector2 GetStartPoint(Vector2 start, Vector2 end) {
        var hit = Physics2D.Raycast(start, (start - end), 1000f, 1 << LayerMask.NameToLayer("Boarder"));
        Vector2 startPoint = hit.point;
        Vector2 direction = (end - start).normalized;
        startPoint = startPoint - direction * _truckObject.Width * 2f;
        
        return startPoint;
    }
}
