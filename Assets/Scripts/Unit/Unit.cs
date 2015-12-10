using UnityEngine;

[System.Serializable]
public class Unit {
	public int id;
	public int _direction = 0;
	public int direction {
		get {
			return _direction;
		}
		set {
			_direction += value;
			if (Mathf.Abs(_direction) == 6) {
				_direction = 0;
			}
			if (_direction == -1) {
				_direction = 5;
			}
			Debug.Log("2: " + _direction + " " + value);
		}
	}

	public Cell cell;
	[System.NonSerialized]
	public GameObject unitContainer;
}
