using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;
using TMPro;

public class ShopUIHandler : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _shopKeeperTextbox;
    private static readonly float TextChangeDelay = 1f;

    public void ChangeShopkeeperTextbox(string str) {
        StartChangeText(_shopKeeperTextbox, str, TextChangeDelay);
    }

    public void StartChangeText(TMP_Text textObj, string str, float duration) {
        StartCoroutine(ChangeText(textObj, str, duration));
    }

    private IEnumerator ChangeText(TMP_Text textObj, string str, float duration) {
        string defaultText = textObj.text;
        textObj.text = str;

        yield return YieldInstructionCache.WaitForSeconds(duration);

        textObj.text = defaultText;
    }
}
