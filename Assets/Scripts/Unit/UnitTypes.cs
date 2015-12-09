using UnityEngine;
using System.Collections;

public static class UnitTypes {
	public static GameObject[] model = new GameObject[] {
		Resources.Load<GameObject>("Prefabs/Models/Units/Friend"),
		Resources.Load<GameObject>("Prefabs/Models/Units/Enemy")
	};
//	public float[] maxHealth new float[] {
//		100,
//		10
//	};
//	public float[] damage = new float[] {
//		1,
//		0.5
//	};
//	public int[] attackDistance;
}
