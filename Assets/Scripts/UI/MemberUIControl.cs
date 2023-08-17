using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class MemberUIControl : MonoBehaviour {
    [SerializeField] private MemberUIElement _memberUIPrefab = null;
    [SerializeField] private Transform _additionalWindow = null;
    [SerializeField] private Transform _memberSpawnPosition = null;
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
        Vector2 mousePosition = MouseUtils.GetMouseWorldPosition();
        if (_selectedEntity) {
            _selectedEntity.transform.position = mousePosition;
        }
    }

    private void SetEntityInteraction(EntityBase target) {
        var entityInteractiveCallback = target.GetComponent<InteractiveEntity>();

        entityInteractiveCallback.OnMouseDownEvent.AddListener(() => {
            _selectedEntity = target;
        });

        entityInteractiveCallback.OnMouseUpEvent.AddListener(() => {
            var hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 100f, (1 << gameObject.layer));
            if (hit.collider != null) {
                CreateUIElement(_selectedEntity);
                _onEntityInactive.Invoke(target);
            }
            _selectedEntity = null;
        });
    }

    private void CreateUIElement(EntityBase target) {
        MemberUIElement uiElement = Instantiate(_memberUIPrefab, _additionalWindow);
        Animator animator = uiElement.GetComponent<Animator>();

        _currentMemberUI.Add(target.ID, uiElement);

        uiElement.PointEnter.callback.AddListener((pointData) => {
            _selectedUI = uiElement.gameObject;
            animator.SetTrigger("highlight");
        });
        uiElement.PointExit.callback.AddListener((pointData) => {
            _selectedUI = null;
            animator.SetTrigger("normal");
        });
        uiElement.PointDown.callback.AddListener((pointData) => {
            target.transform.position = _memberSpawnPosition.position;
            _onEntityActive.Invoke(target);
            _currentMemberUI.Remove(target.ID);
            Destroy(_selectedUI);
        });
    }

    public void UpdateMemberElement(EntityBase entity) {
        if (_currentMemberUI.TryGetValue(entity.ID, out MemberUIElement uiElement)) {
            uiElement.UpdateHealthText(entity.Health);
            uiElement.UpdateManaText(entity.Mana);
            uiElement.UpdateStressText(entity.Stress);
            uiElement.UpdateManaText(entity.AttackDamage);
        }
        else {
            Debug.LogError("Member UI Not Exits!");
        }
    }
}
