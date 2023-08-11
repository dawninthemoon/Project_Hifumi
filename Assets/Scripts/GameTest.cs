using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class GameTest : MonoBehaviour {
    [SerializeField] private int _entityCount = 2;
    [SerializeField] private Agent _allyPrefabMelee = null, _allyPrefabRange = null;
    [SerializeField] private Agent _enemyPrefabMelee = null, _enemyPrefabRange = null;
    private KdTree<Agent> _allies;
    private KdTree<Agent> _enemies;
    private List<Agent> _allAgents;
    public static readonly float Width = 18f;
    public static readonly float Height = 10f;
    private float _maxWidth;
    private float _minWidth;
    private float _maxHeight;
    private float _minHeight;

    private void Awake() {
    }

    private void Start() {
        InitializeCombat();
    }

    private void InitializeCombat() {
        if (_allAgents != null) {
            foreach (var toRemove in _allAgents) {
                Destroy(toRemove.gameObject);
            }
        }

        _allies = new KdTree<Agent>(true);
        _enemies = new KdTree<Agent>(true);
        _allAgents = new List<Agent>();

        for (int i = 0; i < _entityCount; ++i) {
            _minWidth = -Width / 2f + _allyPrefabMelee.Radius;
            _maxWidth = Width / 2f - _allyPrefabMelee.Radius;

            _minHeight = -Height / 2f + _allyPrefabMelee.Radius;
            _maxHeight = Height / 2f - _allyPrefabMelee.Radius;

            Vector3 randPos1 = new Vector3(Random.Range(_minWidth, _maxWidth), Random.Range(_minHeight, _maxHeight));
            Vector3 randPos2 = new Vector3(Random.Range(_minWidth, _maxWidth), Random.Range(_minHeight, _maxHeight));
            
            Agent allyPrefab = _allyPrefabMelee;
            if (Random.Range(0, 2) > 0) {
                allyPrefab = _allyPrefabRange;
            }
            Agent enemyPrefab = _enemyPrefabMelee;
            if (Random.Range(0, 2) > 0) {
                enemyPrefab = _enemyPrefabRange;
            }

            var ally = Instantiate(allyPrefab, randPos1, Quaternion.identity);
            var enemy = Instantiate(enemyPrefab, randPos2, Quaternion.identity);

            _allies.Add(ally);
            _enemies.Add(enemy);
            _allAgents.Add(ally);
            _allAgents.Add(enemy);
        }
    }

    private void Update() {
        MoveProgress();

        for (int i = 0; i < _allAgents.Count; ++i) {
            var agent = _allAgents[i];
            if (agent.Hp <= 0) {
                _allAgents.RemoveAt(i--);
                agent.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _enemies.Count; ++i) {
            var enemy = _enemies[i];
            if (!enemy.gameObject.activeSelf) {
                _enemies.RemoveAt(i--);
                Destroy(enemy.gameObject);
            }
        }

        for (int i = 0; i < _allies.Count; ++i) {
            var ally = _allies[i];
            if (!ally.gameObject.activeSelf) {
                _allies.RemoveAt(i--);
                Destroy(ally.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            InitializeCombat();
        }
    }

    private void MoveProgress() {
        foreach (Agent ally in _allies) {
            Agent target = _enemies.FindClosest(ally.transform.position);
            if (ally.DoingAttack || target == null) {
                ally.SetMoveAnimationState(false);
                continue;
            }

            ally.Move(target);

            ClampPosition(ally);
        }

        foreach (Agent enemy in _enemies) {
            Agent target = _allies.FindClosest(enemy.transform.position);
            if (enemy.DoingAttack || target == null) {
                enemy.SetMoveAnimationState(true);
                continue;
            }

            enemy.Move(target);
            
            ClampPosition(enemy);
        }
    }

    private void ClampPosition(Agent agent) {
        Vector2 pos = agent.transform.position;
        pos.x = Mathf.Clamp(pos.x, _minWidth, _maxWidth);
        pos.y = Mathf.Clamp(pos.y, _minHeight, _maxHeight);
        agent.transform.position = pos;
    }
}
