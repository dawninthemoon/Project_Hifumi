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
    private HashSet<EntityInfo> _appearableAlliesSet;

    private void Awake() {
        _appearableAlliesSet = Resources.LoadAll<EntityInfo>("ScriptableObjects/Allies")
                                .ToHashSet();
        _currentAllies = new EntityInfo[3];
    }

    public void InitializeAllies() {
        foreach (EntityInfo playerAlly in GameMain.PlayerData.Allies) {
            if (_appearableAlliesSet.Contains(playerAlly)) {
                _appearableAlliesSet.Remove(playerAlly);
            }
        }

        for (int i = 0; (i < _rescueButtons.Length) && (_appearableAlliesSet.Count > 0); ++i) {
            int selectedIdx = i;

            EntityInfo entityInfo = GetRandomEntityFromSet();
            _appearableAlliesSet.Remove(entityInfo);

            _allyImages[i].sprite = entityInfo.BodySprite;
            _currentAllies[i] = entityInfo;

            _rescueButtons[i].onClick.AddListener(() => OnRescueButtonClicked(selectedIdx));
        }
    }

    public void Reset() {
        for (int i = 0; i < _rescueButtons.Length; ++i) {
            _rescueButtons[i].onClick.RemoveAllListeners();
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
            if (_currentAllies[i] != null)
                _appearableAlliesSet.Add(_currentAllies[i]);
        }
        GameMain.PlayerData.Allies.Add(_currentAllies[selectedIndex]);
    }

    private EntityInfo GetRandomEntityFromSet() {
        int numOfEntities = _appearableAlliesSet.Count;
        int index = Random.Range(0, numOfEntities);
        return _appearableAlliesSet.ElementAt(index);
    }
}
