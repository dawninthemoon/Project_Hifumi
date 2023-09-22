using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ObserverSubject {
    [SerializeField] private List<EntityInfo> _allies;
    [SerializeField] private List<Belongings> _belongingsInventory;
    private Dictionary<string, List<Belongings>> _belongingsDictionary;

    public List<EntityInfo> Allies { get { return _allies; } }
    public List<Belongings> BelongingsInventory { get { return _belongingsInventory; } }
    public Dictionary<string, List<Belongings>> BelongingsDictionary { get { return _belongingsDictionary; } }

    private void Awake() {
        if (_allies == null)
            _allies = new List<EntityInfo>();
        if (_belongingsInventory == null)
            _belongingsInventory = new List<Belongings>();
        _belongingsDictionary = new Dictionary<string, List<Belongings>>();
    }

    public void AddAlly(EntityInfo ally) {
        _allies.Add(ally);
    }

    public void RemoveAlly(EntityInfo ally) {
        _allies.Remove(ally);
    }

    public void RemoveRandomAlly() {
        int randomIndex = Random.Range(0, _allies.Count);
        _allies.RemoveAt(randomIndex);
    }

    public List<Belongings> GetBelongingsList(string entityID) {
        if (!_belongingsDictionary.TryGetValue(entityID, out List<Belongings> belongingsList)) {
            belongingsList = new List<Belongings>();
            _belongingsDictionary.Add(entityID, belongingsList);
        }
        return belongingsList;
    }

    public void AddBelongings(string entityID, Belongings belongings) {
        if (!_belongingsDictionary.TryGetValue(entityID, out List<Belongings> belongingsList)) {
            _belongingsDictionary.Add(entityID, new List<Belongings>());
        }
        _belongingsDictionary[entityID].Add(belongings);
        Notify();
    }

    public void AddBelongingsInInventory(Belongings belongings) {
        _belongingsInventory.Add(belongings);
    }

    public void RemoveBelongingsInInventory(Belongings belongings) {
        _belongingsInventory.Remove(belongings);
    }

    public void UnequipBelongings(string entityID, Belongings belongings) {
        if (!_belongingsDictionary.TryGetValue(entityID, out List<Belongings> belongingsList)) {
            Debug.LogError("Belongings Not Exists!");
            return;
        }
        belongingsList.Remove(belongings);
        AddBelongingsInInventory(belongings);
        Notify();
    }
}
