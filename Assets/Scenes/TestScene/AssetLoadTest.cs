using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetLoadTest : MonoBehaviour {
    private async UniTaskVoid Awake() {
        var list = await AssetManager.Instance.LoadAssetsAsync<Sprite>("EventSprites");
        Debug.Log(list.Count);
    }
}
