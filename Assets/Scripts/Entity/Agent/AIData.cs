using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class AIData : MonoBehaviour {
    public Collider2D[] obstacles = null;
    public Agent selectedTarget = null;
    public Transform currentTarget = null;
}
