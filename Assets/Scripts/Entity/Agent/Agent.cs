using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CustomPhysics;

public class Agent : MonoBehaviour {
    [SerializeField] private float _moveSpeed = 30f;
    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours = null;
    [SerializeField] private List<Detector> _detectors = null;
    [SerializeField] private AIData _aiData = null;
    [SerializeField] private ContextSolver _movementDirectionSolver = null;
    [SerializeField] private float _detectionDelay = 0.05f, _aiUpdateDelay = 0.06f, _attackDelay = 1f;
    [SerializeField] private float _attackDistance = 18f;
    [SerializeField] private Vector2 _movementInput;
    private CustomCollider _bodyCollider;
    public UnityEvent OnAttackRequested;
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    private bool _following;

    private void Start() {
        InvokeRepeating("PerformDetection", 0f, _detectionDelay);
        OnAttackRequested.AddListener(() => Debug.Log("Attack!"));
        OnMovementInput.AddListener((direction) => transform.position += (Vector3)direction * Time.deltaTime * _moveSpeed);
    }

    public void SetTarget(Transform target) {
        _aiData.selectedTarget = target;
    }

    private void PerformDetection() {
        foreach (Detector detector in _detectors) {
            detector.Detect(_aiData);
        }
    }

    private void Update() {
        if (_aiData.currentTarget) {
            OnPointerInput?.Invoke(_aiData.currentTarget.position);
            if (!_following) {
                _following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }
        OnMovementInput?.Invoke(_movementInput);
    }

    private IEnumerator ChaseAndAttack() {
        while (true) {
            if (!_aiData.currentTarget) {
                _movementInput = Vector2.zero;
                _following = false;
                yield break;
            }
            else {
                float distance = Vector2.Distance(_aiData.currentTarget.position, transform.position);
                if (distance < _attackDistance) {
                    _movementInput = Vector2.zero;
                    OnAttackRequested?.Invoke();
                    yield return new WaitForSeconds(_attackDelay);
                }
                else {
                    _movementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, _aiData);
                    yield return new WaitForSeconds(_aiUpdateDelay);
                }
            }
        }
    }
}
