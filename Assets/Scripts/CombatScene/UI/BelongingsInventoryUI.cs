using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BelongingsInventoryUI : MonoBehaviour {
    [SerializeField] private RectTransform _inventoryParent = null;
    private Image[] _inventoryElementArray;
    private int _inventorySize;
    private void Awake() {
        _inventorySize = _inventoryParent.childCount;
        _inventoryElementArray = new Image[_inventorySize];
        for (int i = 0; i < _inventorySize; ++i) {
            Transform inventorySpace = _inventoryParent.GetChild(i);
            _inventoryElementArray[i] = inventorySpace.GetChild(0).GetComponent<Image>();
        }
    }

    private void Update() {
        List<Belongings> unequipedBelongings = GameMain.PlayerData.UnequipedBelongings;
        for (int i = 0; i < _inventorySize; ++i) {
            if (i < unequipedBelongings.Count) {
                _inventoryElementArray[i].sprite = unequipedBelongings[i].Sprite;
                _inventoryElementArray[i].gameObject.SetActive(true);
            }
            else {
                _inventoryElementArray[i].gameObject.SetActive(false);
            }
        }
    }
}
