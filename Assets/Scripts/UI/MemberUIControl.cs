using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class MemberUIControl : MonoBehaviour {
    [SerializeField] private MemberUIElement _memberUIPrefab = null;
    [SerializeField] private Transform _additionalWindow = null;
    private Dictionary<string, MemberUIElement> _currentMemberUI;
    private EntityBase _selectedEntity;
    private GameObject _selectedUI;
    private UnityAction<EntityBase> _onEntityActive;
    private UnityAction<EntityBase> _onEntityInactive;
    private InteractiveEntity _interactiveZone;

    private void Awake() {
        _currentMemberUI = new Dictionary<string, MemberUIElement>();

        _interactiveZone = GetComponent<InteractiveEntity>();
        _interactiveZone.OnMouseDownEvent.AddListener(() => {
            _additionalWindow.gameObject.ToggleGameObject();
        });
    }

    public void InitializeEntityUI(UnityAction<EntityBase> onEntityActive, UnityAction<EntityBase> onEntityInactive, List<EntityBase> entities) {
        _onEntityActive = onEntityActive;
        _onEntityInactive = onEntityInactive;

        foreach (EntityBase entity in entities) {
            SetEntityInteraction(entity);
        }
    }
    
    private void Update() {
        if (_selectedEntity) {
            Vector2 mousePosition = MouseUtils.GetMouseWorldPosition();
            _selectedEntity.transform.position = mousePosition;
        }
    }

    private void SetEntityInteraction(EntityBase target) {
        var entityInteractiveCallback = target.GetComponent<InteractiveEntity>();

        entityInteractiveCallback.OnMouseDownEvent.AddListener(() => {
            _selectedEntity = target;
            target.SetEntitySelected(true);
        });

        entityInteractiveCallback.OnMouseDragEvent.AddListener(() => {
            target.Morale -= 2f * Time.deltaTime;
        });

        entityInteractiveCallback.OnMouseUpEvent.AddListener(() => {
            var hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 100f, (1 << gameObject.layer));
            if (hit.collider != null) {
                CreateUIElement(_selectedEntity);
                _onEntityInactive.Invoke(target);
            }
            target.SetEntitySelected(false);
            _selectedEntity = null;
        });
    }

    private void CreateUIElement(EntityBase target) {
        MemberUIElement uiElement = Instantiate(_memberUIPrefab, _additionalWindow);

        _currentMemberUI.Add(target.ID, uiElement);

        uiElement.PointEnter.callback.AddListener((pointData) => {
            _selectedUI = uiElement.gameObject;
            uiElement.SetHighlight();
        });
        uiElement.PointExit.callback.AddListener((pointData) => {
            _selectedUI = null;
            uiElement.SetNormal();
        });
        uiElement.PointDown.callback.AddListener((pointData) => {
            if (target.Morale < 20) return;

            _onEntityActive.Invoke(target);
            _currentMemberUI.Remove(target.ID);
            Destroy(_selectedUI);
        });
    }

    public void UpdateMemberElement(EntityBase entity) {
        if (!_additionalWindow.gameObject.activeSelf)
            return;
        if (_currentMemberUI.TryGetValue(entity.ID, out MemberUIElement uiElement)) {
            uiElement.SetLocked(entity.Morale <= 20);

            entity.Morale += Time.deltaTime;
            uiElement.UpdateHealthText(entity.Health);
            uiElement.UpdateManaText(entity.Mana);
            uiElement.UpdateMoraleText(Mathf.FloorToInt(entity.Morale));
            uiElement.UpdateManaText(entity.AttackDamage);
        }
    }
}
