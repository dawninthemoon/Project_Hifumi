using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;
using RieslingUtils;

public class MemberUIElement {
    public string targetID;
    public GameObject gameObject;
    public RectCollider collider;
    public MemberUIElement(string id, GameObject gameObject, RectCollider collider) {
        this.targetID = id;
        this.gameObject = gameObject;
        this.collider = collider;
    }
}

public class MemberUITest : MonoBehaviour {
    [SerializeField] private GameObject _memberUIPrefab = null;
    [SerializeField] private Transform _additionalWindow = null;
    private Dictionary<string, EntityBase> _entityPrefabDictionary;
    private List<MemberUIElement> _currentMemberUI;
    private Circle _mouseShape;
    private MemberUIElement _selectedUI;
    private EntityBase _selectedEntity;
    public System.Action<EntityBase> OnEntityCreated { get; set; }
    private RectCollider _zoneCollider;

    private void Awake() {
        _currentMemberUI = new List<MemberUIElement>();
        _zoneCollider = GetComponent<RectCollider>();

        _entityPrefabDictionary = new Dictionary<string, EntityBase>();
        var entityPrefabs = Resources.LoadAll<EntityBase>("Prefabs/");
        foreach (EntityBase entity in entityPrefabs) {
            _entityPrefabDictionary.Add(entity.ID, entity);

            var uiObj = Instantiate(_memberUIPrefab, _additionalWindow);
            var collider = uiObj.GetComponentInChildren<RectCollider>();

            uiObj.transform.localPosition = GetUIPosition(_currentMemberUI.Count);
            uiObj.transform.SetParent(null);
            uiObj.transform.localScale = Vector3.one;

            _currentMemberUI.Add(new MemberUIElement(entity.ID, uiObj, collider));
        }

        SetUIActive(false);
    }

    private void Update() {
        if (IsMouseDownWithCollider(_zoneCollider)) {
            SetUIActive(!_additionalWindow.gameObject.activeSelf);
        }

        Vector2 mousePosition = MouseUtils.GetMouseWorldPosition();
        if (_selectedEntity) {
            _selectedEntity.transform.position = mousePosition;
            if (Input.GetMouseButtonUp(0)) {
                SpawnEntity();
            }
        }

        foreach (MemberUIElement memberUI in _currentMemberUI) {
            if (IsMouseOverWithCollider(memberUI.collider)) {
                memberUI.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                if (Input.GetMouseButtonDown(0)) {
                    _selectedUI = memberUI;
                    _selectedEntity = Instantiate(_entityPrefabDictionary[memberUI.targetID], mousePosition, Quaternion.identity);
                }
            }
            else {
                memberUI.gameObject.transform.localScale = Vector3.one;
            }
        }

        if (_selectedUI != null) {
            _selectedUI.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        }
    }

    private void SetUIActive(bool active) {
        _additionalWindow.gameObject.SetActive(active);
        foreach (MemberUIElement element in _currentMemberUI) {
            element.gameObject.SetActive(active);
        }
    }

    private void AlignUIPosition() {
        for (int i = 0; i < _currentMemberUI.Count; ++i) {
            GameObject obj = _currentMemberUI[i].gameObject;
            obj.transform.SetParent(_additionalWindow);
            obj.transform.localPosition = GetUIPosition(i);
            obj.transform.SetParent(null);
        }
    }

    private Vector3 GetUIPosition(int index) {
        int row = index / 3;
        int column = index % 3;

        return new Vector3(-0.3f + column * 0.3f, 0.25f - row * 0.5f);
    }

    private void SpawnEntity() {
        OnEntityCreated.Invoke(_selectedEntity);
        _selectedUI.gameObject.transform.localScale = Vector3.one;

        _currentMemberUI.Remove(_selectedUI);
        Destroy(_selectedUI.gameObject);

        _selectedEntity = null;
        _selectedUI = null;

        AlignUIPosition();
    }

    private bool IsMouseOverWithCollider(RectCollider collider) {
        Vector2 mousePosition = MouseUtils.GetMouseWorldPosition();
        Circle mouseShape = new Circle(mousePosition, 10f);
        return CollisionManager.Instance.IsCollision(collider, mouseShape);
    }

    private bool IsMouseDownWithCollider(RectCollider collider) {
        return IsMouseOverWithCollider(collider) && Input.GetMouseButtonDown(0);
    }
}
