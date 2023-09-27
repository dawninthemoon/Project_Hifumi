using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RandomEvent;

public class SelectionButton : MonoBehaviour {
    private Button _button;
    private TextMeshProUGUI _text;
    private StringBuilder _stringBuilder;
    private static readonly string ColorTextPrefix = "<color=>";
    private static readonly string ColorTextSuffix = "</color>";
    private static readonly int ColorPrefixStartIndex = 7;
    private EventSelections _selectionData;
    public EventSelections SelectionData {
        get { return _selectionData; }
    }

    private void Awake() {
        _button = GetComponent<Button>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _stringBuilder = new StringBuilder();
    }

    public void AddListener(System.Action<EventEffects[]> onButtonClicked) {
        _button.onClick.AddListener(() => {
            EventEffects[] effects = null;
            if (_selectionData != null) {
                effects = _selectionData.Effects;
            }
            onButtonClicked.Invoke(effects);
        });
    }

    public void Initialize(EventSelections selectionData) {
        _selectionData = selectionData;
        if (_selectionData == null) {
            gameObject.SetActive(false);
            return;
        }

        if (selectionData.Name != null) {
            AddText("[ ", "red");
            AddText(selectionData.Name, "red");
            AddText(" ] ", "red");
        }
        AddText(selectionData.Text, "black");
        ApplyText();
        gameObject.SetActive(true);
    }

    public void Reset() {
        gameObject.SetActive(false);
        _text.text = null;
        _selectionData = null;
    }

    public void AddText(string text, string color) {
        string prefix = ColorTextPrefix.Insert(ColorPrefixStartIndex, color);
        _stringBuilder.Append(prefix);
        _stringBuilder.Append(text);
        _stringBuilder.Append(ColorTextSuffix);
    }

    public void ApplyText() {
        _text.text = _stringBuilder.ToString();
        _stringBuilder.Clear();
    }
}
