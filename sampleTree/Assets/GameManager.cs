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
    public const char MAGIC_KEY = '*';
    public Tree tree;
    public UIManager uiManager;
    
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

    public bool IsPositionOccupied(float x, float y) {
        return tree.IsPositionOccupied(x, y);
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
            tree.Insert(Random.Range(0, 999));
        }
    }

    public void Insert() {
        tree.Insert(this.inputNumber);
    }
}