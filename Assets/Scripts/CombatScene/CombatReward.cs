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

    public void OpenRewardSet(System.Action<Belongings> onRewardSelected) {
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Reward, true);
        for (int i = 0; i < 3; ++i) {
            int randIdx = Random.Range(0, _belongingData.Length);
            Belongings stuff = _belongingData[i];
            Vector3 position = _truckTransform.position + _belongingsPosition[i];
            BelongingObject obj = CreateBelongingObject(stuff, position);
            obj.SetSprite(stuff.Sprite);
            obj.Interactive.OnMouseDownEvent.AddListener(() => {
                obj.gameObject.SetActive(false);
                InteractiveEntity.SetInteractive(InteractiveEntity.Type.Reward, false);
                onRewardSelected.Invoke(stuff);
            });
        }
    }

    private BelongingObject CreateBelongingObject(Belongings belonging, Vector3 position) {
        BelongingObject obj = Instantiate(_belongingsPrefab, position, Quaternion.identity);
        obj.BelongingData = belonging;
        return obj;
    }
}
