using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExTimeCounter {
    private Dictionary<string, float> _timerSet = new Dictionary<string, float>();
    private Dictionary<string, float> _timeLimitSet = new Dictionary<string, float>();

    public static bool _isUpdate;

    public float InitTimer(string target, float time = 0f, float timeLimit = 1f) {
        if (!_timerSet.ContainsKey(target)) {
            _timerSet.Add(target, time);
            _timeLimitSet.Add(target, timeLimit);
        }
        else {
            _timerSet[target] = time;
            _timeLimitSet[target] = timeLimit;
        }

        return time;
    }

    public float IncreaseTimerSelf(string target, out bool overLimit, float factor) {
        float limit = _timeLimitSet[target];
        return IncreaseTimerSelf(target, limit, out overLimit,factor);
    }

    public float IncreaseTimerSelf(string target, float limit, out bool overLimit, float factor) {
        if (!_timerSet.ContainsKey(target)) {
            _timerSet.Add(target, 0f);
        }

        var curr = _timerSet[target] += factor;
        overLimit = false;

        if (curr >= limit) {
            curr = limit;
            overLimit = true;
        }

        return curr;
    }

    public float IncreaseTimer(string target, out bool overLimit, float timeScale = 1f) {
        float limit = _timeLimitSet[target];
        return IncreaseTimer(target, limit, out overLimit, timeScale);
    }

    public float IncreaseTimer(string target, float limit, out bool overLimit, float timeScale = 1f) {
        if (!_timerSet.ContainsKey(target)) {
            _timerSet.Add(target, 0f);
        }

        var curr = _timerSet[target] += timeScale * GetDeltaTime();
        overLimit = false;

        if(curr >= limit) {
            curr = limit;
            overLimit = true;
        }

        return curr;
    }

    public float DecreaseTimer(string target, float limit, out bool overLimit) {
        if(!_timerSet.ContainsKey(target)) {
            _timerSet.Add(target, 0f);
        }

        var curr = _timerSet[target] -= GetDeltaTime();
        overLimit = false;
        
        if(curr <= limit) {
            curr = limit;
            overLimit = true;
        }

        return curr;
    }

    public float GetCurrentTime(string target) {
        return _timerSet[target];
    }

    public float GetTimeLimit(string target) {
        if (_timeLimitSet.ContainsKey(target)) {
            return _timeLimitSet[target];
        }
        else {
            return -1f;
        }
    }

    public float GetDeltaTime() {
        return _isUpdate ? Time.deltaTime : Time.fixedDeltaTime;
    }

    public bool Contains(string target) {
        return _timerSet.ContainsKey(target);
    }
}