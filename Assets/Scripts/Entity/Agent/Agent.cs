using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class Agent : MonoBehaviour, ITargetable {
    [SerializeField] private float _moveSpeed = 30f;
    [SerializeField] private List<SteeringBehaviour> _steeringBehaviours = null;
    [SerializeField] private List<Detector> _detectors = null;
    [SerializeField] private ContextSolver _movementDirectionSolver = null;
    [SerializeField] private float _detectionDelay = 0.05f, _aiUpdateDelay = 0.06f, _attackDelay = 1f;
    [SerializeField] private float _scentDelay = 0.3f;
    [SerializeField] private float _attackDistance = 18f;
    private AIData _aiData;
    private Vector2 _movementInput;
    private float _scentCounter;
    private bool _following;
    private Rigidbody2D _rigidbody;
    public UnityEvent OnAttackRequested { get; set; }
    public UnityEvent<Vector2> OnMovementInput { get; set; }
    public AgentScent Scent { get; private set; }
    public Vector3 Position {
        get { return transform.position; }
    }
    public float AttackDistance { 
        get { return _attackDistance; }
    }

    private void Awake() {
        _aiData = new AIData();
        Scent = new AgentScent();

        _rigidbody = GetComponent<Rigidbody2D>();

        OnAttackRequested = new UnityEvent();
        OnMovementInput = new UnityEvent<Vector2>();
        
        _aiData.attackDistance = _attackDistance;

        OnMovementInput.AddListener((direction) => {
            if (direction.sqrMagnitude > 0f) {
                Vector3 nextPosition = transform.position + (Vector3)direction * Time.deltaTime * _moveSpeed;
                _rigidbody.MovePosition(nextPosition);
            }
        });
    }

    private void Start() {
        InvokeRepeating("PerformDetection", 0f, _detectionDelay);
    }

    private void OnDisable() {
        _following = false;
        Scent.Reset();
    }

    public void SetTarget(ITargetable target) {
        _aiData.SelectedTarget = target;
    }

    public List<Vector2> GetScentTrail() {
        return Scent.ScentTrail;
    }

    private void PerformDetection() {
        foreach (Detector detector in _detectors) {
            detector.Detect(_aiData);
        }
    }

    private void ScentProgress() {
        Scent.AddScent(transform.position);
    }

    private void Update() {
        _scentCounter += Time.deltaTime;
        if (_scentCounter > _scentDelay) {
            _scentCounter -= _scentCounter;
            ScentProgress();
        }

        if (_aiData.CurrentTarget != null) {
            if (!_following) {
                _following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }

        OnMovementInput?.Invoke(_movementInput);
    }

    private IEnumerator ChaseAndAttack() {
        while (true) {
            if (_aiData.SelectedTarget == null) {
                _movementInput = Vector2.zero;
                _following = false;
                yield break;
            }
            else {
                float distance = Vector2.Distance(_aiData.SelectedTarget.Position, transform.position);
                if (distance < _attackDistance) {
                    _movementInput = Vector2.zero;
                    OnAttackRequested?.Invoke();
                    yield return YieldInstructionCache.WaitForSeconds(_attackDelay);
                }
                else {
                    _movementInput = _movementDirectionSolver.GetDirectionToMove(_steeringBehaviours, _aiData);
                    yield return YieldInstructionCache.WaitForSeconds(_aiUpdateDelay);
                }
            }
        }
    }
}
