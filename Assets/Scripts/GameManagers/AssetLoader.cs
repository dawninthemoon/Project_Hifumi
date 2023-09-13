using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public interface ILoadable {
    static bool IsLoadCompleted {
        get;
    }
}

public class AssetManager : Singleton<AssetManager> {
    public AsyncOperationHandle<T> LoadAssetAsync<T>(string key) where T : Object {
        return Addressables.LoadAssetAsync<T>(key);
    }

    public void LoadAssetAsync<T>(string key, System.Action<AsyncOperationHandle<T>> callback) {
        try {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            handle.Completed += callback;
        }
        catch (System.Exception e) {
            Debug.LogError(e.Message);
        }
    }

    public AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>(string key) where T : Object {
        return Addressables.LoadAssetsAsync<T>(key, (op) => {});
    }

    public void LoadAssetsAsync<T>(string key, System.Action<AsyncOperationHandle<IList<T>>> callback) where T : Object {
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(key, (op) => {});
        handle.Completed += callback;
    }

    public async UniTask<T> InstantiateAsync<T>(string key) where T : Object {
        var handle = Addressables.InstantiateAsync(key);
        await handle.Task;
        return handle.Result as T;
    }
}
