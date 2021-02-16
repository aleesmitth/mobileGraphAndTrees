using System;
using System.Collections.Generic;
using UnityEngine;

public class AVLNode : BinaryTreeNode, ITreeNode {
    private AVLNode parent;
    private AVLNode leftChild;
    private AVLNode rightChild;
    private int balanceFactor;

    ///i make sure that i can cast BinaryTreeNodes to my node type (AVLNode)
    public sealed override BinaryTreeNode LeftChild {
        get => leftChild;
        set => leftChild = (AVLNode)value;
    }

    ///i make sure that i can cast BinaryTreeNodes to my node type (AVLNode)
    public sealed override BinaryTreeNode RightChild {
        get => rightChild;
        set => rightChild = (AVLNode)value;
    }
    
    public sealed override BinaryTreeNode Parent {
        get => parent;
        set => parent = (AVLNode)value;
    }

    public AVLNode() { 
        leftChild = null; 
        rightChild = null;
        balanceFactor = 0;
    }
    
    public void Insert(ref ITreeNode node, ref int value, ref Vector2 insertedPosition,
        ref Vector2 parentPosition, Dictionary<string,List<Vector2>> balancedNodesPositions = null) {
        var avlNode = (AVLNode) node;
        Insert(ref avlNode, ref value, ref insertedPosition, ref parentPosition, balancedNodesPositions);
    }

    public void DeleteNode(ITreeNode node, Dictionary<string, string> textNodesUpdate, out Vector2 deletedNodePosition) {
        throw new NotImplementedException();
    }

    private void Insert(ref AVLNode avlNode, ref int value, ref Vector2 insertedPosition,
        ref Vector2 parentPosition, Dictionary<string,List<Vector2>> balancedNodesPositions = null, int depth = -2, BinaryTreeNode parentNode = null) {
        depth++;
        if (depth > GameManager.MAX_HEIGHT) {
            throw new HeightTreeLimitException("Max tree height reached, cant insert node.");
        }
        if (avlNode == null) {
            // hardcodeo para no tener 2 nodos superpuestos. no se coloca si ya hay uno ahi.
            if (GameManager.instance.IsPositionOccupied(insertedPosition)) {
                throw new PositionOccupiedByNodeException("Position already occupied by another node.");
            }
            avlNode = new AVLNode {Position = insertedPosition, Value = value, Parent = parentNode, Depth = depth, Height = 0};
        }
        else if (value > avlNode.Value) {
            InsertNodeRecursive(ref avlNode.rightChild, ref value, ref insertedPosition,
                ref parentPosition, avlNode, balancedNodesPositions, 1);
        } else {
            InsertNodeRecursive(ref avlNode.leftChild, ref value, ref insertedPosition,
                ref parentPosition, avlNode, balancedNodesPositions, -1);
        }

        if (avlNode.Value <= -1) return;
        UpdateBalanceFactor(avlNode);
        BalanceNode(avlNode, balancedNodesPositions);
    }

    private void UpdateBalanceFactor(AVLNode avlNode) {
        if(avlNode == null) return;
        avlNode.Height = GetHeight(avlNode);
        Debug.Log(avlNode.Height);
        Debug.Log("Height: " + avlNode.Height);
        avlNode.balanceFactor = GetHeight(avlNode.leftChild) - GetHeight(avlNode.rightChild);
        Debug.Log("Balance factor: " + avlNode.balanceFactor);
    }

    private void BalanceNode(AVLNode avlNode, Dictionary<string,List<Vector2>> balancedNodesPositions) {
        if (avlNode.balanceFactor < 2 && avlNode.balanceFactor > -2) return;
        AVLNode headNode;
        if (avlNode.balanceFactor > 1) {
            if (avlNode.leftChild.rightChild == null || avlNode.leftChild.balanceFactor > 0) {
                headNode = RightRotation(avlNode);
            }
            else {
                headNode = LeftRightRotation(avlNode);
            }
        }
        else {
            if (avlNode.rightChild.leftChild == null || avlNode.rightChild.balanceFactor < 0)
                headNode = LeftRotation(avlNode);
            else {
                headNode = RightLeftRotation(avlNode);
            }
        }
        UpdateBalanceFactor(headNode);
        UpdateBalanceFactor(headNode.rightChild);
        UpdateBalanceFactor(headNode.leftChild);
        UpdatePosition(headNode, avlNode.Position, balancedNodesPositions);
    }

    private void UpdatePosition(AVLNode avlNode, Vector2 position, Dictionary<string,List<Vector2>> balancedNodesPositions) {
        if (avlNode == null) return;
        balancedNodesPositions ??= new Dictionary<string, List<Vector2>>();
        
        var key = TreeContainer.MakeNodeKey(avlNode.Position);
        if (balancedNodesPositions.ContainsKey(key)) {
            balancedNodesPositions[key][0] = position;
            balancedNodesPositions[key][1] = avlNode.parent.Position;
        }
        else {
            balancedNodesPositions.Add(key, new List<Vector2> {position, avlNode.parent.Position});
        }

        avlNode.Position = position;
        var positionRight = position + new Vector2(GameManager.TREE_X_OFFSET - avlNode.Depth, -GameManager.TREE_Y_OFFSET);
        Debug.Log("position right: " + positionRight + "\n");
        var positionLeft = position + new Vector2(-(GameManager.TREE_X_OFFSET - avlNode.Depth), -GameManager.TREE_Y_OFFSET);
        Debug.Log("position left: " + positionLeft + "\n");
        UpdatePosition(avlNode.rightChild, positionRight, balancedNodesPositions);
        UpdatePosition(avlNode.leftChild, positionLeft, balancedNodesPositions);
    }

    private AVLNode RightLeftRotation(AVLNode avlNode) {
        ref var aux = ref avlNode.rightChild.leftChild;
        avlNode.rightChild.leftChild = aux.rightChild;
        avlNode.rightChild = aux;
        return LeftRotation(avlNode);
    }

    private AVLNode LeftRightRotation(AVLNode avlNode) {
        ref var aux = ref avlNode.leftChild.rightChild;
        avlNode.leftChild.rightChild = aux.leftChild;
        avlNode.leftChild = aux;
        return RightRotation(avlNode);
    }

    private AVLNode LeftRotation(AVLNode avlNode) {
        ref var aux = ref avlNode.rightChild.leftChild;
        avlNode.rightChild.leftChild = avlNode;
        avlNode.rightChild = aux;
        return avlNode;
    }

    private AVLNode RightRotation(AVLNode avlNode) {
        Debug.Log("LLEGUEACA-------------------------------------");
        var original = avlNode;
        var node = avlNode.leftChild;
        var branch = avlNode.leftChild.rightChild;
        var parentNode = avlNode.parent;
        if (parentNode.rightChild != null && parentNode.rightChild.Position == avlNode.Position) {
            parentNode.rightChild = original.leftChild;
        }
        else {
            parentNode.leftChild = original.leftChild;
        }

        node.leftChild = original.leftChild.leftChild;
        node.rightChild = original;
        node.rightChild.leftChild = branch;
        Debug.Log("LLEGUEACA------------------------------------- " + node.Value + " - "
        + node.leftChild.Value + " - " + node.rightChild.Value + " - \n");
        Debug.Log("LLEGUEACA------------------------------------- " + node.Position + " - "
                  + node.leftChild.Position + " - " + node.rightChild.Position + " - \n");
        node.rightChild.parent = node;
        node.Depth--;
        node.rightChild.Depth++;
        node.leftChild.Depth--;
        return node;
    }

    private void InsertNodeRecursive(ref AVLNode childNode, ref int value, ref Vector2 insertedPosition,
        ref Vector2 parentPosition, BinaryTreeNode parentNode, Dictionary<string,List<Vector2>> balancedNodesPositions, int childDirection) {
        parentPosition = insertedPosition;
        // divido por n para q no se superpongan los hijos de nodos hermanos.
        insertedPosition += new Vector2(childDirection * (GameManager.TREE_X_OFFSET - parentNode.Depth), -GameManager.TREE_Y_OFFSET);
        Insert(ref childNode, ref value, ref insertedPosition, ref parentPosition, balancedNodesPositions, parentNode.Depth, parentNode);
    }

    private void PrintTree(AVLNode avlNode) {
        //implement
    }

    private AVLNode SearchNode(Vector2 nodeposition, int value, AVLNode avlNode) {
        //implement
        return null;
    }
}