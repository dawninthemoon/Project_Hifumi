using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class EntityAnimationControl : MonoBehaviour {
    [SerializeField] private SpriteRenderer _bodyRenderer = null;
    [SerializeField] private SpriteRenderer _weaponRenderer = null;
    [SerializeField] private Transform _handTransform = null;
    private Animator _animator;
    private static readonly string MovingParameterKey = "isMoving";
    private static readonly string AttackTriggerKey = "doAttack";
    //private static readonly string SkillTriggerKey = "doSkill";
    void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void Initialize(Sprite bodySprite, Sprite weaponSprite, RuntimeAnimatorController controller) {
        _bodyRenderer.sprite = bodySprite;
        _weaponRenderer.sprite = weaponSprite;
        _animator.runtimeAnimatorController = controller;
    }

    public void SetFaceDir(Vector2 direction) {
        _bodyRenderer.flipX = (direction.x < 0f);
        _handTransform.localScale = new Vector3(1f, Mathf.Sign(direction.x), 1f);
        _handTransform.rotation = VectorToQuaternion(direction);
    }

    public void SetMoveAnimationState(bool isMoving) {
        _animator.SetBool(MovingParameterKey, isMoving);
    }

    public void PlayAttackAnimation() {
        _animator.SetTrigger(AttackTriggerKey);
    }

    public void PlayDeadAnimation() {
        _animator.SetTrigger("die");
    }

    private Quaternion VectorToQuaternion(Vector2 direction) {
        float theta = Mathf.Atan2(direction.y, direction.x);
        return Quaternion.Euler(0f, 0f, theta * Mathf.Rad2Deg);
    }
}
