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

	public void Dash (Direction _swipeDirection)
	{
		if (Path [0].LocalNodes [(int)_swipeDirection] != null) {
			Node targetWalkNode = Path [0].WalkNodes [(int)_swipeDirection];

			if (targetWalkNode != null) {
				GoTo (targetWalkNode);
			} else {
				Node targetLocalNode = Path [0].LocalNodes [(int)_swipeDirection];

				// Change target node type to ladder
				targetLocalNode.type = NodeTypes.Ladder;
				Direction laderDir = MapController.GetPointDirection (Path [0].Position, targetLocalNode.Position);
				targetLocalNode.ladderDirection = laderDir;

				// Update target's walknodes
				for (int i = 0; i < targetLocalNode.LocalNodes.Length; i++) {
					Node localNodeOfTheTargetLocalNode = targetLocalNode.LocalNodes[i];
					if (localNodeOfTheTargetLocalNode == null) {
						continue;
					}

					if (MapController.IsNodesConnected (
						    targetLocalNode.type,
						    targetLocalNode.Position,
						    targetLocalNode.ladderDirection,
							localNodeOfTheTargetLocalNode.type,
						    localNodeOfTheTargetLocalNode.Position,
						    localNodeOfTheTargetLocalNode.ladderDirection
					    )) {
						targetLocalNode.WalkNodes [i] = localNodeOfTheTargetLocalNode;
						localNodeOfTheTargetLocalNode.WalkNodes [(int)MapController.GetOppositeDirection ((Direction)i)] = targetLocalNode;
					} else {
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
	}
}

