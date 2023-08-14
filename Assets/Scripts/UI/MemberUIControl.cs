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
    [SerializeField] private Transform _memberSpawnPosition = null;
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

        var uiCollider = GetComponent<UICollider>();
        uiCollider.OnMouseDown.AddListener(() => { 
            _additionalWindow.gameObject.ToggleGameObject();
        });
        uiCollider.OnMouseUp.AddListener(() => {
            if (_selectedEntity) {
                CreateUIElement(_selectedEntity);
                _selectedEntity.gameObject.SetActive(false);
                _selectedEntity = null;
            }
        });

        _entityPrefabDictionary = new Dictionary<string, EntityBase>();
        var entityPrefabs = Resources.LoadAll<EntityBase>("Prefabs/Allies");
        foreach (EntityBase entity in entityPrefabs) {
            _entityPrefabDictionary.Add(entity.ID, entity);
            CreateUIElement(entity);
        }
    }

    private void Update() {
        Vector2 mousePosition = MouseUtils.GetMouseWorldPosition();
        if (_selectedEntity) {
            _selectedEntity.transform.position = mousePosition;
        }

        foreach (GameObject obj in _currentMemberUI) {
            Image image = obj.GetComponent<Image>();
            Color uiColor = (_selectedUI == null || obj.Equals(_selectedUI)) ? Color.white : Color.gray;
            image.color = uiColor;
        }
    }

    private void CreateUIElement(EntityBase target) {
        EventTrigger trigger = Instantiate(_memberUIPrefab, _additionalWindow);
        EventTrigger.Entry pointEnter = new EventTrigger.Entry();
        pointEnter.eventID = EventTriggerType.PointerEnter;

        EventTrigger.Entry pointExit = new EventTrigger.Entry();
        pointExit.eventID = EventTriggerType.PointerExit;

        EventTrigger.Entry pointDown = new EventTrigger.Entry();
        pointDown.eventID = EventTriggerType.PointerDown;

        trigger.triggers.Add(pointEnter);
        trigger.triggers.Add(pointExit);
        trigger.triggers.Add(pointDown);

        _currentMemberUI.Add(trigger.gameObject);

        pointEnter.callback.AddListener((pointData) => {
            _selectedUI = trigger.gameObject;
        });
        pointExit.callback.AddListener((pointData) => {
            _selectedUI = null;
        });
        pointDown.callback.AddListener((pointData) => {
            var newEntity = Instantiate(_entityPrefabDictionary[target.ID], _memberSpawnPosition.position, Quaternion.identity);
            /*
            newEntity.UICollider.OnMouseDown.AddListener(() => {
                _selectedEntity = newEntity;
            });
            newEntity.UICollider.OnMouseUp.AddListener(() => {
                _selectedEntity = null;
            });*/

            _onEntityCreated.Invoke(newEntity);
            _currentMemberUI.Remove(_selectedUI);
            Destroy(_selectedUI);
        });
    }
}
