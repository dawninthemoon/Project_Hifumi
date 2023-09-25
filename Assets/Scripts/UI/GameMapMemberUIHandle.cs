using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapMemberUIHandle : MonoBehaviour, IObserver {
    [SerializeField] private Transform _memberScrollviewContent;
    [SerializeField] private GameMapMemberUIElement _memberInfoPrefab;
    private ObjectPool<GameMapMemberUIElement> _memberUIPool;

    private void Awake() {
        _memberUIPool = new ObjectPool<GameMapMemberUIElement>(
            10,
            () => Instantiate(_memberInfoPrefab, _memberScrollviewContent),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );
        GameMain.PlayerData.Attach(this);
    }

    private void Start() {
        StartCoroutine(StartInitializeMemberUI());
    }

    private IEnumerator StartInitializeMemberUI() {
        yield return new WaitUntil(() => GameMain.IsLoadCompleted);

        Initialize();
    }

    private void Initialize() {
        UpdateMemberUIElements();
    }

    public void Notify(ObserverSubject subject) {
        UpdateMemberUIElements();
    }

    private void UpdateMemberUIElements() {
        _memberUIPool.Clear();

        foreach (EntityInfo entityInfo in GameMain.PlayerData.Allies) {
            var memberUIElement = _memberUIPool.GetObject();
            memberUIElement.SetEntityInfo(entityInfo);
            memberUIElement.UpdateMemberInfo();
        }
    }
}
