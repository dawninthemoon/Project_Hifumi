using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CustomPhysics;

public class AgentTest : MonoBehaviour {
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
        _bodyCollider = GetComponent<CustomCollider>();
        _bodyCollider.OnCollisionEvent.AddListener(EdgeOut);
        InvokeRepeating("PerformDetection", 0f, _detectionDelay);
        OnAttackRequested.AddListener(() => Debug.Log("Attack!"));
        OnMovementInput.AddListener((direction) => transform.position += (Vector3)direction * Time.deltaTime * 18f);
    }

    private void PerformDetection() {
        foreach (Detector detector in _detectors) {
            detector.Detect(_aiData);
        }
    }

    private void EdgeOut(CustomCollider self, CustomCollider other) {
        Debug.Log(self.name + ", " + other.name);
        Transform selfObj = self.transform.parent;
        Transform otherObj = other.transform.parent;
        if (selfObj == null)
            selfObj = self.transform;
        if (otherObj == null)
            otherObj = other.transform;

        float radius = (self as CircleCollider).CircleShape.radius;
        Vector3 pos = (other as RectCollider).GetClosestPoint(selfObj.transform.position);
        Vector3 dir = (selfObj.transform.position - pos).normalized;
        selfObj.transform.position += dir * (radius);

        //otherObj.transform.position -= dir * (radius * 0.1f);
    }

    private void Update() {
        if (_aiData.currentTarget) {
            OnPointerInput?.Invoke(_aiData.currentTarget.position);
            if (!_following) {
                _following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }
        else if (_aiData.TargetsCount > 0) {
            _aiData.currentTarget = _aiData.targets[0];
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
