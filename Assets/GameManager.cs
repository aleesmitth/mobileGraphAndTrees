using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public const int MAX_HEIGHT = 9;
    public const int TREE_X_OFFSET = 10;
    public const int TREE_Y_OFFSET = 4;
    private const char MAGIC_KEY = '*';
    public TreeContainer treeContainer;
    public GraphContainer graphContainer;
    public UIManager uiManager;
    public int BALL_MARGIN = 10;
    
    private int inputNumber = default(int);
    
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

    public bool IsPositionOccupied(Vector2 position) {
        return treeContainer.IsPositionOccupied(position);
    }

    ///makes key used to store unique node position
    public static string MakeNodeKey(Vector2 position) {
        return position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + position.y;
    }

    public void SetInputNumber(TMP_InputField input) {
        var number = default(int);
        try {
            if (input.text.Length > 3) {
                input.text = "";
                throw new InputNumberTooBigException("The input number inserted needs to be smaller.");
            }

            if (!int.TryParse(input.text, out number)) {
                input.text = "";
                throw new InputWasNotANumberException("The input needs to be a number.");
            }
        }
        catch (InputNumberTooBigException e) {
            Debug.Log(e.Message);
            return;
        }
        catch (InputWasNotANumberException e) {
            Debug.Log(e.Message);
            return;
        }
        try {
            if (number < 0) {
                input.text = "";
                throw new InputNumberNegativeException("The input number inserted needs to be positive.");
            }
        }
        catch (InputNumberNegativeException e) {
            Debug.Log(e.Message);
            return;
        }
        uiManager.UpdateInputNumber(number);
        this.inputNumber = number;
    }

    public void InserRandom() {
        for (int i = 0 ; i < this.inputNumber ; i++) {
            treeContainer.Insert(Random.Range(0, 999));
        }
    }

    public void Insert() {
        treeContainer.Insert(this.inputNumber);
    }

    public void ChangeTreeType() {
        treeContainer.ChangeTreeType();
        uiManager.ChangeTreeType();
    }

    public void ActivateGraphMode() {
        treeContainer.enabled = false;
        graphContainer.enabled = true;
    }
    
    public void ActivateTreeMode() {
        graphContainer.enabled = false;
        treeContainer.enabled = true;
    }

    public void SearchForMinNodeACTIVATED() {
        graphContainer.SearchForMinMode();
    }

    public void DeactivateInsertMode() {
        graphContainer.DeactivateInsertNode();
    }

    public void ActivateInsertMode() {
        graphContainer.ActivateInsertNode();
    }
}