using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScent {
    private static readonly int MaxScentCount = 200;
    public List<Vector2> ScentTrail { 
        get;
        private set;
    }

    public AgentScent() {
        ScentTrail = new List<Vector2>();
    }

    public void Reset() {
        ScentTrail.Clear();
    }

    public void AddScent(Vector2 position) {
        if (ScentTrail.Count > 0 && position.Equals(ScentTrail[0])) return;

        ScentTrail.Insert(0, position);
        if (ScentTrail.Count > MaxScentCount) {
            ScentTrail.RemoveAt(MaxScentCount);
        }
    }
}
