using UnityEngine;
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
        Node _source,
        bool _tutorialTrigger
    )
        : base
	(
            _prefabPath,
            _rotation,
            _source,
            _tutorialTrigger
        )
    {
        SetModel();
    }

    public override void SetModel()
    {
        if (model != null)
        {
            GameObject.Destroy(model);
        }

        if (prefabPath.Contains("Friend"))
        {
            MapController.player = this;
        }

        D.Log("Set model to unit: " + prefabPath);

        model = GameObject.Instantiate(Resources.Load<GameObject>(prefabPath));

        Transform modelTransform = model.GetComponent<Transform>();

        modelTransform.localPosition = path[0].Position;
        modelTransform.eulerAngles = MapController.GetEulerAngle(modelDirection);

        Move modelMove = model.GetComponent<Move>();
        if (modelMove != null)
        {
            modelMove.Path = new List<Vector3> { path[0].Position };
            modelMove.dynamicObject = this;
            model.GetComponent<Rotate>().target = path[0].Position;
            model.GetComponent<SnowballCollider>().snowball = this;
        }

        // Tutorial
        if (tutorialTrigger && tutorialActivated)
        {
            HideTutorial();
        }

        GameController.SavePlayerData();
    }

    public void Dash(Direction _swipeDir)
    {
        // If there is Local Node in _swipeDir
        if (
            path[0].LocalNodes[(int)_swipeDir] != null
            && path[0].LocalNodes[(int)_swipeDir].Walk
            && path[0].LocalNodes[(int)_swipeDir].type != NodeTypes.Ladder
        )
        {
            Node targetWalkNode = path[0].WalkNodes[(int)_swipeDir];

            if (targetWalkNode != null)
            {
//				if (targetWalkNode.type == NodeTypes.Ladder)
//				{
//					// If there is Walk Node in _swipeDir and it is downside Ladder
//					GoTo(targetWalkNode);
//				}
            }
            else
            {
                // Or create Walk Node
                CreateLadder(_swipeDir);
                tutorialActivated = true;
                HideTutorial();
            }
        }
    }

    void CreateLadder(Direction _swipeDir)
    {
        Node newLadder = path[0].LocalNodes[(int)_swipeDir];

        // Change target node type to ladder
        newLadder.type = NodeTypes.Ladder;
        Direction laderDir = MapController.GetPointDirection(path[0].Position, newLadder.Position);
        newLadder.ladderDir = laderDir;

        // Update target's walknodes
        for (int i = 0; i < newLadder.LocalNodes.Length; i++)
        {
            Node newLadderLocalNode = newLadder.LocalNodes[i];
            if (newLadderLocalNode == null)
            {
                continue;
            }

            if (MapController.AreNodesConnected(
                    newLadder.type,
                    newLadder.Position,
                    newLadder.ladderDir,
                    newLadderLocalNode.type,
                    newLadderLocalNode.Position,
                    newLadderLocalNode.ladderDir
                ))
            {
                newLadder.WalkNodes[i] = newLadderLocalNode;
                newLadderLocalNode.WalkNodes[(int)MapController.GetOppositeDirection((Direction)i)] = newLadder;
            }
            else
            {
                newLadder.WalkNodes[i] = null;
                newLadderLocalNode.WalkNodes[(int)MapController.GetOppositeDirection((Direction)i)] = null;
            }
        }

        // Swap snowball to ladder
        prefabPath = "Tiles/Tile_Ladder";
        modelDirection = laderDir;
        path[0].Walk = true;
        path[0] = newLadder;
        SetModel();
    }

    static void HideTutorial()
    {
        GameObject tutorial = GameObject.FindGameObjectWithTag("Tutorial");
        if (tutorial != null)
        {
            tutorial.SetActive(false);
        }
    }
}

