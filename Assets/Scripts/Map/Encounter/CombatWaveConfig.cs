using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CombatWaveInfo {
    public string waveID;
    public string[] enemyIDArray;
}

[CreateAssetMenu(menuName = "ScriptableObjects/CombatWaveConfig", fileName = "NewWaveConfig")]
public class CombatWaveConfig : ScriptableObject {
    [SerializeField] private int _waveRank = 1;
    [SerializeField] private CombatWaveInfo[] _waveInfoArray;
    public int WaveRank { get { return _waveRank; } }
}
