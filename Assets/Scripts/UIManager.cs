using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour{
    [SerializeField] public TextMeshProUGUI[] inputNumberListeners;
    [SerializeField] public TextMeshProUGUI typeOfTreeListener;
    const string AVL_TREE = "AVL Tree";
    const string BST_TREE = "BST Tree";

    private void Awake() {
        typeOfTreeListener.text = BST_TREE;
    }

    public void UpdateInputNumber(int number) {
        foreach (var box in inputNumberListeners) {
            box.text = number.ToString();
        }
    }

    public void ChangeTreeType() {
        typeOfTreeListener.text = typeOfTreeListener.text switch {
            AVL_TREE => BST_TREE,
            BST_TREE => AVL_TREE,
            _ => typeOfTreeListener.text
        };
    }
}