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


    public void Insert() {
        //for (int i = 0; i < 10; i++) {
            Vector2 insertedPosition = Vector2.zero;
            Vector2 parentPosition = Vector2.zero;
            int value = Random.Range(0, 500);
            try {
                root = root.Insert(root, ref value, ref insertedPosition, ref parentPosition, -1);
            }
            catch (HeightLimitException e) {
                Debug.Log(e.Message);
                return;
            }

            if (lastInsertedNodeBuffer != null) {
                print(lastInsertedNodeBuffer.GetComponentInChildren<MeshRenderer>().material = defaultNodeMat);
            }

            GameObject node = Instantiate(treeNodePrefab, insertedPosition, Quaternion.identity);
            node.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
            LineRenderer lineRenderer = node.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, new Vector3(insertedPosition.x, insertedPosition.y, 0));
            lineRenderer.SetPosition(1, new Vector3(parentPosition.x, parentPosition.y, 0));
            lastInsertedNodeBuffer = node;

            AddNodeToDictionary(node, insertedPosition);
            /*print(value);
            print(insertedPosition.x + " " + insertedPosition.y);*/
        //}
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
        Dictionary<string, string> textNodeUpdate = new Dictionary<string, string>();
        var node = root.SearchNode(new Vector2(nodePosition.x, nodePosition.y), int.Parse(textMeshProUGUI.text), root, ref n);
        root.DeleteNode(ref node, textNodeUpdate, out string deletedNodeKey);
        //
        //updates text of modified nodes.
        foreach(KeyValuePair<string, string> entry in textNodeUpdate) {
            nodesDictionary[entry.Key].GetComponentInChildren<TextMeshProUGUI>().text = entry.Value;
        }

        Destroy(nodesDictionary[deletedNodeKey]);
        //var key = nodePosition.x.ToString(CultureInfo.InvariantCulture) + nodePosition.y;
        //Destroy(nodesDictionary[key]);
    }
}
