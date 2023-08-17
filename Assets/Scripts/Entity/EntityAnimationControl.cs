using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimationControl : MonoBehaviour {
    [SerializeField] private SpriteRenderer _bodyRenderer = null;
    [SerializeField] private Transform _handTransform = null;
    private Animator _animatorController;
    private static readonly string MovingParameterKey = "isMoving";
    private static readonly string AttackTriggerKey = "doAttack";
    //private static readonly string SkillTriggerKey = "doSkill";
    void Awake() {
        _animatorController = GetComponent<Animator>();
    }

    public void SetFaceDir(Vector2 direction) {
        _bodyRenderer.flipX = (direction.x < 0f);
        _handTransform.localScale = new Vector3(1f, Mathf.Sign(direction.x), 1f);
        _handTransform.rotation = VectorToQuaternion(direction);
    }

    public void SetMoveAnimationState(bool isMoving) {
        _animatorController.SetBool(MovingParameterKey, isMoving);
    }

    public void PlayAttackAnimation() {
        _animatorController.SetTrigger(AttackTriggerKey);
    }

    public void PlayDeadAnimation() {
        _animatorController.SetTrigger("die");
    }

    private Quaternion VectorToQuaternion(Vector2 direction) {
        float theta = Mathf.Atan2(direction.y, direction.x);
        return Quaternion.Euler(0f, 0f, theta * Mathf.Rad2Deg);
    }
}
