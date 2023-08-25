using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable {
    float Radius { get; }
    Vector3 Position { get; }
}