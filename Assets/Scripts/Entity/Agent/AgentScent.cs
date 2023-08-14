using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScent : MonoBehaviour {
    private static readonly int MaxScentCount = 20;
    public List<Vector2> ScentTrail { 
        get;
        private set;
    }

    private void Awake() {
        ScentTrail = new List<Vector2>();
        InvokeRepeating("Progress", 0f, 0.3f);
    }

    private void Progress() {
        AddScent(transform.position);
    }

    public void AddScent(Vector2 position) {
        if (ScentTrail.Count > 0 && !position.Equals(ScentTrail[0])) return;

        ScentTrail.Insert(0, position);
        if (ScentTrail.Count > MaxScentCount) {
            ScentTrail.RemoveAt(MaxScentCount);
        }
    }
}
