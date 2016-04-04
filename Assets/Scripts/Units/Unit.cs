using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Unit : DynamicObject
{

    // Constructor
    public Unit
	(
        string _prefabPath,
        Direction _rotation,
        Node _source
    )
    {
        prefabPath = _prefabPath;
        modelDirection = _rotation;

        path = new PathIndexer(new List<Node>{ _source });
        path[0].Walk = false;
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
        }
    }

    public void GoTo(Node _target)
    {
        List<Node> newPath = MapController.GetPath(path[0], _target);
        if (newPath != null)
        {
            path = new PathIndexer(newPath);
            model.GetComponent<Move>().enabled = true;
        }
    }

    public override void Move(string _callback)
    {
        if (path == null)
        {
            Debug.LogWarning("Path == null");
            model.GetComponent<Move>().enabled = false;
            model.GetComponent<Rotate>().enabled = false;
            return;
        }

        if (path.Count < 2)
        {
            GameController.SavePlayerData();
            return;
        }

        if (!path[0].WalkNodes.Contains (path[1]))
        {
            path = new PathIndexer(new List<Node>{ path[0] });
            return;
        }

        List<Vector3> newPath = new List<Vector3>();
		
        // Set path Horisontal
        if (path[0].type == NodeTypes.Horisontal)
        {
            if (path[1].type == NodeTypes.Horisontal)
            {
                newPath.Add(path[1].Position);
            }
            if (path[1].type == NodeTypes.Ladder)
            {
                newPath.Add(GetHorisontalSide(path[0].Position, path[1].Position));
                newPath.Add(GetLadderMiddle(path[1].Position));
            }
        }

        // Set path Ladder
        if (path[0].type == NodeTypes.Ladder)
        {
            if (path[1].type == NodeTypes.Horisontal)
            {
                if (path[0].Position.y > path[1].Position.y)
                {
                    newPath.Add(GetHorisontalSide(path[0].Position, path[1].Position));
                }
                else
                {
                    newPath.Add(GetHorisontalSide(path[1].Position, path[0].Position));
                }
                newPath.Add(path[1].Position);
            }

            if (path[1].type == NodeTypes.Ladder)
            {
                newPath.Add(GetLadderMiddle(path[1].Position));
            }
        }

        model.GetComponent<Move>().Path = newPath;
        model.GetComponent<Move>().enabled = true;
        model.GetComponent<Rotate>().target = path[1].Position;
        model.GetComponent<Rotate>().enabled = true;

        // Activate triggers
        path[0].Walk = true;
        path[1].Walk = false;
        path[1].ActivateTriggers();
        path.RemoveAt(0);
    }

    static Vector3 GetHorisontalSide(Vector3 _source, Vector3 _target)
    {
        _target.y = _source.y;
        Vector3 sidepoint;
        sidepoint = Vector3.Lerp(_source, _target, 0.5f);
        return sidepoint;
    }

    static Vector3 GetLadderMiddle(Vector3 _source)
    {
        _source.y += MapController.tileHeight / 2;
        return _source;
    }
}
