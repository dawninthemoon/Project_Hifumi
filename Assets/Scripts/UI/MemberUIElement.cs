using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemberUIElement : MonoBehaviour {
    [SerializeField] private Image _portrait;
    [SerializeField] private TMP_Text _maxHPText;
    [SerializeField] private TMP_Text _attackText;
    private EntityInfo _targetEntityInfo;

    public void SetEntityInfo(EntityInfo info) {
        _targetEntityInfo = info;
    }

    public void UpdateMemberInfo() {
        if (_targetEntityInfo == null) {
            return;
        }
        
        _portrait.sprite = _targetEntityInfo.BodySprite;
        _maxHPText.text = _targetEntityInfo.Health.ToString();
        _attackText.text = _targetEntityInfo.AttackDamage.ToString();
    }
}
