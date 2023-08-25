using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatReward : MonoBehaviour {
    [SerializeField] private BelongingObject _belongingsPrefab = null;
    [SerializeField] private Transform _truckTransform = null;
    private Vector3[] _belongingsPosition = null;
    private Belongings[] _belongingData;
    private void Awake() {
        _belongingData = Resources.LoadAll<Belongings>("ScriptableObjects/Belongings");
        _belongingsPosition = new Vector3[] {
            new Vector3(-50f, 100f),
            new Vector3(0f, 100f),
            new Vector3(50f, 100f)
        };
    }

    public void OpenRewardSet() {
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Reward, true);
        for (int i = 0; i < 3; ++i) {
            int randIdx = Random.Range(0, _belongingData.Length);
            Vector3 position = _truckTransform.position + _belongingsPosition[i];
            BelongingObject obj = CreateBelongingObject(_belongingData[i], position);
            obj.SetSprite(_belongingData[i].Sprite);
            obj.Interactive.OnMouseDownEvent.AddListener(() => {
                obj.gameObject.SetActive(false);
                InteractiveEntity.SetInteractive(InteractiveEntity.Type.Reward, false);
            });
        }
    }

    private BelongingObject CreateBelongingObject(Belongings belonging, Vector3 position) {
        BelongingObject obj = Instantiate(_belongingsPrefab, position, Quaternion.identity);
        obj.BelongingData = belonging;
        return obj;
    }
}
