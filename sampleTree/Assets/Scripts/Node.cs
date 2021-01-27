using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Node {
    private const float NODE_SEPARATOR = 1;
    private const int LEFT = -1;
    private const int RIGHT = 1;
    public Vector2 position;
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
        if (root == null || root.value == -1) {
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
            rootPosition.x += (GameManager.TREE_X_OFFSET - n/NODE_SEPARATOR);
            rootPosition.y -= GameManager.TREE_Y_OFFSET;
            Debug.Log("inserta" + value + " en derecha de " + root.value + " parent position: " + parentPosition.x + "," + parentPosition.y+"\n");
            root.rightChild = Insert(root.rightChild, ref value, ref rootPosition, ref parentPosition,n);

            /*Debug.Log("el hijo derecho de " + root.value + " es " + rightChild + " y su valor es " + rightChild.value + "\n");*/
        } else {
            Debug.Log("pasada l en n = " + n + " - vale : " + this.value + "\n");
            parentPosition = rootPosition;
            // divido por n para q no se superpongan los hijos de nodos hermanos.
            rootPosition.x -= (GameManager.TREE_X_OFFSET - n/NODE_SEPARATOR);
            rootPosition.y -= GameManager.TREE_Y_OFFSET;
            Debug.Log("inserta" + value + " en izquierda de " + root.value + " parent position: " + parentPosition.x + "," + parentPosition.y+ "\n");
            root.leftChild = Insert(root.leftChild, ref value, ref rootPosition, ref parentPosition,n);
            /*Debug.Log("el hijo izquierdo de " + root.value + " es " + leftChild + " y su valor es " + leftChild.value + "\n");*/
        }

        return root;
    }

    public void PrintTree(Node root) {
        if (root == null) {
            Debug.Log("null \n");
            return;
        }

        PrintTree(root.leftChild);
        PrintTree(root.rightChild);
    }

    public Node SearchNode(Vector2 position, int value, Node root, ref int n) {
        n++;
        if (root == null) return null;
        if (root.position == position) return root;
        else if (value > root.value) return SearchNode(position, value, root.rightChild, ref n);
        else {
            return SearchNode(position, value, root.leftChild, ref n);
        }
    }

    public void DeleteNode(Node node, int n, Dictionary<string, string> positionsUpdated) {
        if (node.rightChild == null && node.leftChild == null) node = null; //aca solo lo borro
        else if (node.leftChild != null && node.rightChild == null) {
            Debug.Log("estoy borrando el-----: " + node.value + " por izquierda" + "\n");
            // guardo la posicion vieja del nodo
            var key = node.leftChild.position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + node.leftChild.position.y;
            node.leftChild.position = node.position;
            // reemplazo nodo por su hijo izquierdo
            node = node.leftChild;
            // guardo la posicion nueva del nodo
            var newPosition = node.position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + node.position.y;
            if (positionsUpdated.ContainsKey(key)) {
                positionsUpdated.Remove(key);
            }
            positionsUpdated.Add(key, newPosition);
            UpdatePosition(node.leftChild, n+1, LEFT, positionsUpdated);
        }
        else if (node.leftChild == null && node.rightChild != null){
            //reemplazo hijo derecho por el nodo
            Debug.Log("estoy borrando el-----: " + node.value + " por derecha" + "\n");
            // guardo la posicion vieja del nodo
            var key = node.rightChild.position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + node.rightChild.position.y;
            node.rightChild.position = node.position;
            // reemplazo nodo por su hijo izquierdo
            node = node.rightChild;
            // guardo la posicion nueva del nodo
            var newPosition = node.position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + node.position.y;
            if (positionsUpdated.ContainsKey(key)) {
                positionsUpdated.Remove(key);
            }
            positionsUpdated.Add(key, newPosition);
            UpdatePosition(node.rightChild, n+1, RIGHT, positionsUpdated);
        }
        else {
            //busco minimo -> hijo derecha, le aplico delete, reemplazo por nodo
            var min = FindMin(node.rightChild);
            Debug.Log(min.value + " :-----este es el mas chico de la derecha y estoy borrando el-----: " + node.value + "\n");
            DeleteNode(min, n, positionsUpdated);
            min.rightChild = node.rightChild;
            min.leftChild = node.leftChild;
            // guardo la posicion vieja del nodo
            var key = min.position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + min.position.y;
            min.position = node.position;
            // guardo la posicion nueva del nodo
            var newPosition = min.position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + min.position.y;
            if (positionsUpdated.ContainsKey(key)) {
                positionsUpdated.Remove(key);
            }
            positionsUpdated.Add(key, newPosition);
            node = min;
        }
    }

    private void UpdatePosition(Node node, int n, int direction, Dictionary<string, string> positionsUpdated) {
        if (node == null) return;
        
        // guardo la posicion vieja del nodo
        var key = node.position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + node.position.y;
        
        if(direction == LEFT) node.position.x += (GameManager.TREE_X_OFFSET + n / NODE_SEPARATOR);
        else node.position.x -= (GameManager.TREE_X_OFFSET + n / NODE_SEPARATOR);
        node.position.y -= (GameManager.TREE_Y_OFFSET);

        // guardo la posicion nueva del nodo
        var newPosition = node.position.x.ToString(CultureInfo.InvariantCulture) + GameManager.MAGIC_KEY + node.position.y;
        
        if (positionsUpdated.ContainsKey(key)) {
            positionsUpdated.Remove(key);
        }
        positionsUpdated.Add(key, newPosition);
        
        UpdatePosition(node.leftChild, n + 1, direction, positionsUpdated);
        UpdatePosition(node.rightChild, n + 1, direction, positionsUpdated);
    }

    private Node FindMin(Node node) {
        if (node.rightChild == null && node.leftChild == null) return node;
        Node min;
        if (node.rightChild == null && node.leftChild != null) min = FindMin(node.leftChild);
        else if (node.rightChild != null && node.leftChild == null) min = FindMin(node.rightChild);
        else {
            min = FindMin(node.leftChild).value < FindMin(node.rightChild).value ? node.leftChild : node.rightChild;
        }

        return node.value < min.value ? node : min;
    }
}
