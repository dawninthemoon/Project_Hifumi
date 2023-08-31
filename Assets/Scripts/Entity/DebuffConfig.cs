using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DebuffInfo {
    [System.Serializable]
    public struct Pair<T> {
        public T value;
        public float durtaion;
    }
    public Pair<bool> stun;
}

[CreateAssetMenu(fileName = "NewDebuffConfig", menuName = "ScriptableObjects/DebuffConfig")]
public class DebuffConfig : ScriptableObject {
    [SerializeField] private DebuffInfo _debuffInfo;
    public DebuffInfo Info {
        get { return _debuffInfo; }
    }
}