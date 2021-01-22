using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tree : MonoBehaviour {
    private Node root;
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
        int value = Random.Range(0,25);
        root.Insert(root, ref value, ref insertedPosition, 0);
        GameObject treeNode = Instantiate(treeNodePrefab, insertedPosition, Quaternion.identity);
        treeNode.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
        print(value);
        print(insertedPosition.x + " " + insertedPosition.y);
    }

    public void PrintTree() {
        root.PrintTree(root,0);
    }
}
