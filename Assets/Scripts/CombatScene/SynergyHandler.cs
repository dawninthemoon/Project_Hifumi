using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SynergyHandler : IResetable {
    private int[] _numOfSynergiesArray;
    private static readonly string SynergyConfigPath = "ScriptableObjects/SynergyConfig";
    private Dictionary<SynergyType, SynergyConfig> _synergyConfigDictionary;
    private Dictionary<SynergyType, BuffConfig> _currentSynergyBuff;

    public SynergyHandler() {
        _numOfSynergiesArray = new int[(int)SynergyType.Count];

        _currentSynergyBuff = new Dictionary<SynergyType, BuffConfig>();
        _synergyConfigDictionary = Resources.LoadAll<SynergyConfig>(SynergyConfigPath)
                                    .ToDictionary(x => x.Type);
    }

    // (prevSynergy, currentSynergy) pair
    public (BuffConfig, BuffConfig) GetSynergyBuffPair(SynergyType type) {
        BuffConfig buff = GetSynergyBuff(type);
        if (buff == null) {
            return (null, null);
        }

        if (_currentSynergyBuff.TryGetValue(type, out BuffConfig prevBuff)) {
            if (!prevBuff.Equals(buff)) {
                _currentSynergyBuff[type] = buff;
            }
            else {
                buff = null;
            }
        }
        else {
            _currentSynergyBuff.Add(type, buff);
        }
        return (prevBuff, buff);
    }

    private BuffConfig GetSynergyBuff(SynergyType type) {
        if (!_synergyConfigDictionary.TryGetValue(type, out SynergyConfig config)) {
            return null;
        }
        BuffConfig buff = null;
        foreach (var synergy in config.Synergies) {
            if (_numOfSynergiesArray[(int)type] >= synergy.requireAllies) {
                buff = synergy.buff;
                break;
            }
        }
        return buff;
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
        _numOfSynergiesArray[synergyIndex] = Mathf.Max(_numOfSynergiesArray[synergyIndex], 0);
    }
}
