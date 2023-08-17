using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable {
    Vector3 Position { get; }
    List<Vector2> GetScentTrail();
}