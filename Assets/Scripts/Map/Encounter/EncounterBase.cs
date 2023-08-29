using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterBase : MonoBehaviour, IResetable {
    public abstract void OnEncounter();
    public abstract void Reset();
}
