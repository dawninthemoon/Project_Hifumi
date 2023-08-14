using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentTest : MonoBehaviour{
    public static Stack<Vector2> ScentTrail = new Stack<Vector2>();

    private void Awake() {
        InvokeRepeating("Progress", 0f, 0.3f);
    }

    private void Progress() {
        AddScent(transform.position);
    }

    public void AddScent(Vector2 position) {
        ScentTrail.Push(position);
    }
}
