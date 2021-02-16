using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class TreeContainer : MonoBehaviour {
    private ITreeNode root = default(ITreeNode);
    //the key is concatenation of x position + y position of the node.
    private Dictionary<string, GameObject> nodesDictionary;
    public GameObject treeNodePrefab;
    private GameObject lastInsertedNodeBuffer = default(GameObject);
    [SerializeField]
    public Material defaultNodeMat;

    private bool AVLTree;

    private void Awake() {
        AVLTree = true;
        nodesDictionary = new Dictionary<string, GameObject>();
        //root = new BSTNode {Value = -1, Position = new Vector2(-GameManager.TREE_X_OFFSET, GameManager.TREE_Y_OFFSET)};
        root = new AVLNode() {Value = -1, Position = new Vector2(-GameManager.TREE_X_OFFSET, GameManager.TREE_Y_OFFSET), Depth = 0};
    }

    private void OnEnable() {
        EventManager.onDeleteNode += DeleteNode;
    }

    private void OnDisable() {
        EventManager.onDeleteNode -= DeleteNode;
    }


    public void Insert(int value) {
        Vector2 insertedPosition = root.Position;
        Vector2 parentPosition = Vector2.zero;
        Dictionary<string, List<Vector2>> updateBalanceNodes = new Dictionary<string, List<Vector2>>();
        try {
            root.Insert(ref root, ref value, ref insertedPosition, ref parentPosition, updateBalanceNodes);
            //root.Insert(ref root, ref value, ref insertedPosition, ref parentPosition);
        }
        catch (HeightTreeLimitException e) {
            Debug.Log(e.Message);
            return;
        }
        catch (PositionOccupiedByNodeException e) {
            Debug.Log(e.Message);
            return;
        }
        //getting node from the pool and initializing it
        GameObject node = NodePool.instance.Get();
        node.transform.position = insertedPosition;
        node.transform.rotation = Quaternion.identity;
        node.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
        
        //if list of nodes isn't empty, restores the color of the previous inserted node to normal, and draws branch to parent
        if (nodesDictionary.Count > 0) {
            lastInsertedNodeBuffer.GetComponentInChildren<MeshRenderer>().material = defaultNodeMat;
            LineRenderer lineRenderer = node.GetComponent<LineRenderer>();
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, insertedPosition);
            lineRenderer.SetPosition(1, parentPosition);
        }
        
        lastInsertedNodeBuffer = node;
        AddNodeToDictionary(node, insertedPosition);
        
        var updatedNodesGameObjects = new List<GameObject>();
        foreach (var updatedNode in updateBalanceNodes) {
            nodesDictionary[updatedNode.Key].transform.position = updatedNode.Value[0];
            updatedNodesGameObjects.Add(nodesDictionary[updatedNode.Key]);
            LineRenderer lineRenderer = nodesDictionary[updatedNode.Key].GetComponent<LineRenderer>();
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, updatedNode.Value[0]);
            lineRenderer.SetPosition(1, updatedNode.Value[1]);
            nodesDictionary.Remove(updatedNode.Key);
        }

        foreach (var nodeGameObject in updatedNodesGameObjects) {
            nodesDictionary.Add(MakeNodeKey((Vector2)nodeGameObject.transform.position), nodeGameObject);
        }

        foreach (var updatedNode in updateBalanceNodes) {
            print("el de: " + updatedNode.Key + " pasa a: " + updatedNode.Value);
        }
    }

    private void AddNodeToDictionary(GameObject node, Vector3 insertedPosition) {
        var key = MakeNodeKey(insertedPosition);
        nodesDictionary.Add(key, node);
    }

    public void PrintTree() {
        root.PrintTree(root);
    }

    private void DeleteNode(Vector3 nodePosition, TextMeshProUGUI textMeshProUGUI) {
        print(nodePosition);
        //deletes the node from the tree.
        Dictionary<string, string> textNodesUpdate = new Dictionary<string, string>();
        var node = root.SearchNode(nodePosition, int.Parse(textMeshProUGUI.text), root);
        root.DeleteNode(node, textNodesUpdate, out Vector2 deletedNodePosition);
        var deletedNodeKey = MakeNodeKey(deletedNodePosition);
        //
        //updates text of modified nodes.
        foreach(KeyValuePair<string, string> entry in textNodesUpdate) {
            nodesDictionary[entry.Key].GetComponentInChildren<TextMeshProUGUI>().text = entry.Value;
        }
        //print("LLEGUE HASTA FINAL DE DELETE Y BORRE EL " + nodesDictionary[deletedNodeKey].transform.position);
        
        //deletes node from the scene pooling it back.
        NodePool.instance.DestroyObject(nodesDictionary[deletedNodeKey]); 
        //deletes the node from my active nodes list.
        nodesDictionary.Remove(deletedNodeKey);
    }

    public void PrintListOfNodes() {
        foreach (var node in nodesDictionary) {
            print(node.Value.transform.position);
        }
    }

    public bool IsPositionOccupied(Vector2 position) {
        var key = MakeNodeKey(position);
        return nodesDictionary.ContainsKey(key);
    }

    ///makes key used to store unique node position
    public static string MakeNodeKey(Vector2 position) {
        return position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + position.y;
    }

    public void PrintHeight() {
        root.PrintHeight(root);
    }
    
    public void PrintDepth() {
        root.PrintDepth(root);
    }
}