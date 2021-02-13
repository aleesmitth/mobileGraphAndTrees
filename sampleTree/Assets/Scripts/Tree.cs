using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Tree : MonoBehaviour {
    private Node root = default(Node);
    //the key is concatenation of x position + y position of the node.
    private Dictionary<string, GameObject> nodesDictionary;
    public GameObject treeNodePrefab;
    private GameObject lastInsertedNodeBuffer = default(GameObject);
    [SerializeField]
    public Material defaultNodeMat;

    /*public int Value {
        get;
        private set;
    }*/

    private void Awake() {
        nodesDictionary = new Dictionary<string, GameObject>();
        root = new Node();
        root.value = -1;
        root.position.x = -(float)GameManager.TREE_X_OFFSET;
        root.position.y = GameManager.TREE_Y_OFFSET;
        /*
        Value = root.value;
        Debug.Log(root.value + "\n");*/
    }

    private void OnEnable() {
        EventManager.onDeleteNode += DeleteNode;
    }

    private void OnDisable() {
        EventManager.onDeleteNode -= DeleteNode;
    }


    public void Insert(int value) {
        Vector2 insertedPosition = root.position;
        Vector2 parentPosition = Vector2.zero;
        try {
            root = root.Insert(root, ref value, ref insertedPosition, ref parentPosition, -1);
        }
        catch (HeightTreeLimitException e) {
            Debug.Log(e.Message);
            return;
        }
        catch (PositionOccupiedByNodeException e) {
            Debug.Log(e.Message);
            return;
        }

        GameObject node = NodePool.instance.Get();
        node.transform.position = insertedPosition;
        node.transform.rotation = Quaternion.identity;
        //Instantiate(treeNodePrefab, insertedPosition, Quaternion.identity);
        node.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
        
        //si lista de nodos no esta vacia, restaura color del nodo insertado previamente a la normalidad y dibuja branch
        if (nodesDictionary.Count > 0) {
            lastInsertedNodeBuffer.GetComponentInChildren<MeshRenderer>().material = defaultNodeMat;
            LineRenderer lineRenderer = node.GetComponent<LineRenderer>();
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, new Vector3(insertedPosition.x, insertedPosition.y, 0));
            lineRenderer.SetPosition(1, new Vector3(parentPosition.x, parentPosition.y, 0));
        }
        lastInsertedNodeBuffer = node;

        AddNodeToDictionary(node, insertedPosition);
        /*print(value);
        print(insertedPosition.x + " " + insertedPosition.y);*/
    }

    private void AddNodeToDictionary(GameObject node, Vector3 insertedPosition) {
        var key = insertedPosition.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + insertedPosition.y;
        nodesDictionary.Add(key, node);
    }

    public void PrintTree() {
        root.PrintTree(root);
    }

    private void DeleteNode(Vector3 nodePosition, TextMeshProUGUI textMeshProUGUI) {
        print(nodePosition);
        //deletes the node from the tree.
        int n = 0;
        Dictionary<string, string> textNodesUpdate = new Dictionary<string, string>();
        var node = root.SearchNode(new Vector2(nodePosition.x, nodePosition.y), int.Parse(textMeshProUGUI.text), root, ref n);
        root.DeleteNode(node, textNodesUpdate, out string deletedNodeKey);
        //
        //updates text of modified nodes.
        foreach(KeyValuePair<string, string> entry in textNodesUpdate) {
            nodesDictionary[entry.Key].GetComponentInChildren<TextMeshProUGUI>().text = entry.Value;
        }
        print("LLEGUE HASTA FINAL DE DELETE Y BORRE EL " + nodesDictionary[deletedNodeKey].transform.position);
        
        //borro el nodo de la escena
        NodePool.instance.DestroyObject(nodesDictionary[deletedNodeKey]); 
        //borro el nodo de mi lista de nodos activos
        nodesDictionary.Remove(deletedNodeKey);
        //var key = nodePosition.x.ToString(CultureInfo.InvariantCulture) + nodePosition.y;
        //Destroy(nodesDictionary[key]);
    }

    public void PrintListOfNodes() {
        foreach (var node in nodesDictionary) {
            print(node.Value.transform.position);
        }
    }

    public bool IsPositionOccupied(float x, float y) {
        return nodesDictionary.ContainsKey(x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + y);
    }
}
