using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MemberUIElement : MonoBehaviour {
    [SerializeField] private TMP_Text _healthText = null;
    [SerializeField] private TMP_Text _manaText = null;
    [SerializeField] private TMP_Text _stressText = null;
    [SerializeField] private TMP_Text _attackText = null;
    private EventTrigger _eventTrigger;
    public EventTrigger.Entry PointEnter{
        get; private set;
    } = new EventTrigger.Entry();
    public EventTrigger.Entry PointExit {
        get; private set;
    } = new EventTrigger.Entry();
    public EventTrigger.Entry PointDown {
        get; private set;
    } = new EventTrigger.Entry();

    private void Awake() {
        InitalizeEventTrigger();
    }

    private void InitalizeEventTrigger() {
        _eventTrigger = GetComponent<EventTrigger>();

        PointEnter.eventID = EventTriggerType.PointerEnter;
        PointExit.eventID = EventTriggerType.PointerExit;
        PointDown.eventID = EventTriggerType.PointerDown;

        _eventTrigger.triggers.Add(PointEnter);
        _eventTrigger.triggers.Add(PointExit);
        _eventTrigger.triggers.Add(PointDown);
    }

    public void UpdateHealthText(int health) {
        _healthText.text = health.ToString();
    }

    public void UpdateManaText(int mana) {
        _manaText.text = mana.ToString();
    }

    public void UpdateMoraleText(int stress) {
        _stressText.text = stress.ToString();
    }

    public void UpdateAttackText(int attack) {
        _attackText.text = attack.ToString();
    }
}
