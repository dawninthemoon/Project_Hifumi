using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class AIData : MonoBehaviour {
    public List<Transform> targets = null;
    public CustomCollider[] obstacles = null;
    public Transform currentTarget = null;

    public int TargetsCount {
        get { return targets == null ? 0 : targets.Count;}
    }
}
