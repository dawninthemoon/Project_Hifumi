using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RewardData {
    private HashSet<EntityInfo> _appearableAlliesSet;
    private HashSet<Belongings> _appearableItemsSet;

    public RewardData(IList<EntityInfo> allyList, IList<Belongings> itemList) {
        _appearableAlliesSet = allyList.ToHashSet();
        _appearableItemsSet = itemList.ToHashSet();

        foreach (var playerAlly in GameMain.PlayerData.Member) {
            if (_appearableAlliesSet.Contains(playerAlly.Info)) {
                _appearableAlliesSet.Remove(playerAlly.Info);
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
