using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public const int TREE_X_OFFSET = 5;
    public const int TREE_Y_OFFSET = 5;
    
    public GameObject rootPrefab;
    private Tree root = null;
    public static GameManager instance = null;
    private void Awake() {
        // if the singleton has been initialized yet
        if (instance != null && instance != this) 
        {
            Destroy(this.gameObject);
        }
 
        // if the singleton hasn't been initialized yet
        instance = this;
        DontDestroyOnLoad( this.gameObject );
    }

    public void Add() {
        if (root == null) {
            GameObject treeNode = Instantiate(rootPrefab) as GameObject;
            root = treeNode.GetComponent<Tree>();
            treeNode.GetComponentInChildren<TextMeshProUGUI>().text = root.Value.ToString();
            Debug.Log(root.Value + "\n");
        }
        else {
            root.Insert();
        }
    }

    public void Printtree() {
        root.PrintTree();
    }
}
