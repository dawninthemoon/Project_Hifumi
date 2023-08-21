using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EntityUIControl : MonoBehaviour {
    [SerializeField] private Transform _hpBarTransform = null;
    [SerializeField] private Transform _mpBarTransform = null;
    [SerializeField] private GameObject _moraleGameObject = null;
    private TMP_Text _moraleText;

    private void Awake() {
        if (_moraleGameObject)
            _moraleText = _moraleGameObject.GetComponentInChildren<TMP_Text>();
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

    public void SetMoraleUIActive(bool active) {
        _moraleGameObject?.SetActive(active);
    }

    public void UpdateMoraleUI(int morale, int maxMorale) {
        if (_moraleGameObject)
            _moraleText.text = morale.ToString() + " / " + maxMorale.ToString();
    }
}
