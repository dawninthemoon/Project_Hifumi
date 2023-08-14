using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CustomPhysics;
using RieslingUtils;

public class MemberUIControl : MonoBehaviour {
    [SerializeField] private EventTrigger _memberUIPrefab = null;
    [SerializeField] private Transform _additionalWindow = null;
    private Dictionary<string, EntityBase> _entityPrefabDictionary;
    private List<GameObject> _currentMemberUI;
    private EntityBase _selectedEntity;
    private GameObject _selectedUI;
    private System.Action<EntityBase> _onEntityCreated;
    private System.Func<Vector3, EntityBase, bool> _canCreateEnemyCallback;

    private void Awake() {
        GameTest test = GameObject.FindObjectOfType<GameTest>();
        _onEntityCreated = test.OnEntityCreated;
        _canCreateEnemyCallback = test.CanCreateEntity;

        _currentMemberUI = new List<GameObject>();
        GetComponent<UICollider>().OnMouseDown.AddListener(() => { _additionalWindow.gameObject.ToggleGameObject(); });

        _entityPrefabDictionary = new Dictionary<string, EntityBase>();
        var entityPrefabs = Resources.LoadAll<EntityBase>("Prefabs/");
        foreach (EntityBase entity in entityPrefabs) {
            _entityPrefabDictionary.Add(entity.ID, entity);
            CreateUIElement(entity);
        }
    }

    private void Update() {
        Vector2 mousePosition = MouseUtils.GetMouseWorldPosition();
        if (_selectedEntity) {
            _selectedEntity.transform.position = mousePosition;
            if (Input.GetMouseButtonUp(0)) {
                SpawnEntity(mousePosition);
            }
        }
    }

    private void CreateUIElement(EntityBase target) {
        EventTrigger trigger = Instantiate(_memberUIPrefab, _additionalWindow);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        trigger.triggers.Add(entry);
        
        _currentMemberUI.Add(trigger.gameObject);

        entry.callback.AddListener((pointData) => {
            Vector3 mousePosition = MouseUtils.GetMouseWorldPosition().ChangeZPos(0f);
            _selectedEntity = Instantiate(_entityPrefabDictionary[target.ID], mousePosition, Quaternion.identity);
            _selectedUI = trigger.gameObject;
        });
    }

    private void SpawnEntity(Vector3 position) {
        if (_canCreateEnemyCallback(position, _selectedEntity)) {
            _onEntityCreated.Invoke(_selectedEntity);
            _currentMemberUI.Remove(_selectedUI);
            Destroy(_selectedUI);
        }
        else {
            Destroy(_selectedEntity.gameObject);
        }

        _selectedUI.gameObject.transform.localScale = Vector3.one;

        _selectedEntity = null;
        _selectedUI = null;
    }
}
