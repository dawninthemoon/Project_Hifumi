using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class EntityBuff {
    private MonoBehaviour _executer;
    private EntityStatusDecorator _status;
    private HashSet<string> _currentDebuffSet;

    public EntityBuff(MonoBehaviour executer, EntityStatusDecorator status) {
        _currentDebuffSet = new HashSet<string>();
        _executer = executer;
        _status = status;
    }

    public void AddBuff(BuffConfig buff) {
        _status.AddBuff(buff);
    }

    public void RemoveBuff(BuffConfig buff) {
        _status.RemoveBuff(buff);
    }

    public void AddBuffWithDuration(BuffConfig buffConfig) {
        _executer.StartCoroutine(AddBuff(buffConfig, buffConfig.Info.buffDuration));
    }

    public void StartAddDebuff(DebuffConfig debuffConfig) {
        DebuffInfo info = debuffConfig.Info;

        if (info.stun.value) {
            _executer.StartCoroutine(AddDebuff(nameof(info.stun), info.stun.durtaion));
        }
    }

    private IEnumerator AddBuff(BuffConfig buff, float duration) {
        AddBuff(buff);

        yield return YieldInstructionCache.WaitForSeconds(duration);

        RemoveBuff(buff);
    }

    private IEnumerator AddDebuff(string debuffName, float duration) {
        _currentDebuffSet.Add(debuffName);

        yield return YieldInstructionCache.WaitForSeconds(duration);

        _currentDebuffSet.Remove(debuffName);
    }

    public bool IsDebuffExists(string debuffName) {
        if (_currentDebuffSet.TryGetValue(debuffName, out string value)) {
            return true;
        }
        return false;
    }
}
