using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SeekBehaviour : SteeringBehaviour {
    [SerializeField] private float _targetReachedThresold = 18f;
    private bool _reachedLastTarget = true;
    private Vector2 _targetPositionCached;
    private float[] _interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData) {
        if (_reachedLastTarget) {
            if (aiData.targets == null || aiData.targets.Count <= 0) {
                aiData.currentTarget = null;
                return (danger, interest);
            }
            else {
                _reachedLastTarget = false;
                aiData.currentTarget = aiData.targets.OrderBy(target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
            }
        }    

        if (aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget)) {
            _targetPositionCached = aiData.currentTarget.position;
        }
        GameObject target = GameObject.Find("AllyMelee");
        _targetPositionCached = target.transform.position;

        if (Vector2.Distance(transform.position, _targetPositionCached) < _targetReachedThresold) {
            _reachedLastTarget = true;
            aiData.currentTarget = null;
            return (danger, interest);
        }

        Vector2 directionToTarget = (_targetPositionCached - (Vector2)transform.position);
        for (int i = 0; i < interest.Length; ++i) {
            float result = Vector2.Dot(directionToTarget.normalized, Directions.EightDirections[i]);
            if (result > 0) {
                float valueToPutIn = result;
                if (valueToPutIn > interest[i]) {
                    interest[i] = valueToPutIn;
                }
            }
        }
        _interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(_targetPositionCached, 7.2f);

        if (Application.isPlaying && _interestsTemp != null) {
            if (_interestsTemp != null) { 
                Gizmos.color = Color.green;
                for (int i = 0; i < _interestsTemp.Length; ++i) {
                    Gizmos.DrawRay(transform.position, 36f * Directions.EightDirections[i] * _interestsTemp[i]);
                }
                if (!_reachedLastTarget) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_targetPositionCached, 3.6f);
                }
            }
        }
    }
}
