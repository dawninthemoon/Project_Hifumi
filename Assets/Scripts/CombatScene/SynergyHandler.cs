using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SynergyHandler : IResetable {
    private KdTree<EntityBase> _activeAllies;
    private int[] _numOfSynergiesArray;
    private static readonly string SynergyConfigPath = "ScriptableObjects/Synergies";
    private Dictionary<SynergyType, SynergyConfig> _synergyConfigDictionary;

    public SynergyHandler(KdTree<EntityBase> activeAllies) {
        _numOfSynergiesArray = new int[(int)SynergyType.Count];
        _activeAllies = activeAllies;

        _synergyConfigDictionary = Resources.LoadAll<SynergyConfig>(SynergyConfigPath)
                                    .ToDictionary(x => x.Type);
    }

    public void PrintCurrentSynergies() {
        int startIndex = (int)SynergyType.None + 1;
        int endIndex = (int)SynergyType.Count;
        for (int i = startIndex; i < endIndex; ++i) {
            Debug.Log((SynergyType)i + ": " + _numOfSynergiesArray[i]);
        }
    }

    public void AddSynergy(EntityBase ally, bool increase) {
        IncreaseSynergy(ally.Synergy1, increase);
        IncreaseSynergy(ally.Synergy2, increase);
    }

    public void Reset() {
        for (int i = 0; i < (int)SynergyType.Count; ++i) {
            _numOfSynergiesArray[i] = 0;
        }
    }

    private void IncreaseSynergy(SynergyType synergy, bool increase) {
        int synergyIndex = (int)synergy;
        int increaseAmount = increase ? 1 : -1;
        _numOfSynergiesArray[synergyIndex] += increaseAmount;
    }
}
