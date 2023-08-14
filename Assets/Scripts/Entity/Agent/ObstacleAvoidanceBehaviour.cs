using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public static class Directions {
    public static readonly List<Vector2> EightDirections = new List<Vector2> {
        new Vector2(0f, 1f).normalized,
        new Vector2(1f, 1f).normalized,
        new Vector2(1f, 0f).normalized,
        new Vector2(1f, -1f).normalized,
        new Vector2(0f, -1f).normalized,
        new Vector2(-1f, -1f).normalized,
        new Vector2(-1f, 0f).normalized,
        new Vector2(-1f, 1f).normalized,
    };
}

public class ObstacleAvoidanceBehaviour : SteeringBehaviour {
    [SerializeField] private float _radius = 100f;
    [SerializeField] private float _agentColliderSize = 20f;
    private float[] _dangersResultTemp;
    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData) {
        foreach (CustomCollider obstacleCollider in aiData.obstacles) {
            Vector2 directionToObstacle
                = obstacleCollider.GetClosestPoint(transform.position) - (Vector2)transform.position;
            float distanceToObstacle = directionToObstacle.magnitude;

            float weight = (distanceToObstacle <= _agentColliderSize) ? 1 : (_radius - distanceToObstacle) / _radius;

            Vector2 directionToObstacleNormalized = directionToObstacle.normalized;

            for (int i = 0; i < Directions.EightDirections.Count; ++i) {
                float result = Vector2.Dot(directionToObstacleNormalized, Directions.EightDirections[i]);
                float valueToPutIn = result * weight;

                if (valueToPutIn > danger[i]) {
                    danger[i] = valueToPutIn;
                }
            }
        }
        _dangersResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos() {
        if (Application.isPlaying && _dangersResultTemp != null) {
            Gizmos.color = Color.red;
            for (int i = 0; i < _dangersResultTemp.Length; ++i) {
                Gizmos.DrawRay(transform.position, 36f * Directions.EightDirections[i] * _dangersResultTemp[i]);
            }
        }
        else {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
