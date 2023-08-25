using System;

public class Singleton<T> where T : new() {
	private static Object _lock = new Object();
	private static T _instance;

	public static T Instance {
		get {
			if (_instance == null) {
				lock (_lock) {
					_instance = new T();
				}
			}
			return _instance;
		}
	}

	public static void DeleteSingleton() {
		_instance = default(T);
	}
}