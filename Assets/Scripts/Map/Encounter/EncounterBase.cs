using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EncounterBase : MonoBehaviour, IResetable {
    public abstract void Initialize(UnityAction roomExitCallback);
    public abstract void OnEncounter();
    public abstract void Reset();
}
