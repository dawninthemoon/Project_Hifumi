using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageUIElement : MonoBehaviour {
    [SerializeField] private Image _portrait;
    [SerializeField] private Image _damageBar;
    [SerializeField] private TextMeshProUGUI _damageText;
    private float _maxDamageBarWidth;
    public int DamageSum {
        get;
        private set;
    }

    private void Awake() {
        _maxDamageBarWidth = _damageBar.rectTransform.rect.width;
    }

    public int AddDamage(int amount) {
        DamageSum += amount;
        _damageText.text = DamageSum.ToString();

        return DamageSum;
    }

    public void UpdateDamageBarWidth(int maxDamageSum) {
        float ratio = (float)DamageSum / maxDamageSum;

        Rect damageBarRect = _damageBar.rectTransform.rect;
        damageBarRect.width = _maxDamageBarWidth * ratio;
        _damageBar.rectTransform.sizeDelta = new Vector2(damageBarRect.width, damageBarRect.height);
    }
}
