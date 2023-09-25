using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemberUIElement : MonoBehaviour {
    [SerializeField] private Image _portrait;
    [SerializeField] private TMP_Text _maxHPText;
    [SerializeField] private TMP_Text _attackText;
    private EntityDecorator _targetEntity;

    public void SetTargetEntity(EntityDecorator entity) {
        _targetEntity = entity;
    }

    public void UpdateMemberInfo() {
        if (_targetEntity == null) {
            return;
        }
        
        _portrait.sprite = _targetEntity.Info.BodySprite;
        _maxHPText.text = _targetEntity.Health.ToString();
        _attackText.text = _targetEntity.AttackDamage.ToString();
    }
}
