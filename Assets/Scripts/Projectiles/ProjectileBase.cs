using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour {
    public abstract void Initialize(Transform target, float moveSpeed);
    protected abstract void Update();
}
