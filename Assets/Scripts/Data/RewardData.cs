using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RewardData {
    private static readonly string SOPath = "ScriptableObjects/";
    private static readonly string AllyDataPath = "Allies/";
    private static readonly string ItemDataPath = "Items/";
    private HashSet<EntityInfo> _appearableAlliesSet;
    private HashSet<Belongings> _appearableItemsSet;
    public RewardData(PlayerData playerData) {
        _appearableAlliesSet 
            = Resources.LoadAll<EntityInfo>(SOPath + AllyDataPath).ToHashSet();
        _appearableItemsSet 
            = Resources.LoadAll<Belongings>(SOPath + ItemDataPath).ToHashSet();

        foreach (EntityInfo playerAlly in GameMain.PlayerData.Allies) {
            if (_appearableAlliesSet.Contains(playerAlly)) {
                _appearableAlliesSet.Remove(playerAlly);
            }
        }
    }

    public void AddItemData(Belongings item) {
        _appearableItemsSet.Add(item);
    }

    public void RemoveItemData(Belongings item) {
        _appearableItemsSet.Remove(item);
    }

    public void AddAllyData(EntityInfo ally) {
        _appearableAlliesSet.Add(ally);
    }

    public void RemoveAllyData(EntityInfo ally) {
        _appearableAlliesSet.Remove(ally);
    }

    public Belongings GetRandomItem(bool pop = false) {
        Belongings item = GetRandomElementFromSet<Belongings>(_appearableItemsSet);
        if (item && pop) {
            _appearableItemsSet.Remove(item);
        }
        return item;
    }

    public EntityInfo GetRandomAlly(bool pop = false) {
        EntityInfo ally = GetRandomElementFromSet<EntityInfo>(_appearableAlliesSet);
        if (ally && pop) {
            _appearableAlliesSet.Remove(ally);
        }
        return ally;
    }

    private T GetRandomElementFromSet<T>(HashSet<T> set) where T : class {
        int numOfElement = set.Count;
        if (numOfElement == 0)
            return null;

        int index = Random.Range(0, numOfElement);
        return set.ElementAt(index);
    }
}
