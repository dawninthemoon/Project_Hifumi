using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEncounter : EncounterBase, IResetable {
    [SerializeField] private CombatSceneHandler _combatHandler = null;
    [SerializeField] private TruckDirectionSelect _truckMover = null;
    private Dictionary<int, CombatStageConfig[]> _combatStageConfigDic;

    private void Awake() {
        _combatStageConfigDic = new Dictionary<int, CombatStageConfig[]>();

        int maxRank = 3;
        for (int rank = 1; rank <= maxRank; ++rank) {
            var combatStageConfigArray = Resources.LoadAll<CombatStageConfig>("ScriptableObjects/CombatStageConfig/Rank" + rank.ToString());
            if (combatStageConfigArray != null) {
                _combatStageConfigDic.Add(rank, combatStageConfigArray);
            }
        }
    }

    public override void OnEncounter() {
        _combatHandler.StartCombat(GetRandomStage(1));
        gameObject.SetActive(true);
    }

    public override void Reset() {
        _combatHandler.Reset();
        _truckMover.Reset();
        ProjectileSpawner.Instance.RemoveAllProjectiles();
    }

    private CombatStageConfig GetRandomStage(int rank) {
        CombatStageConfig[] targetStageArray = _combatStageConfigDic[rank];
        int randomIndex = Random.Range(0, targetStageArray.Length);
        return targetStageArray[randomIndex];
    }
}
