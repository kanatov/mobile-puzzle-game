using UnityEngine;

[System.Serializable]
public class Unit {
	public int id;
	public Cell cell;
	[System.NonSerialized]
	public GameObject unitContainer;
}
