﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Snowball : Unit
{
	// Constructor
	public Snowball
	(
		string _prefabPath,
		Direction _rotation,
		Node _source
	)
		: base
	(
			_prefabPath,
			_rotation,
			_source
		)
	{
		SetModel ();
	}

	public override void SetModel ()
	{
		if (model != null) {
			GameObject.Destroy (model);
		}

		if (prefabPath.Contains ("Friend")) {
			MapController.player = this;
		}

		D.Log ("Set model to unit: " + prefabPath);

		model = GameObject.Instantiate (Resources.Load<GameObject> (prefabPath));

		Transform modelTransform = model.GetComponent<Transform> ();

		modelTransform.localPosition = path [0].Position;
		modelTransform.eulerAngles = MapController.GetEulerAngle (modelDirection);

		Move modelMove = model.GetComponent<Move> ();
		if (modelMove != null) {
			modelMove.Path = new List<Vector3> { path [0].Position };
			modelMove.dynamicObject = this;
			model.GetComponent<Rotate> ().target = path [0].Position;
			model.GetComponent<SnowballCollider> ().snowball = this;
		}
	}
		
	public void Dash (Direction _swipeDir)
	{
		// If there is Local Node in _swipeDir
		if (path [0].LocalNodes [(int)_swipeDir] != null && path [0].LocalNodes [(int)_swipeDir].Walk) {
			Node targetWalkNode = path [0].WalkNodes [(int)_swipeDir];

			if (targetWalkNode != null) {
				if (targetWalkNode.type == NodeTypes.Ladder && targetWalkNode.ladderDir == _swipeDir) {
					// If there is Walk Node in _swipeDir and it is downside Ladder
					GoTo (targetWalkNode);
				}
			} else {
				// Or create Walk Node
				CreateLadder (_swipeDir);
			}
		}
	}

	void CreateLadder (Direction _swipeDir)
	{
		Node targetLocalNode = path [0].LocalNodes [(int)_swipeDir];

		// Change target node type to ladder
		targetLocalNode.type = NodeTypes.Ladder;
		Direction laderDir = MapController.GetPointDirection (path [0].Position, targetLocalNode.Position);
		targetLocalNode.ladderDir = laderDir;

		// Update target's walknodes
		for (int i = 0; i < targetLocalNode.LocalNodes.Length; i++)
		{
			Node localNodeOfTheTargetLocalNode = targetLocalNode.LocalNodes [i];
			if (localNodeOfTheTargetLocalNode == null)
			{
				continue;
			}
			if (MapController.AreNodesConnected (
				targetLocalNode.type,
				targetLocalNode.Position,
				targetLocalNode.ladderDir,
				localNodeOfTheTargetLocalNode.type,
				localNodeOfTheTargetLocalNode.Position,
				localNodeOfTheTargetLocalNode.ladderDir
			)) {
				targetLocalNode.WalkNodes [i] = localNodeOfTheTargetLocalNode;
				localNodeOfTheTargetLocalNode.WalkNodes [(int)MapController.GetOppositeDirection ((Direction)i)] = targetLocalNode;
			}
			else
			{
				targetLocalNode.WalkNodes [i] = null;
				localNodeOfTheTargetLocalNode.WalkNodes [(int)MapController.GetOppositeDirection ((Direction)i)] = null;
			}
		}
		// Swap snowball to ladder
		prefabPath = "Tiles/Tile_Ladder";
		modelDirection = laderDir;
		path [0].Walk = true;
		path [0] = targetLocalNode;
		SetModel ();
	}
}

