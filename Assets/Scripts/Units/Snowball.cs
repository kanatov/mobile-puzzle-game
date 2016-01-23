using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Snowball : DynamicObject {
	public Snowball (string _prefabPath, Node _source) {
		prefabPath = _prefabPath;

		Path = new PathIndexer (new List<Node>{ _source });
		_source.Walk = false;

		SetModel ();
	}

	public override void SetModel () {
		model = GameObject.Instantiate (Resources.Load<GameObject>(prefabPath));

		Transform modelTransform = model.GetComponent<Transform> ();
		modelTransform.localPosition = Path[0].Position;

		SnowballCollider collider = model.GetComponent<SnowballCollider> ();
		collider.snowball = this;

		model.GetComponent<Move> ().Path = new List<Vector3> { Path [0].Position };
		model.GetComponent<Move> ().dynamicObject = this;
		model.GetComponent<Rotate> ().target = Path[0].Position;
	}

	public void GoTo(SwipeDirection _swipeDirection) {
	}
}
