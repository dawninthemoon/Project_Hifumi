using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RieslingUtils;

public class CombatDamageDisplay : MonoBehaviour, IObserver {
    [SerializeField] private Canvas _damageDisplayCanvas;
    [SerializeField] private Transform _damageDisplayContent;
    [SerializeField] private float _damageTextDuration = 0.3f;
    private static readonly string TextPrefabName = "DamageDisplayText";
    private static readonly string ElementPrefabName = "DamageUIElement";
    private ObjectPool<TMP_Text> _damageTextPool;
    private ObjectPool<DamageUIElement> _uiElementPool;
    private Dictionary<string, DamageUIElement> _uiElementDictionary;
    private List<DamageUIElement> _sortedUIElementList;
    private int _maxDamageSum;

    private void Awake() {
        _uiElementDictionary = new Dictionary<string, DamageUIElement>();
        _sortedUIElementList = new List<DamageUIElement>();
        _damageDisplayCanvas.worldCamera = Camera.main;

        AssetLoader.Instance.LoadAssetAsync<GameObject>(TextPrefabName, (handle) => {
            var prefab = handle.Result.GetComponent<TMP_Text>();
            _damageTextPool = new ObjectPool<TMP_Text>(
                10,
                () => Instantiate(prefab),
                (x) => x.gameObject.SetActive(true),
                (x) => x.gameObject.SetActive(false)
            );
        });
        AssetLoader.Instance.LoadAssetAsync<GameObject>(ElementPrefabName, (handle) => {
            var uiElementPrefab = handle.Result.GetComponent<DamageUIElement>();
            _uiElementPool = new ObjectPool<DamageUIElement>(
                10,
                () => Instantiate(uiElementPrefab, _damageDisplayContent),
                (x) => x.gameObject.SetActive(true),
                (x) => x.Reset()
            );
        });
    }

    public void Reset() {
        _damageTextPool.Clear();
        _uiElementDictionary.Clear();
        _sortedUIElementList.Clear();
        _uiElementPool.Clear();
    }

    public void Notify(IObserverSubject subject) {
        DamageInfo damageInfo = subject as DamageInfo;
        if (damageInfo != null) {
            Vector3 pos = damageInfo.Self.transform.position;
            pos.y += damageInfo.Self.Radius;

            string text = damageInfo.FinalDamage.ToString();

            StartDisplayText(text, pos, _damageTextDuration);

            DamageUIElement selectedElement;
            if (damageInfo.Caster && damageInfo.Caster.CompareTag("Ally")) {
                if (!_uiElementDictionary.TryGetValue(damageInfo.Caster.ID, out selectedElement)) {
                    selectedElement = _uiElementPool.GetObject();
                    selectedElement.SetPortrait(damageInfo.Caster.Info.BodySprite);

                    _uiElementDictionary.Add(damageInfo.Caster.ID, selectedElement);
                    _sortedUIElementList.Add(selectedElement);
                }
                int sum = selectedElement.AddDamage(damageInfo.FinalDamage);
                _maxDamageSum = Mathf.Max(_maxDamageSum, sum);
            }
            
            _sortedUIElementList.Sort((a, b) => b.DamageSum.CompareTo(a.DamageSum));
            int uiElementsCount = _sortedUIElementList.Count;
            for (int i = 0; i < uiElementsCount; ++i) {
                _sortedUIElementList[i].UpdateDamageBarWidth(_maxDamageSum);
                _sortedUIElementList[i].transform.SetSiblingIndex(i + 1);
            }
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
