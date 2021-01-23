using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tree : MonoBehaviour {
    private Node root;
    //the key is concatenation of x position + y position of the node.
    //private Dictionary<string, GameObject> nodesDictionary;
    public GameObject treeNodePrefab;

    public int Value {
        get;
        private set;
    }

    private void Awake() {
        root = new Node();
        root.value = Random.Range(0, 25);
        Value = root.value;
        Debug.Log(root.value + "\n");
    }

    public void Insert() {
        Vector2 insertedPosition = Vector2.zero;
        Vector2 parentPosition = Vector2.zero;
        int value = Random.Range(0,25);
        try {
            root.Insert(root, ref value, ref insertedPosition, ref parentPosition, 0);
        }
        catch (HeightLimitException e) {
            Debug.Log(e.Message);
            return;
        }

        GameObject treeNode = Instantiate(treeNodePrefab, insertedPosition, Quaternion.identity);
        treeNode.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
        LineRenderer lineRenderer = treeNode.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector3(insertedPosition.x, insertedPosition.y, 0));
        lineRenderer.SetPosition(1, new Vector3(parentPosition.x, parentPosition.y, 0));
        //var key = insertedPosition.x.ToString(CultureInfo.InvariantCulture) + insertedPosition.y;
        //nodesDictionary.Add(key, treeNode);
        print(value);
        print(insertedPosition.x + " " + insertedPosition.y);
    }

    public void PrintTree() {
        root.PrintTree(root,0);
    }
}
