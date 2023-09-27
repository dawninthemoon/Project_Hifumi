using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ObserverSubject {
    [SerializeField, Tooltip("For Test Code")] private List<EntityInfo> _allies;
    [SerializeField, Tooltip("For Test Code")] private List<Belongings> _belongingsInventory;
    [SerializeField] private int _defaultMaxMemberCount = 5;
    private Dictionary<string, List<Belongings>> _itemInventory;
    public List<EntityDecorator> Member { 
        get;
        private set;
    }
    public int MaximumMemberCount {
        get;
        set;
    }
    public int Gold {
        get;
        private set;
    }
    public List<Belongings> BelongingsInventory { get { return _belongingsInventory; } }
    public Dictionary<string, List<Belongings>> BelongingsDictionary { get { return _itemInventory; } }

    private void Awake() {
        MaximumMemberCount = _defaultMaxMemberCount;
        InitializeMember();

        if (_belongingsInventory == null)
            _belongingsInventory = new List<Belongings>();
        _itemInventory = new Dictionary<string, List<Belongings>>();
    }

    private void InitializeMember() {
        Member = new List<EntityDecorator>();
        if (_allies == null) {
            return;
        }

        foreach (EntityInfo entity in _allies) {
            EntityDecorator decorator = new EntityDecorator(entity);
            Member.Add(decorator);
        }
    }

    public void AddMember(EntityInfo entity) {
        Member.Add(new EntityDecorator(entity));
        Notify();
    }

    public void RemoveRandomMember() {
        int randomIndex = Random.Range(0, Member.Count);
        Member.RemoveAt(randomIndex);
        Notify();
    }

    public void AddItemInInventory(Belongings belongings) {
        _belongingsInventory.Add(belongings);
        Notify();
    }

    public void RemoveItemInInventory(Belongings belongings) {
        _belongingsInventory.Remove(belongings);
        Notify();
    }

    public void AddGold(int amount) {
        Gold += amount;
    }
}
