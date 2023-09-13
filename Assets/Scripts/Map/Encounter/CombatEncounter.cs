using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEncounter : EncounterBase, IResetable, ILoadable {
    [SerializeField] private CombatSceneHandler _combatHandler = null;
    [SerializeField] private TruckDirectionSelect _truckMover = null;
    private Dictionary<int, List<CombatStageConfig>> _combatStageConfigDic;
    private static readonly string CombatConfigKey = "CombatConfig";
    public static bool IsLoadCompleted {
        get;
        private set;
    } = false;

    private void Awake() {
        _combatStageConfigDic = new Dictionary<int, List<CombatStageConfig>>();
        AssetManager.Instance.LoadAssetsAsync<CombatStageConfig>(
            CombatConfigKey,
            (handle) => {
                IList<CombatStageConfig> stageConfigs = handle.Result;
                foreach (CombatStageConfig config in stageConfigs) {
                    if (_combatStageConfigDic.TryGetValue(config.StageRank, out var list)) {
                        list.Add(config);
                    }
                    else {
                        List<CombatStageConfig> stageList = new List<CombatStageConfig>();
                        stageList.Add(config);
                        _combatStageConfigDic.Add(config.StageRank, stageList);
                    }
                }

                IsLoadCompleted = true;
            }
        );
    }

    public override void OnEncounter() {
        _combatHandler.StartCombat(GetRandomStage(1));
        gameObject.SetActive(true);
    }

    public override void Reset() {
        _combatHandler.Reset();
        _truckMover.Reset();
        CombatMap.SetMapView(Vector3.zero);
        ProjectileSpawner.Instance.RemoveAllProjectiles();
    }

    private CombatStageConfig GetRandomStage(int rank) {
        List<CombatStageConfig> targetStageList = _combatStageConfigDic[rank];
        int randomIndex = Random.Range(0, targetStageList.Count);
        return targetStageList[randomIndex];
    }
}
