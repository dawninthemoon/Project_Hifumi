using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIHandler : MonoBehaviour, IObserver {
    private void Awake() {
        GameMain.PlayerData.Attach(this);
    }
    
    public void Notify(ObserverSubject subject) {

    }
}
