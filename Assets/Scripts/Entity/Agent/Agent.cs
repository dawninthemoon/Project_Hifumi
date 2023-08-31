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

    private void OnEnable() {
        StartCoroutine(ChaseAndAttack());
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

    private IEnumerator ChaseAndAttack() {
        while (gameObject.activeSelf) {
            Vector2 movementInput = Vector2.zero;

            if (_aiData.SelectedTarget != null) {
                float distance = Vector2.Distance(_aiData.SelectedTarget.Position, transform.position);
                if (distance - Mathf.Sqrt(_aiData.SelectedTarget.Radius) < _entityStatus.AttackRange) {
                    movementInput = Vector2.zero;
                    OnAttackRequested?.Invoke();
                    yield return YieldInstructionCache.WaitForSeconds(_entityStatus.AttackSpeed);
                }
                else {
                    movementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, _aiData);
                    yield return YieldInstructionCache.WaitForSeconds(_aiUpdateDelay);
                }
            }

            OnMovementInput?.Invoke(movementInput);
            yield return null;
        }
    }
}
