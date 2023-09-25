using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggle : MonoBehaviour {
    public void ToggleSelf() {
        Toggle(gameObject);
    }

    public void Toggle(GameObject obj) {
        bool oppositeState = !gameObject.activeSelf;
        obj.SetActive(oppositeState);
    }
}
