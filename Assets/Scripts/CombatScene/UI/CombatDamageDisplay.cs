using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RieslingUtils;

public class CombatDamageDisplay : MonoBehaviour, IObserver {
    private static readonly string PrefabPath = "DamageDisplayText";
    private ObjectPool<TMP_Text> _damageTextPool;

    private void Awake() {
        AssetLoader.Instance.LoadAssetAsync<GameObject>(PrefabPath, (handle) => {
            var prefab = handle.Result.GetComponent<TMP_Text>();
            _damageTextPool = new ObjectPool<TMP_Text>(
                10,
                () => Instantiate(prefab),
                (x) => x.gameObject.SetActive(true),
                (x) => x.gameObject.SetActive(false)
            );
        });
    }

    public void Notify(ObserverSubject subject) {
        DamageInfo damageInfo = subject as DamageInfo;
        if (damageInfo != null) {
            Vector3 pos = damageInfo.Self.transform.position;
            pos.y += damageInfo.Self.Radius;

            string text = damageInfo.FinalDamage.ToString();

            StartDisplayText(text, pos, 0.3f);
        }
    }

    public void StartDisplayText(string text, Vector3 position, float duration) {
        StartCoroutine(DisplayText(text, position, duration));
    }

    private IEnumerator DisplayText(string text, Vector3 position, float duration) {
        TMP_Text damageText = _damageTextPool.GetObject();
        damageText.transform.position = position;
        damageText.text = text;

        yield return YieldInstructionCache.WaitForSeconds(duration);

        _damageTextPool.ReturnObject(damageText);
    }
}
