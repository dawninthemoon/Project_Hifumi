using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatReward : MonoBehaviour {
    [SerializeField] private BelongingObject _belongingsPrefab = null;
    [SerializeField] private Transform _truckTransform = null;
    private Vector3[] _belongingsPosition = null;
    private BelongingObject[] _currentBelongingObjArr;
    private int _numOfRewards;

    private void Awake() {
        _belongingsPosition = new Vector3[] {
            new Vector3(-50f, 100f),
            new Vector3(0f, 100f),
            new Vector3(50f, 100f)
        };
        _currentBelongingObjArr = new BelongingObject[3];
    }

    public void OpenRewardSet(System.Action<Belongings> onRewardSelected) {
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Reward, true);
        _numOfRewards = 0;

        for (int i = 0; i < 3; ++i) {
            Belongings item = GameMain.RewardData.GetRandomItem(true);
            if (!item) {
                break;
            }
            int curr = i;

            Vector3 position = _truckTransform.position + _belongingsPosition[curr];
            BelongingObject obj = CreateBelongingObject(item, position);

            obj.SetSprite(item.Sprite);
            obj.Interactive.OnMouseDownEvent
                .AddListener(() => OnRewardSelected(curr, onRewardSelected));

            _currentBelongingObjArr[curr] = obj;
            _numOfRewards = i + 1;
        }
    }

    private BelongingObject CreateBelongingObject(Belongings belonging, Vector3 position) {
        BelongingObject obj = Instantiate(_belongingsPrefab, position, Quaternion.identity);
        obj.ItemData = belonging;
        return obj;
    }

    private void OnRewardSelected(int index, System.Action<Belongings> onRewardSelected) {
        BelongingObject item = _currentBelongingObjArr[index];
        item.gameObject.SetActive(false);
        InteractiveEntity.SetInteractive(InteractiveEntity.Type.Reward, false);
        onRewardSelected.Invoke(item.ItemData);

        for (int i = 0; i < _numOfRewards; ++i) {
            if (i != index) {
                GameMain.RewardData.AddItemData(_currentBelongingObjArr[i].ItemData);
            }
            Destroy(_currentBelongingObjArr[i].gameObject);
        }
    }
}
