using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class AIData {
    public List<Collider2D> Obstacles {
        get;
        set;
    }
    public ITargetable SelectedTarget {
        get;
        set;
    }
    public Transform CurrentTarget {
        get;
        set;
    }
    public float AttackRange {
        get;
        set;
    }
    public float Radius {
        get;
        set;
    }
}
