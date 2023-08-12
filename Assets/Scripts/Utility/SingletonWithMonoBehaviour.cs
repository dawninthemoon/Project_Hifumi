using UnityEngine;

public class SingletonWithMonoBehaviour<T> : MonoBehaviour where T : Component {
	private static Object _lock = new Object();
	private static bool _isApplicationRunning = true;
	private static T _instance;
	public static T Instance {
		get {
			if (_instance == null && _isApplicationRunning) {
				lock (_lock) {
					CreateSingletonInstance();
				}
			}
			return _instance;
		}
	}

	private static void CreateSingletonInstance() {
		_instance = FindObjectOfType<T>();
		if (_instance == null) {
			GameObject obj = new GameObject();
			obj.name = typeof(T).Name;
			_instance = obj.AddComponent<T>();
			DontDestroyOnLoad(obj);
		}
	}

	private void OnApplicationQuit() {
		_isApplicationRunning = false;
	}
}