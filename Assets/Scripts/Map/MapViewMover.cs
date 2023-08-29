using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class MapViewMover : MonoBehaviour {
    [SerializeField] private float _boarderSize = 30f;
    [SerializeField] private float _width = 640f;
    [SerializeField] private float _height = 360f;
    [SerializeField] private float _moveAmount = 50f;
    private Camera _mainCamera;
    private Vector3 _lastCameraPosition = new Vector3(0f, 0f, -10f);

    private void Awake() {
        _mainCamera = Camera.main;
    }

    private void OnEnable() {
        _mainCamera.transform.position = _lastCameraPosition;
    }

    private void Update() {
        Vector3 direction = GetCameraDirection(MouseUtils.GetMouseWorldPosition());
        _mainCamera.transform.position += direction * _moveAmount * Time.deltaTime;
        _lastCameraPosition = _mainCamera.transform.position;
    }

    private Vector3 GetCameraDirection(Vector2 position) {
        Vector3 dir = Vector3.zero;

        if (IsOverlapedWithTop(position)) {
            dir += Vector3.up;
        }
        else if (IsOverlapedWithBottom(position)) {
            dir += Vector3.down;
        }

        /*
        if (IsOverlapedWithLeft(position)) {
            dir += Vector3.left;
        }
        else if (IsOverlapedWithRight(position)) {
            dir += Vector3.right;
        }
        */
        return dir.normalized;
    }

    private bool IsOverlapedWithTop(Vector2 position) {
        float topY = _mainCamera.transform.position.y + _height * 0.5f;
        return (position.y > topY - _boarderSize) && (position.y < topY);
    }

    private bool IsOverlapedWithBottom(Vector2 position) {
        float bottomY = _mainCamera.transform.position.y - _height * 0.5f;
        return (position.y < bottomY + _boarderSize) && (position.y > bottomY);
    }

    private bool IsOverlapedWithLeft(Vector2 position) {
        float leftX = _mainCamera.transform.position.x - _width * 0.5f;
        return (position.x < leftX + _boarderSize) && (position.x > leftX);
    }

    private bool IsOverlapedWithRight(Vector2 position) {
        float rightX = _mainCamera.transform.position.x + _width * 0.5f;
        return (position.x > rightX - _boarderSize) && (position.x < rightX);
    }
}
