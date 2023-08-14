using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class AIData : MonoBehaviour {
    public List<Transform> targets = null;
    public Vector2[] closestPointWithObstacles = null;
    public Transform currentTarget = null;

    public int TargetsCount {
        get { return targets == null ? 0 : targets.Count;}
    }
}
