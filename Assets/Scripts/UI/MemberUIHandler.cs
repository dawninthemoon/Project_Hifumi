using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberUIHandler : MonoBehaviour, IObserver {
    [SerializeField] private Transform _memberScrollviewContent;
    [SerializeField] private MemberUIElement _memberInfoPrefab;
    private ObjectPool<MemberUIElement> _memberUIPool;

    private void Awake() {
        _memberUIPool = new ObjectPool<MemberUIElement>(
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

        foreach (EntityDecorator entityDecorator in GameMain.PlayerData.Member) {
            var memberUIElement = _memberUIPool.GetObject();
            memberUIElement.SetTargetEntity(entityDecorator);
            memberUIElement.UpdateMemberInfo();
        }
    }
}
