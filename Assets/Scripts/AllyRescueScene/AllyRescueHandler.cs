using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AllyRescueHandler : MonoBehaviour {
    [SerializeField] private Image[] _allyImages = null;
    [SerializeField] private Button[] _rescueButtons = null;
    private EntityInfo[] _entityInfoArray;
    private EntityInfo[] _currentAllies;

    private void Awake() {
        _entityInfoArray = Resources.LoadAll<EntityInfo>("ScriptableObjects/Allies");
        _currentAllies = new EntityInfo[3];
    }

    public void Reset() {
        var entityTable = _entityInfoArray.ToList();
        for (int i = 0; i < _rescueButtons.Length; ++i) {
            int selectedIdx = i;

            int randomEntityIndex = Random.Range(0, entityTable.Count);
            EntityInfo entityInfo = entityTable[randomEntityIndex];
            entityTable.RemoveAt(randomEntityIndex);

            _allyImages[i].transform.GetChild(0).GetComponent<Image>().sprite = entityInfo.BodySprite;
            _currentAllies[i] = entityInfo;

            _rescueButtons[i].onClick.AddListener(() => OnRescueButtonClicked(selectedIdx));
        }
    }

    private void OnRescueButtonClicked(int index) {
        for (int i = 0; i < _allyImages.Length; ++i) {
            if (i == index) {
                continue;
            }
            _allyImages[i].gameObject.SetActive(false);
        }
        GameMain.PlayerData.Allies.Add(_currentAllies[index]);
    }
}
