using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggle : MonoBehaviour {
    public void Toggle() {
        bool oppositeState = !gameObject.activeSelf;
        gameObject.SetActive(oppositeState);
    }
}
