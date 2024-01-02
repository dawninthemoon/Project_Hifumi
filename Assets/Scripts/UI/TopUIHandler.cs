using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopUIHandler : MonoBehaviour {
    [SerializeField] private TMP_Text _goldText;
    
    void Update() {
        _goldText.text = GameMain.PlayerData.Gold.ToString();
    }
}
