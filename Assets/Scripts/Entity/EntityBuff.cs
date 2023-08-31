using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class EntityBuff {
    private MonoBehaviour _executer;
    private EntityStatusDecorator _status;
    private HashSet<string> _currentBuffSet;

    public EntityBuff(MonoBehaviour executer, EntityStatusDecorator status) {
        _executer = executer;
        _status = status;
    }

    public void StartApplyBuff(BuffConfig buffConfig) {
        BuffInfo info = buffConfig.Info;

        if (info.stun.value) {
            _executer.StartCoroutine(ApplyBuff(nameof(info.stun.value), info.stun.durtaion));
        }
        
        _executer.StartCoroutine(ApplyBuff(buffConfig, buffConfig.Info.statusBuffDuration));
    }

    private IEnumerator ApplyBuff(BuffConfig buff, float duration) {
        _status.AddBuff(buff);

        yield return YieldInstructionCache.WaitForSeconds(duration);

        _status.RemoveBuff(buff);
    }

    private IEnumerator ApplyBuff(string buffName, float duration) {
        _currentBuffSet.Add(buffName);

        yield return YieldInstructionCache.WaitForSeconds(duration);

        _currentBuffSet.Remove(buffName);
    }
}
