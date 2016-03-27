using UnityEngine;
using System.Collections;

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
		SetSnowballModel ();
	}

	public void SetSnowballModel ()
	{
		Move modelMove = model.GetComponent<Move> ();
		if (modelMove != null) {
			model.GetComponent<Move> ().dynamicObject = this;
			model.GetComponent<SnowballCollider> ().snowball = this;
		}
	}

	public void Dash (Direction _swipeDir)
	{
		if (Path [0].LocalNodes [(int)_swipeDir] != null) {
			Node targetWalkNode = Path [0].WalkNodes [(int)_swipeDir];

			if (targetWalkNode != null) {
				if (targetWalkNode.type == NodeTypes.Ladder && targetWalkNode.ladderDir == _swipeDir) {
					GoTo (targetWalkNode);
				}
			} else {
				CreateLadder (_swipeDir);
			}
		}
	}

	void CreateLadder (Direction _swipeDir)
	{
		Node targetLocalNode = Path [0].LocalNodes [(int)_swipeDir];
		// Change target node type to ladder
		targetLocalNode.type = NodeTypes.Ladder;
		Direction laderDir = MapController.GetPointDirection (Path [0].Position, targetLocalNode.Position);
		targetLocalNode.ladderDir = laderDir;
		// Update target's walknodes
		for (int i = 0; i < targetLocalNode.LocalNodes.Length; i++) {
			Node localNodeOfTheTargetLocalNode = targetLocalNode.LocalNodes [i];
			if (localNodeOfTheTargetLocalNode == null) {
				continue;
			}
			if (MapController.IsNodesConnected (targetLocalNode.type, targetLocalNode.Position, targetLocalNode.ladderDir, localNodeOfTheTargetLocalNode.type, localNodeOfTheTargetLocalNode.Position, localNodeOfTheTargetLocalNode.ladderDir)) {
				targetLocalNode.WalkNodes [i] = localNodeOfTheTargetLocalNode;
				localNodeOfTheTargetLocalNode.WalkNodes [(int)MapController.GetOppositeDirection ((Direction)i)] = targetLocalNode;
			}
			else {
				targetLocalNode.WalkNodes [i] = null;
				localNodeOfTheTargetLocalNode.WalkNodes [(int)MapController.GetOppositeDirection ((Direction)i)] = null;
			}
		}
		// Swap snowball to ladder
		prefabPath = "Tiles/Tile_Ladder";
		modelDirection = laderDir;
		Path [0].Walk = true;
		Path [0] = targetLocalNode;
		SetModel ();
		SetSnowballModel ();
	}
}

