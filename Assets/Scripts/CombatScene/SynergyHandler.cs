using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyHandler : IResetable {
    private KdTree<EntityBase> _activeAllies;
    private int[] _numOfSynergiesArray;

    public SynergyHandler(KdTree<EntityBase> activeAllies) {
        _activeAllies = activeAllies;
        _numOfSynergiesArray = new int[(int)EntitySynergy.Count];
    }

    public void PrintCurrentSynergies() {
        int startIndex = (int)EntitySynergy.None + 1;
        int endIndex = (int)EntitySynergy.Count;
        for (int i = startIndex; i < endIndex; ++i) {
            Debug.Log((EntitySynergy)i + ": " + _numOfSynergiesArray[i]);
        }
    }

    public void AddSynergy(EntityBase ally, bool increase) {
        IncreaseSynergy(ally.Synergy1, increase);
        IncreaseSynergy(ally.Synergy2, increase);
    }

    public void Reset() {
        for (int i = 0; i < (int)EntitySynergy.Count; ++i) {
            _numOfSynergiesArray[i] = 0;
        }
    }

    private void IncreaseSynergy(EntitySynergy synergy, bool increase) {
        int synergyIndex = (int)synergy;
        int increaseAmount = increase ? 1 : -1;
        _numOfSynergiesArray[synergyIndex] += increaseAmount;
    }
}
