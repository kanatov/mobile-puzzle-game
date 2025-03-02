﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GenericData;
using System.Linq;
using UnityEngine.UI;

public enum NodeTypes
{
    Horisontal = 0,
    Ladder,
    Vertical
}

public enum InteractiveTypes
{
    None = 0,
    Trigger,
    DynamicObject
}

public enum TriggerTypes
{
    General = 0,
    RemoveOnActivation,
    Finish
}

public enum DynamicObjectTypes
{
    Unit = 0,
    SnowBall
}

public enum Direction
{
    Up = 0,
    UpRight,
    DownRight,
    Down,
    DownLeft,
    UpLeft,
}

public static class MapController
{
    public static float tileHeight = 0.5f;
    public static string TAG_UNIT = "Unit";
    public static string TAG_TILE = "Tile";
    public static string TAG_NODE = "Node";
    public static string TAG_TRIGGER = "Trigger";
    public static string TAG_GAMEUI = "GameUI";
    public static string TAG_CONTAINER = "sContainer";

    public static Node[] currentLevelNodes
    {
        get { return GameController.playerData.levelsData[GameController.playerData.currentLevel].currentLevelNodes; }
        set { GameController.playerData.levelsData[GameController.playerData.currentLevel].currentLevelNodes = value; }
    }

    public static List<DynamicObject> dynamicObjects
    {
        get { return GameController.playerData.levelsData[GameController.playerData.currentLevel].dynamicObjects; }
        set { GameController.playerData.levelsData[GameController.playerData.currentLevel].dynamicObjects = value; }
    }

    public static Trigger[] triggers
    {
        get { return GameController.playerData.levelsData[GameController.playerData.currentLevel].triggers; }
        set { GameController.playerData.levelsData[GameController.playerData.currentLevel].triggers = value; }
    }

    public static GameObject nodeCollider = Resources.Load<GameObject>("Nodes/NodeCollider");
    public static Unit player;

    public static List<Trigger> triggersList;
    static GameObject[] currentLevelNodesDT;
    static List<string> triggersDTNames;
    static List<TriggerDT> triggersDT;

    public static void Init()
    {
        D.Log("___Map init");
//		GameObject[] lights = GameObject.FindGameObjectsWithTag ("Light");
//		foreach (var _light in lights) {
//			_light.GetComponent<Light>().shadows = LightShadows.None;
//		}

        GameController.GameUI = GameObject.FindGameObjectsWithTag(TAG_GAMEUI)[0];

        currentLevelNodesDT = GameObject.FindGameObjectsWithTag(TAG_NODE);
        triggersDTNames = new List<string>();
        triggersList = new List<Trigger>();

        if (
            currentLevelNodes == null
            || dynamicObjects == null
            || triggers == null)
        {
            D.Log("___Map init: Prepare New Level");
            PrepareNewLevel();
        }
        else
        {
            D.Log("___Map init: Prepare Game Session");
            PrepareGameSession();
        }

        // Remove references and objects after game inicialisation
        currentLevelNodesDT = null;
        triggersDTNames = null;
        triggersList = null;
        GameObject.DestroyImmediate(GameObject.FindGameObjectWithTag(TAG_NODE + TAG_CONTAINER));

        // Organise scene
        SetContainer(TAG_NODE);
        SetContainer(TAG_TRIGGER);
        SetContainer(TAG_UNIT);

        // FadeIn scene
        GameController.FadeInLevel();
    }


    static void PrepareNewLevel()
    {
        currentLevelNodes = new Node[currentLevelNodesDT.Length];
        List <DynamicObject> newDynamicObjects = new List<DynamicObject>();

        D.Log("Map Init: Populate empty nodes");

        // Populate empty nodes
        for (int i = 0; i < currentLevelNodes.Length; i++)
        {
            NodeDT nodeDT = currentLevelNodesDT[i].GetComponent<NodeDT>();
            currentLevelNodes[i] = new Node(
                i,
                nodeDT.type,
                nodeDT.walk,
                nodeDT.singleActivation,
                nodeDT.touchActiovation,
                nodeDT.GetComponent<Transform>().position,
                new int[nodeDT.walkNodes.Length],
                new int[nodeDT.localNodes.Length],
                new int[nodeDT.triggers.Count]
            );
        }

        D.Log("Map Init: Fill nodes");
        // Fill nodes
        for (int i = 0; i < currentLevelNodes.Length; i++)
        {
            NodeDT nodeDTCur = currentLevelNodesDT[i].GetComponent<NodeDT>();

            // Prepare walknodes
            for (int k = 0; k < currentLevelNodes[i].WalkNodes.Length; k++)
            {
                currentLevelNodes[i].WalkNodes[k] = GetNodeByGO(nodeDTCur.walkNodes[k]);
            }

            // Prepare localnodes
            for (int m = 0; m < currentLevelNodes[i].LocalNodes.Length; m++)
            {
                currentLevelNodes[i].LocalNodes[m] = GetNodeByGO(nodeDTCur.localNodes[m]);
            }

            // Copy TriggersDT from NodeDT once
            if (i == 0)
            {
                triggersDT = nodeDTCur.triggersList;
            }

            // Prepare triggers
            if (nodeDTCur.interactiveType == InteractiveTypes.Trigger)
            {
                for (int l = 0; l < currentLevelNodes[i].Triggers.Length; l++)
                {
                    currentLevelNodes[i].Triggers[l] = GetTrigger(nodeDTCur.triggers[l]);
                }
            }

            // Prepare dynamic objects
            if (nodeDTCur.interactiveType == InteractiveTypes.DynamicObject)
            {
                if (nodeDTCur.dynamicObjectPrefabPath != null && nodeDTCur.dynamicObjectPrefabPath != "")
                {
                    newDynamicObjects.Add(GetDynamicObject(nodeDTCur));
                }
                else
                {
                    D.LogError("Prefab path is empty: " + nodeDTCur.GetComponent<Transform>().position);
                }
            }
        }

        D.Log("Map Init: Copy dynamic objects");
        // Copy new dynamic objects
        dynamicObjects = newDynamicObjects;

        // Copy Trigger List to Array
        triggers = new Trigger[triggersList.Count];
        for (int i = 0; i < triggers.Length; i++)
        {
            triggers[i] = triggersList[i];
        }

        GameController.SavePlayerData();
    }


    static Trigger GetTrigger(string _triggerDTName)
    {
        // Check for duplicate
        int triggerIndex = triggersDTNames.IndexOf(_triggerDTName);
        if (triggerIndex != -1)
        {
            return triggersList[triggerIndex];
        }

        // Trigger doesn't found, so add the new name to the array of existed names
        triggersDTNames.Add(_triggerDTName);

        // Prepare new Trigger
        int id = 0;
        for (int i = 0; i < triggersDT.Count; i++)
        {
            if (triggersDT[i].name == _triggerDTName)
            {
                id = i;
                break;
            }
        }

        // Copy activateNodes
        int[] activateNodes = new int[triggersDT[id].activateNodes.Count];
        for (int i = 0; i < triggersDT[id].activateNodes.Count; i++)
        {
            activateNodes[i] = GetNodeByGO(triggersDT[id].activateNodes[i]).id;
        }

        // Copy path
        List<Node> path = new List<Node>();
        foreach (var _node in triggersDT[id].path)
        {
            path.Add(GetNodeByGO(_node));
        }
        ;

        triggersList.Add(new Trigger(
                triggersList.Count,
                triggersDT[id].type,
                path,
                triggersDT[id].prefabPath,
                triggersDT[id].modelDirection,
                0,
                activateNodes
            ));
        return triggersList.Last();
    }


    static DynamicObject GetDynamicObject(NodeDT _nodeDT)
    {
        DynamicObject dynamicObject = null;

        switch (_nodeDT.dynamicObjectType)
        {
            case DynamicObjectTypes.SnowBall:
                dynamicObject = new Snowball(
                    _nodeDT.dynamicObjectPrefabPath,
                    _nodeDT.dynamicObjectDirection,
                    GetNodeByGO(_nodeDT.gameObject),
                    _nodeDT.tutorialTrigger
                );
                break;

            default :
                dynamicObject = new Unit(
                    _nodeDT.dynamicObjectPrefabPath,
                    _nodeDT.dynamicObjectDirection,
                    GetNodeByGO(_nodeDT.gameObject),
                    _nodeDT.tutorialTrigger
                );
                break;
        }

        return dynamicObject;
    }


    static void PrepareGameSession()
    {
        // Nodes
        foreach (var _node in currentLevelNodes)
        {
            _node.SetCollider();
        }

        // Dynamic Objects
        foreach (var _dynamicObject in dynamicObjects)
        {
            _dynamicObject.SetModel();
        }

        // Triggers
        foreach (var _trigger in triggers)
        {
            _trigger.SetModel();
        }
    }

    // Pathfinding
    public static List<Node> GetPath(Node _source, Node _target)
    {
        if (_source == _target)
        {
            D.LogWarning("Pathfinding: source == target");
            return null;
        }

        List<Node> opened = new List<Node>();
        HashSet<Node> closed = new HashSet<Node>();

        opened.Add(_source);

        while (opened.Count > 0)
        {
            // Assign some active node as current
            Node currentNode = opened[0];

            // Looking for the closest node to our target
            for (int i = 1; i < opened.Count; i++)
            {
                // If the fCost of some node is less then current cell fCost
                // or
                // If the fCost of some node is equal but hCost is less
                if (opened[i].fCost < currentNode.fCost || opened[i].fCost == currentNode.fCost && opened[i].hCost < currentNode.hCost)
                {
                    currentNode = opened[i];
                }
            }

            // Closest node was found
            // Let's remove it from active node and put it to the closed
            opened.Remove(currentNode);
            closed.Add(currentNode);

            if (currentNode == _target)
                break;

            // For every neighbour of current cell
            for (int i = 0; i < currentNode.WalkNodes.Length; i++)
            {
                if (currentNode.WalkNodes[i] == null)
                {
                    continue;
                }

                if (closed.Contains(currentNode.WalkNodes[i]))
                {
                    continue;
                }
				
                if (!currentNode.WalkNodes[i].Walk)
                {
                    continue;
                }

                float newMovementCostToNeghbour = currentNode.gCost + Vector3.Distance(currentNode.Position, currentNode.WalkNodes[i].Position);

                if (newMovementCostToNeghbour < currentNode.WalkNodes[i].gCost || !opened.Contains(currentNode.WalkNodes[i]))
                {
                    currentNode.WalkNodes[i].gCost = newMovementCostToNeghbour;
                    currentNode.WalkNodes[i].hCost = Vector3.Distance(currentNode.WalkNodes[i].Position, _target.Position);

                    currentNode.WalkNodes[i].parent = currentNode;

                    if (!opened.Contains(currentNode.WalkNodes[i]))
                    {
                        opened.Add(currentNode.WalkNodes[i]);
                    }
                }
            }
        }

        if (!closed.Contains(_target))
        {
            Node closestCell = null;
            float distance = 0;

            foreach (var cell in closed)
            {
                float altDistance = Vector3.Distance(cell.Position, _target.Position);
                if (distance == 0 || altDistance < distance)
                {
                    closestCell = cell;
                    distance = altDistance;
                }
            }

            _target = closestCell;
        }

        List<Node> path = new List<Node>();
        Node tmpCell = _target;

        while (tmpCell != _source)
        {
            path.Add(tmpCell);
            tmpCell = tmpCell.parent;
        }
        path.Add(_source);

        path.Reverse();
        return path;
    }
		
    // Helpers
    public static int GetRotationDegree(Direction _unitRotation)
    {
        return (int)_unitRotation * 60;
    }

    public static Vector3 GetEulerAngle(Direction _rotation)
    {
        return new Vector3(0f, GetRotationDegree(_rotation), 0f);
    }

    static Node GetNodeByGO(GameObject _target)
    {
        if (_target == null)
        {
            return null;
        }
        int i = System.Array.IndexOf(currentLevelNodesDT, _target);
        return currentLevelNodes[i];
    }

    // Scene clear
    public static GameObject[] SetContainer(string _tag)
    {
        GameObject container = GameObject.FindGameObjectWithTag(_tag + TAG_CONTAINER);
		
        if (container == null)
        {
            container = new GameObject();
            container.GetComponent<Transform>().position = Vector3.zero;
            string tag = _tag + TAG_CONTAINER;
            container.tag = tag;
            container.name = tag;
        }
		
        Transform containerTransform = container.GetComponent<Transform>();
		
        GameObject[] items = GameObject.FindGameObjectsWithTag(_tag);
		
        foreach (var _item in items)
        {
            _item.GetComponent<Transform>().SetParent(containerTransform);
        }
		
        return items;
    }

    public static Direction GetOppositeDirection(Direction _direction)
    {
        int directionNumber = (int)_direction;
        int directionsCount = System.Enum.GetValues(typeof(Direction)).Length;
        if (directionNumber < 3)
        {
            int oppositeDirection = directionNumber + directionsCount / 2;
            return (Direction)oppositeDirection;
        }
        else
        {
            int oppositeDirection = directionNumber - directionsCount / 2;
            return (Direction)oppositeDirection;
        }
    }

    public static Direction GetPointDirection(Vector3 _source, Vector3 _target)
    {
        Vector3 difference = _target - _source;

        float x = difference.x;
        float y = difference.z;

        if (y > 0)
        {
            if (x > 0)
            {
                return Direction.UpRight;
            }
            if (x < 0)
            {
                return Direction.UpLeft;
            }

            return Direction.Up;
        }
        else
        {
            if (x > 0)
            {
                return Direction.DownRight;
            }
            if (x < 0)
            {
                return Direction.DownLeft;
            }

            return Direction.Down;
        }
    }

    // Update Node Network helpers
    static bool CheckStraightConnection(
        Vector3 _sourcePos,
        Vector3 _targetPos,
        Direction _ladderDirection
    )
    {
        return _ladderDirection == GetPointDirection(_sourcePos, _targetPos);
    }

    static bool CheckOppositConnection(
        Vector3 _sourcePos,
        Vector3 _targetPos,
        Direction _ladderDirection
    )
    {
        Direction suitableDirection = GetOppositeDirection(GetPointDirection(_sourcePos, _targetPos));
        return _ladderDirection == suitableDirection;
    }

    // Update Node Network
    public static bool AreNodesConnected(
        NodeTypes _sourceType,
        Vector3 _sourcePos,
        Direction _sourceLaderDirection,
        NodeTypes _targetType,
        Vector3 _targetPos,
        Direction _targetLaderDirection
    )
    {
        // Hor
        if (_sourceType == NodeTypes.Horisontal)
        {
            // Hor to Hor
            if (_targetType == NodeTypes.Horisontal)
            {
                if (_sourcePos.y == _targetPos.y)
                {
                    return true;
                }
            }

            // Hor to Ladder
            if (_targetType == NodeTypes.Ladder)
            {
                if (_sourcePos.y > _targetPos.y)
                {
                    if (CheckStraightConnection(_sourcePos, _targetPos, _targetLaderDirection))
                    {
                        return true;
                    }
                }
                if (_sourcePos.y == _targetPos.y)
                {
                    if (CheckOppositConnection(_sourcePos, _targetPos, _targetLaderDirection))
                    {
                        return true;
                    }
                }
            }
        }

        // Ladder
        if (_sourceType == NodeTypes.Ladder)
        {
            // Ladder to Hor
            if (_targetType == NodeTypes.Horisontal)
            {
                if (_sourcePos.y < _targetPos.y)
                {
                    if (CheckStraightConnection(_targetPos, _sourcePos, _sourceLaderDirection))
                    {
                        return true;
                    }
                }
                if (_sourcePos.y == _targetPos.y)
                {
                    if (CheckOppositConnection(_targetPos, _sourcePos, _sourceLaderDirection))
                    {
                        return true;
                    }
                }
            }

            // Ladder to Ladder
            if (_targetType == NodeTypes.Ladder)
            {
                if (_sourcePos.y > _targetPos.y)
                {
                    if (
                        _sourceLaderDirection == _targetLaderDirection
                        && _targetLaderDirection == GetPointDirection(_sourcePos, _targetPos)
                    )
                    {
                        return true;
                    }
                }
                if (_sourcePos.y < _targetPos.y)
                {
                    if (
                        _sourceLaderDirection == _targetLaderDirection
                        && _targetLaderDirection == GetPointDirection(_targetPos, _sourcePos)
                    )
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static int EnumCount <T>()
    {
        return System.Enum.GetValues(typeof(T)).Length;
    }
}
