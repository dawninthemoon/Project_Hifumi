using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CombatStageConfig", fileName = "NewStageConfig")]
public class CombatStageConfig : ScriptableObject {
    [SerializeField] private int[] _stageInfoArray;
    public int[] StageInfoArray { 
        get { return _stageInfoArray; }
    }

    public int GetStageLength() {
        return _stageInfoArray.Length;
    }
}
