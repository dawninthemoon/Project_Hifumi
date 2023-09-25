using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShopHandler : MonoBehaviour, IResetable {
    [SerializeField] private Button _itemGachaButton;
    [SerializeField] private Button _memberGachaButton;
    private static readonly int MaxGachaCount = 3;
    private int _remainedItemGachaCount;
    private int _remainedMemberGachaCount;

    private void Awake() {
        _itemGachaButton.onClick.AddListener(OnItemGachaButtonClicked);
        _memberGachaButton.onClick.AddListener(OnMemberGachaButtonClicked);
        Reset();
    }

    public void Reset() {
        _remainedItemGachaCount = MaxGachaCount;
        _remainedMemberGachaCount = MaxGachaCount;
    }

    private void OnItemGachaButtonClicked() {
        if (_remainedItemGachaCount == 0)
            return;
        --_remainedItemGachaCount;

        Belongings item = GameMain.RewardData.GetRandomItem(true);
        if (item) {
            GameMain.PlayerData.AddItemInInventory(item);
            Debug.Log(item.name);
        }
    }

    private void OnMemberGachaButtonClicked() {
        if (_remainedMemberGachaCount == 0)
            return;
        --_remainedMemberGachaCount;

        EntityInfo member = GameMain.RewardData.GetRandomAlly(true);
        if (member) {
            GameMain.PlayerData.AddMember(member);
            Debug.Log(member.name);
        }
    }
}
