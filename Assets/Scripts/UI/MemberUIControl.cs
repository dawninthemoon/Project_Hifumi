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
    private InteractiveEntity _interactiveZone;

    private void Awake() {
        GameTest test = GameObject.FindObjectOfType<GameTest>();
        _onEntityCreated = test.OnEntityCreated;

        _currentMemberUI = new List<GameObject>();

        _interactiveZone = GetComponent<InteractiveEntity>();
        _interactiveZone.OnMouseDownEvent.AddListener(() => {
            _additionalWindow.gameObject.ToggleGameObject();
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
    }

    private void CreateUIElement(EntityBase target) {
        EventTrigger trigger = Instantiate(_memberUIPrefab, _additionalWindow);
        Animator animator = trigger.GetComponent<Animator>();
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
            animator.SetTrigger("highlight");
        });
        pointExit.callback.AddListener((pointData) => {
            _selectedUI = null;
            animator.SetTrigger("normal");
        });
        pointDown.callback.AddListener((pointData) => {
            var newEntity = Instantiate(_entityPrefabDictionary[target.ID], _memberSpawnPosition.position, Quaternion.identity);
            var entityInteractiveCallback = newEntity.GetComponent<InteractiveEntity>();
            
            entityInteractiveCallback.OnMouseDownEvent.AddListener(() => {
                _selectedEntity = newEntity;
            });
            entityInteractiveCallback.OnMouseUpEvent.AddListener(() => {
                var hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 100f, (1 << gameObject.layer));
                if (hit.collider != null) {
                    CreateUIElement(_selectedEntity);
                    _selectedEntity.gameObject.SetActive(false);
                }
                _selectedEntity = null;
            });

            _onEntityCreated.Invoke(newEntity);
            _currentMemberUI.Remove(_selectedUI);
            Destroy(_selectedUI);
        });
    }
}
