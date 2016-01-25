using UnityEngine;
using System.Collections;

public static class D {
	public static void Log (object _msg) {
		Debug.Log (_msg + "\n");
	}
	public static void LogWarning (object _msg) {
		Debug.LogWarning (_msg + "\n");
	}
	public static void LogError (object _msg) {
		Debug.LogError (_msg + "\n");
	}

	public static void DrawLine (Vector3 _source, Vector3 _target) {
		Debug.DrawLine (_source, _target, Color.yellow);
	}
}
