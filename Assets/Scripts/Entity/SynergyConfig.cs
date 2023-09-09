using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SynergyType {
    None,
    SMG,
    Handgun,
    Shotgun,
    Sniper,
    Grenade,
    Flame,
    AcademyA,
    AcademyB,
    AcademyC,
    AcademyD,
    Count
}

[CreateAssetMenu(menuName = "ScriptableObjects/SynergyConfig", fileName = "NewSynergy")]
public class SynergyConfig : ScriptableObject {
    [System.Serializable]
    public struct SynergyPair {
        public int requireAllies;
        public BuffConfig buff;
    }
    [SerializeField] private SynergyType _type;
    [SerializeField] private SynergyPair[] _synergies;
    public SynergyType Type {
        get { return _type; }
    }
    public SynergyPair[] Synergies {
        get { return _synergies; }
    }
}