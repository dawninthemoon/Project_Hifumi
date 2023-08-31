using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class Agent : MonoBehaviour, ITargetable {
    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours = null;
    [SerializeField] private List<Detector> _detectors = null;
    [SerializeField] private ContextSolver _movementDirectionSolver = null;
    [SerializeField] private float _detectionDelay = 0.05f, _aiUpdateDelay = 0.06f;
    private AIData _aiData;
    private Vector2 _movementInput;
    private bool _following;
    private EntityStatusDecorator _entityStatus;
    private Rigidbody2D _rigidbody;
    public UnityEvent OnAttackRequested { get; set; } = new UnityEvent();
    public UnityEvent<Vector2> OnMovementInput { get; set; } = new UnityEvent<Vector2>();
    public Vector3 Position {
        get { return transform.position; }
    }
    public float Radius { get; private set; }

    private void Awake() {
        _aiData = new AIData();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(EntityStatusDecorator status, float radius) {
        _entityStatus = status;
        Radius = radius;

        OnMovementInput.RemoveAllListeners();
    }

    private void Start() {
        InvokeRepeating("PerformDetection", 0f, _detectionDelay);
    }

    private void OnDisable() {
        _following = false;
    }

    public void SetTarget(ITargetable target) {
        _aiData.SelectedTarget = target;
    }

    private void PerformDetection() {
        foreach (Detector detector in _detectors) {
            detector.Detect(_aiData);
        }
    }

    private void Update() {
        if (_aiData.CurrentTarget != null) {
            if (!_following) {
                _following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }
        OnMovementInput?.Invoke(_movementInput);
    }

    public void ApplyKnockback(Vector2 knockback) {
        _rigidbody.AddForce(knockback, ForceMode2D.Impulse);
    }

    private IEnumerator ChaseAndAttack() {
        while (_following) {
            if (_aiData.SelectedTarget == null) {
                _movementInput = Vector2.zero;
                _following = false;
                yield break;
            }
            else {
                float distance = Vector2.Distance(_aiData.SelectedTarget.Position, transform.position);
                if (distance - Mathf.Sqrt(_aiData.SelectedTarget.Radius) < _entityStatus.AttackRange) {
                    _movementInput = Vector2.zero;
                    OnAttackRequested?.Invoke();
                    yield return YieldInstructionCache.WaitForSeconds(_entityStatus.AttackSpeed);
                }
                else {
                    _movementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, _aiData);
                    yield return YieldInstructionCache.WaitForSeconds(_aiUpdateDelay);
                }
            }
        }
    }
}
