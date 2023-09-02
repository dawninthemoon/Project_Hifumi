using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {
    public List<EntityInfo> Allies { get; private set; }
    public Dictionary<string, List<Belongings>> BelongingsDictionary { get; private set; }
    public List<Belongings> UnequipedBelongings { get; private set; }

    public PlayerData() {
        Allies = new List<EntityInfo>();
        BelongingsDictionary = new Dictionary<string, List<Belongings>>();
        UnequipedBelongings = new List<Belongings>();
    }

    public List<Belongings> GetBelongingsList(string entityID) {
        if (!BelongingsDictionary.TryGetValue(entityID, out List<Belongings> belongingsList)) {
            belongingsList = new List<Belongings>();
            BelongingsDictionary.Add(entityID, belongingsList);
        }
        return belongingsList;
    }

    public void AddBelongings(string entityID, Belongings belongings) {
        if (!BelongingsDictionary.TryGetValue(entityID, out List<Belongings> belongingsList)) {
            BelongingsDictionary.Add(entityID, new List<Belongings>());
        }
        BelongingsDictionary[entityID].Add(belongings);
    }

    public void AddBelongingsInInventory(Belongings belongings) {
        UnequipedBelongings.Add(belongings);
    }

    public void RemoveBelongingsInInventory(Belongings belongings) {
        UnequipedBelongings.Remove(belongings);
    }

    public void UnequipBelongings(string entityID, Belongings belongings) {
        if (!BelongingsDictionary.TryGetValue(entityID, out List<Belongings> belongingsList)) {
            Debug.LogError("Belongings Not Exists!");
            return;
        }
        belongingsList.Remove(belongings);
        AddBelongingsInInventory(belongings);
    }
}
