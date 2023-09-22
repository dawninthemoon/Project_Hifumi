using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AllyRescueHandler : MonoBehaviour, IResetable {
    [SerializeField] private Image[] _allyImages = null;
    [SerializeField] private Button[] _rescueButtons = null;
    private EntityInfo[] _entityInfoArray;
    private EntityInfo[] _currentAllies;

    private void Awake() {
        _currentAllies = new EntityInfo[3];
    }

    public void InitializeAllies() {
        for (int i = 0; i < _rescueButtons.Length; ++i) {
            int selectedIdx = i;

            EntityInfo entityInfo = GameMain.RewardData.GetRandomAlly(true);
            if (!entityInfo) {
                break;
            }

            _allyImages[i].sprite = entityInfo.BodySprite;
            _currentAllies[i] = entityInfo;

            _rescueButtons[i].onClick.AddListener(() => OnRescueButtonClicked(selectedIdx));
        }
    }

    public void Reset() {
        for (int i = 0; i < _rescueButtons.Length; ++i) {
            _rescueButtons[i].onClick.RemoveAllListeners();
            _rescueButtons[i].interactable = true;
            _rescueButtons[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < _allyImages.Length; ++i) {
            _allyImages[i].sprite = null;
        }
        for (int i = 0; i < _currentAllies.Length; ++i) {
            _currentAllies[i] = null;
        }
    }

    private void OnRescueButtonClicked(int selectedIndex) {
        for (int i = 0; i < _allyImages.Length; ++i) {
            if (i == selectedIndex) {
                continue;
            }
            _rescueButtons[i].gameObject.SetActive(false);
            if (_currentAllies[i] != null) {
                GameMain.RewardData.AddAllyData(_currentAllies[i]);
            }
        }
        _rescueButtons[selectedIndex].interactable = false;
        GameMain.PlayerData.Allies.Add(_currentAllies[selectedIndex]);
    }
}
