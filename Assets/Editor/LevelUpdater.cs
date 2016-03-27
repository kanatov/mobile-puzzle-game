using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CustomEditor(typeof(MapControllerLoader))]
class LevelUpdater : Editor {
	[SerializeField] public static GameObject[] nodes;
	static readonly float walkNodeDistance = 0.9f;
	static readonly float localNodeDistance = 1f;

	void OnSceneGUI() {
		DrawNodeNetwork ();
	}

	public static void CreateNodes (GameObject _instanceObject) {
		GameObject[] items = Selection.gameObjects;

		foreach (var _item in items) {
			GameObject node = GameObject.Instantiate (_instanceObject);
			node.GetComponent<Transform> ().position = _item.GetComponent<Transform> ().position;
		}
	}

	// Place tiles to container
	public static void UpdateTiles () {
		MapController.SetContainer (MapController.TAG_TILE);
	}

	// Place node to container
	// Update node data
	public static void UpdateNodes () {
		nodes = MapController.SetContainer (MapController.TAG_NODE);

		for (int a = 0; a < nodes.Length; a++) {
			NodeDT nodeDT = nodes[a].GetComponent<NodeDT> ();
			DynamicObjectDT dynamicObjectDT = nodes[a].GetComponent<DynamicObjectDT> ();

			SetNodeNetwork (nodeDT);

			// Set prefab path
			if (dynamicObjectDT) {
				dynamicObjectDT.unitPrefabPath = GetModelPath (dynamicObjectDT);
			}

			// Set icon
			SetIcon (nodes [a]);

			// Save changes
			EditorUtility.SetDirty(nodeDT);
		}
	}

	static void SetNodeNetwork (NodeDT _nodeDT) {
		Vector3 nodeDTPos = _nodeDT.GetComponent<Transform> ().position;
		_nodeDT.walkNodes = new GameObject[MapController.EnumCount<Direction>()];
		_nodeDT.localNodes = new GameObject[MapController.EnumCount<Direction>()];

		_nodeDT.gameObject.SetActive(false);
		Collider[] walkNodes = Physics.OverlapSphere(nodeDTPos, walkNodeDistance);
		Collider[] localNodes = Physics.OverlapSphere(nodeDTPos, localNodeDistance);
		_nodeDT.gameObject.SetActive(true);

		// WalkNodes network
		for (int i = 0; i < walkNodes.Length; i++) {
			Vector3 walkNodeDTPos = walkNodes [i].GetComponent<Transform> ().position;
			NodeDT walkNodeDT = walkNodes [i].GetComponent<NodeDT> ();

			// Alien check
			if (walkNodeDT == null) {
				continue;
			}

			if (MapController.IsNodesConnected (
				_nodeDT.type,
				nodeDTPos,
				_nodeDT.ladderDirection,
				walkNodeDT.type,
				walkNodeDTPos,
				walkNodeDT.ladderDirection
			)) {
				_nodeDT.walkNodes[(int)MapController.GetPointDirection(nodeDTPos, walkNodeDTPos)] = walkNodeDT.gameObject;
			}
		}

		// LocalNodes network
		_nodeDT.localNodes = new GameObject[6];
		for (int j = 0; j < localNodes.Length; j++) {
			NodeDT localNodeDT = localNodes [j].GetComponent<NodeDT> ();

			if (localNodeDT == null) {
				continue;
			}

			int d = (int)MapController.GetPointDirection (nodeDTPos, localNodes[j].GetComponent<Transform>().position);
			_nodeDT.localNodes[d] = localNodes[j].gameObject;
		}
	}


	static void SetIcon (GameObject _nodeDTInstance) {
		IconManager.SetIcon (_nodeDTInstance, IconManager.Icon.DiamondGray);
		NodeDT _nodeDT = _nodeDTInstance.GetComponent<NodeDT>();

		if (!_nodeDT.walk) {
			IconManager.SetIcon (_nodeDTInstance, IconManager.Icon.DiamondRed);
		}		
	}
				
	static void DrawNodeNetwork () {
		if (nodes == null) {
			return;
		}

		foreach (var _node in nodes) {
			if (_node == null) {
				continue;
			}
			NodeDT nodeObject = _node.GetComponent<NodeDT> ();
			Transform nodeTransform = _node.GetComponent<Transform> ();

			foreach (var _neigbour in _node.GetComponent<NodeDT> ().walkNodes) {

				NodeDT neigbourObject = _neigbour.GetComponent<NodeDT> ();
				Transform neigbourTransform = _neigbour.GetComponent<Transform> ();

				if (!nodeObject.GetComponent<SphereCollider>().enabled || !neigbourObject.GetComponent<SphereCollider>().enabled ) {
					Handles.color = Color.red;
				} else{
					Handles.color = Color.white;
				}

				Vector3 middlePoint = Vector3.Lerp(nodeTransform.position, neigbourTransform.position, 0.45f);
				Handles.DrawLine (nodeTransform.position, middlePoint);
			}
		}
	}

	static string GetModelPath (DynamicObjectDT _dynamicObjectDT) {
		if (_dynamicObjectDT.model == null) {
			return null;
		}

		string path = AssetDatabase.GetAssetPath (_dynamicObjectDT.model);
		path = path.Substring (0, path.Length - 7);
		path = path.Substring (17);
		return path;
	}
}