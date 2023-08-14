using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics;

public class TestCode : MonoBehaviour {
    CircleCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<CircleCollider>();
        collider.OnCollisionEvent.AddListener((c1, c2) => {
            Debug.Log("Collision!" + Time.time);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
