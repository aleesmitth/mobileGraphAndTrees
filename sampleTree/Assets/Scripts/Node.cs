using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node {
    private Vector2 position;
    public Node leftChild = null;
    public Node rightChild = null;
    public int value;

    public Node() {
        leftChild = null;
        rightChild = null;
    }

    public Node Insert(Node root, ref int value, ref Vector2 rootPosition, ref Vector2 parentPosition, int n) {
        n++;
        if (n > 8) {
            throw new HeightLimitException("Max tree height reached, cant insert node.");
        }
        if (root == null) {
            Debug.Log("nuevo en n = " + n + " - vale : " + value + "\n");
            root = new Node();
            root.position.x = rootPosition.x;
            root.position.y = rootPosition.y;
            root.value = value;
            Debug.Log("inserte " + root.value + "\n");
        }
        else if (value > root.value) {
            Debug.Log("pasada r en n = " + n + " - vale : " + this.value + "\n");
            parentPosition = rootPosition;
            // divido por n para q no se superpongan los hijos de nodos hermanos.
            rootPosition.x += (float)(GameManager.TREE_X_OFFSET - n/1.8);
            rootPosition.y -= GameManager.TREE_Y_OFFSET;
            Debug.Log("inserta" + value + " en derecha de " + root.value + " parent position: " + parentPosition.x + "," + parentPosition.y+"\n");
            root.rightChild = Insert(root.rightChild, ref value, ref rootPosition, ref parentPosition,n);

            /*Debug.Log("el hijo derecho de " + root.value + " es " + rightChild + " y su valor es " + rightChild.value + "\n");*/
        } else {
            Debug.Log("pasada l en n = " + n + " - vale : " + this.value + "\n");
            parentPosition = rootPosition;
            // divido por n para q no se superpongan los hijos de nodos hermanos.
            rootPosition.x -= (float)(GameManager.TREE_X_OFFSET - n/1.8);
            rootPosition.y -= GameManager.TREE_Y_OFFSET;
            Debug.Log("inserta" + value + " en izquierda de " + root.value + " parent position: " + parentPosition.x + "," + parentPosition.y+ "\n");
            root.leftChild = Insert(root.leftChild, ref value, ref rootPosition, ref parentPosition,n);
            /*Debug.Log("el hijo izquierdo de " + root.value + " es " + leftChild + " y su valor es " + leftChild.value + "\n");*/
        }

        return root;
    }

    public void PrintTree(Node root,int n) {
        if (root == null) {
            Debug.Log("null \n");
            return;
        }

        PrintTree(root.leftChild, n);
        PrintTree(root.rightChild, n);
    }
}
