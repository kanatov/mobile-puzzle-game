using UnityEngine;
using System.Collections;

public static class D {
	public static void Log (object _msg) {
		Debug.Log (_msg + "\n");
	}
	public static void LogWarning (object _msg) {
		Debug.LogWarning (_msg + "\n");
	}
}
