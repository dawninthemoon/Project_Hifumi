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
    private EntityStatusDecorator _entityStatus;
    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
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
        _aiData.Radius = radius;
        _aiData.AttackRange = _entityStatus.AttackRange;
        OnMovementInput.RemoveAllListeners();
    }

    private void OnEnable() {
        ExecuteAgent();
    }

    private void Start() {
        InvokeRepeating("PerformDetection", 0f, _detectionDelay);
    }

    public void SetTarget(ITargetable target) {
        _aiData.SelectedTarget = target;
    }

    private void PerformDetection() {
        foreach (Detector detector in _detectors) {
            detector.Detect(_aiData);
        }
    }

    private void ExecuteAgent() {
        StartCoroutine(Chase());
        StartCoroutine(Attack());
    }

    private IEnumerator Chase() {
        while (gameObject.activeSelf) {
            _movementInput = Vector2.zero;
            if (_aiData.SelectedTarget != null) {
                _movementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, _aiData);
                yield return YieldInstructionCache.WaitForSeconds(_aiUpdateDelay);
            }

            OnMovementInput?.Invoke(_movementInput);
            yield return null;
        }
    }

    private IEnumerator Attack() {
        while (gameObject.activeSelf) {
            if (_aiData.SelectedTarget != null) {
                float distance = Vector2.Distance(_aiData.SelectedTarget.Position, transform.position);
                if (distance - Mathf.Sqrt(_aiData.SelectedTarget.Radius) < _entityStatus.AttackRange) {
                    OnAttackRequested?.Invoke();
                    yield return YieldInstructionCache.WaitForSeconds(_entityStatus.AttackSpeed);
                }
            }
            yield return null;
        }
    }
    
    private void Update() {
        OnMovementInput?.Invoke(_movementInput);
    }
}
