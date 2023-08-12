using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;
using RieslingUtils;

public class MemberUIElement {
    public GameObject gameObject;
    public UICollider collider;
    public MemberUIElement(GameObject gameObject, UICollider collider) {
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
    private UICollider _zoneCollider;

    private void Awake() {
        _currentMemberUI = new List<MemberUIElement>();
        _zoneCollider = GetComponent<UICollider>();

        _zoneCollider.OnMouseDown.AddListener(() => {  SetUIActive(!_additionalWindow.gameObject.activeSelf); });

        _entityPrefabDictionary = new Dictionary<string, EntityBase>();
        var entityPrefabs = Resources.LoadAll<EntityBase>("Prefabs/");
        foreach (EntityBase entity in entityPrefabs) {
            _entityPrefabDictionary.Add(entity.ID, entity);
            CreateUIElement(entity);
        }

        SetUIActive(false);
    }

    private void Update() {
        Vector2 mousePosition = MouseUtils.GetMouseWorldPosition();
        if (_selectedEntity) {
            _selectedEntity.transform.position = mousePosition;
            if (Input.GetMouseButtonUp(0)) {
                SpawnEntity();
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

    private void CreateUIElement(EntityBase target) {
        var uiObj = Instantiate(_memberUIPrefab, _additionalWindow);
        var collider = uiObj.GetComponentInChildren<UICollider>();

        uiObj.transform.localPosition = GetUIPosition(_currentMemberUI.Count);
        uiObj.transform.SetParent(null);
        uiObj.transform.localScale = Vector3.one;

        MemberUIElement element = new MemberUIElement(uiObj, collider);
        _currentMemberUI.Add(element);

        collider.OnMouseOver.AddListener(() => {
            uiObj.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        });
        collider.OnMouseDown.AddListener(() => {
            Vector3 mousePosition = MouseUtils.GetMouseWorldPosition().ChangeZPos(0f);
            _selectedUI = element;
            _selectedEntity = Instantiate(_entityPrefabDictionary[target.ID], mousePosition, Quaternion.identity);
        });
        collider.OnMouseExit.AddListener(() => {
            if (_selectedUI != element) {
                uiObj.transform.localScale = Vector3.one;
            }
        });
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
}
