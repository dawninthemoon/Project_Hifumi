using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShopHandler : MonoBehaviour {
    [SerializeField] private Button[] _belongingButtons = null;
    private Belongings[] _belongingData;
    private Dictionary<int, bool> _itemSoldCheckDictionary;

    private void Awake() {
        _belongingData = Resources.LoadAll<Belongings>("ScriptableObjects/Belongings");
        _itemSoldCheckDictionary = new Dictionary<int, bool>();
    }

    private void Start() {
        List<Belongings> belongingsTable = _belongingData.ToList();
        for (int i = 0; i < _belongingButtons.Length; ++i) {
            int id = i;

            int randomBelongingIdx = Random.Range(0, belongingsTable.Count);
            Belongings data = belongingsTable[randomBelongingIdx];
            belongingsTable.RemoveAt(randomBelongingIdx);

            _itemSoldCheckDictionary.Add(id, false);
            _belongingButtons[i].image.sprite = data.Sprite;
            _belongingButtons[i].onClick.AddListener(() => OnButtonClicked(id));
        }
    }

    private void OnButtonClicked(int id) {
        if (_itemSoldCheckDictionary[id]) {
            return;
        }
        _belongingButtons[id].gameObject.SetActive(false);
    }
}
