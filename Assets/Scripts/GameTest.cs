using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTest : MonoBehaviour {
    [SerializeField] private int _entityCount = 2;
    [SerializeField] private Agent _allyPrefabMelee = null, _allyPrefabRange = null, _enemyPrefab = null;
    private KdTree<Agent> _allies;
    private KdTree<Agent> _enemies;
    private List<Agent> _allAgents;
    public static readonly float Width = 18f;
    public static readonly float Height = 18f;

    private void Awake() {
        InitializeCombat();
    }

    private void InitializeCombat() {
        _allies = new KdTree<Agent>(true);
        _enemies = new KdTree<Agent>(true);
        _allAgents = new List<Agent>();

        for (int i = 0; i < _entityCount; ++i) {
            Vector3 randPos1 = new Vector3(Random.Range(-Width / 2f, Width / 2f), Random.Range(-Height / 2f, Height / 2f));
            Vector3 randPos2 = new Vector3(Random.Range(-Width / 2f, Width / 2f), Random.Range(-Height / 2f, Height / 2f));
            Vector3 randPos3 = new Vector3(Random.Range(-Width / 2f, Width / 2f), Random.Range(-Height / 2f, Height / 2f));
            
            var ally1 = Instantiate(_allyPrefabMelee, randPos1, Quaternion.identity);
            var ally2 = Instantiate(_allyPrefabRange, randPos2, Quaternion.identity);
            var enemy = Instantiate(_enemyPrefab, randPos3, Quaternion.identity);

            _allies.Add(ally1);
            _allies.Add(ally2);
            _enemies.Add(enemy);
            _allAgents.Add(ally1);
            _allAgents.Add(ally2);
            _allAgents.Add(enemy);
        }
    }
    
    private void Start() {

    }

    private void Update() {
        MoveProgress();
        AttackProgress();
    }

    private void MoveProgress() {
        foreach (Agent ally in _allies) {
            Agent target = _enemies.FindClosest(ally.transform.position);

            ally.Move(target);

            foreach (Agent other in _allAgents) {
                if (!other.Equals(ally) && IsIntersects(ally, other)) {
                    EdgeOut(ally, other);
                }
            }
        }
    }

    private void AttackProgress() {
        foreach (Agent ally in _allies) {
            Agent target = _enemies.FindClosest(ally.transform.position);

            if (IsIntersects(target.transform.position, ally.transform.position, target.Radius, ally.AttackRadius)) {
                if (ally.DoingAttack) continue;
                ally.Attack(target);
            }
        }
    }

    private void EdgeOut(Agent agent, Agent other) {
        Vector3 dir = (other.transform.position - agent.transform.position).normalized;
        other.transform.position += dir * (agent.Radius * 0.1f);
        agent.transform.position -= dir * (agent.Radius * 0.1f);
    }

    public bool IsIntersects(Vector2 p1, Vector2 p2, float r1, float r2) {
        float sqrX = Mathf.Pow(p1.x - p2.x, 2f);
        float sqrY = Mathf.Pow(p1.y - p2.y, 2f);
        float sqrRadius = Mathf.Pow(r1 + r2, 2f);

        return sqrX + sqrY < sqrRadius;
    }

    public bool IsIntersects(Agent target, Agent other) {
        Vector2 p1 = target.transform.position;
        Vector2 p2 = other.transform.position;

        return IsIntersects(p1, p2, target.Radius, other.Radius);
    }
}
