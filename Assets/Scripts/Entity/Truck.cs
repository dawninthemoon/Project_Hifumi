using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : EntityBase, ITargetable {
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _height = 1f;
    public float Width {
        get { return _width; }
    }
    public float Height {
        get { return _height; }
    }
    public Vector3 Position {
        get { return transform.position; }
    }
    public List<Vector2> GetScentTrail() {
        return new List<Vector2>();
    }

    private void Awake() {
        
    }
}
