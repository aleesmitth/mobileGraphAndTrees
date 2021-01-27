using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public const int TREE_X_OFFSET = 10;
    public const int TREE_Y_OFFSET = 4;
    public const char MAGIC_KEY = '*';
    
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
}
