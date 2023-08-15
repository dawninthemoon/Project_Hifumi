using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityUIControl : MonoBehaviour {
    [SerializeField] private Transform _hpBarTransform = null;
    [SerializeField] private Transform _mpBarTransform = null;

    void Start() {
        
    }

    public void UpdateHealthBar(int health, int maxHealth) {
        var hpBarScale = _hpBarTransform.localScale;
        hpBarScale.x = (float)health / maxHealth * 0.7f;
        _hpBarTransform.localScale = hpBarScale;
    }

    public void UpdateManaBar(int mana, int maxMana) {
        var mpBarScale = _mpBarTransform.localScale;
        mpBarScale.x = (float)mana / maxMana * 0.7f;
        _mpBarTransform.localScale = mpBarScale;
    }
}
