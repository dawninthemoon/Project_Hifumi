using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

[System.Serializable]
public struct CircleColliderConfig {
    public float radius;
    public Vector2 offset;
    public Color color;
}

public class Agent {
    private CircleCollider _bodyCollider;
    private float _moveSpeed;

    private static readonly float RequireMoveTime = 0.5f;
    public float MovedTime { get; set; }
    public bool DoingMove { 
        get {
            return MovedTime < RequireMoveTime;
        }
    }

    private void Awake() {
        MovedTime = RequireMoveTime;
    }

    public Agent(CircleCollider bodyCollider, float moveSpeed) {
        _bodyCollider = bodyCollider;
        _moveSpeed = moveSpeed;

        _bodyCollider.OnCollisionEvent.AddListener(EdgeOut);
    }

    public (Vector2, bool) Move(Transform transform, EntityBase target, float attackRangeRadius) {
        bool isAgentMoved;
        MovedTime += Time.deltaTime;

        Vector3 moveDirection = (target.transform.position - transform.position).normalized;
        Vector3 targetPosition = target.transform.position;
        targetPosition -= moveDirection * (target.Radius + attackRangeRadius - 0.1f);

        float t = (Time.deltaTime * _moveSpeed) / Vector2.Distance(transform.position, targetPosition);
        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPosition, t);

        isAgentMoved = nextPosition.Equals(transform.position);
        transform.position = nextPosition;

        return (moveDirection, isAgentMoved);
    }

    private void EdgeOut(CustomCollider self, CustomCollider other) {
        Transform selfObj = self.transform.parent;
        Transform otherObj = other.transform.parent;
        if (otherObj == null)
            otherObj = other.transform;

        float radius = (self as CircleCollider).CircleShape.radius;
        Vector3 dir = (selfObj.transform.position - otherObj.transform.position).normalized;
        selfObj.transform.position += dir * (radius * 0.2f);
        //otherObj.transform.position -= dir * (radius * 0.1f);
    }
}
