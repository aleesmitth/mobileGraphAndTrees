using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public const int MAX_HEIGHT = 9;
    public const int TREE_X_OFFSET = 10;
    public const int TREE_Y_OFFSET = 4;
    public const char MAGIC_KEY = '*';
    public Tree tree;
    
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

    public bool IsPositionOccupied(float x, float y) {
        return tree.IsPositionOccupied(x, y);
    }

    public void Testtt() {
        Node node1 = new Node();
        ref Node node2 = ref node1;
        var node3 = node2;
        node2 = null;
        print(node1);
        print(node2);
        print(node3);
    }
}
