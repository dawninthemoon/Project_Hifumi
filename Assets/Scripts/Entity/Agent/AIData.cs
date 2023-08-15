using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class AIData : MonoBehaviour {
    public float attackDistance {
        get;
        set;
    }
    public Collider2D[] Obstacles {
        get;
        set;
    }
    public Agent SelectedTarget {
        get;
        set;
    }
    public Transform CurrentTarget {
        get;
        set;
    }
}
